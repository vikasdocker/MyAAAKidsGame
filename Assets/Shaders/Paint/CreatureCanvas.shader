Shader "Dab/Paint/CreatureCanvas"
{
    Properties
    {
        _BaseMap ("Base Map", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (1, 1, 1, 1)

        [NoScaleOffset] _PaintMap ("Paint Texture (rgb color, a mask)", 2D) = "black" {}
        _PaintStrength ("Paint Strength", Range(0, 1)) = 1.0

        _RimColor ("Rim Color", Color) = (1.0, 0.95, 0.82, 1)
        _RimPower ("Rim Power", Range(0.5, 8.0)) = 3.0
        _RimStrength ("Rim Strength", Range(0.0, 3.0)) = 1.2

        _AmbientBoost ("Ambient Boost", Range(0, 1)) = 0.35
        _Wrap ("Diffuse Wrap", Range(0, 1)) = 0.6

        _Smoothness ("Smoothness", Range(0, 1)) = 0.35
        _SpecularColor ("Specular", Color) = (0.25, 0.25, 0.25, 1)
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            Cull Back
            ZWrite On

            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _SpecularColor;
                float4 _RimColor;
                float  _PaintStrength;
                float  _RimPower;
                float  _RimStrength;
                float  _AmbientBoost;
                float  _Wrap;
                float  _Smoothness;
            CBUFFER_END

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_PaintMap);
            SAMPLER(sampler_PaintMap);

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 normalWS   : TEXCOORD2;
                float  fogFactor  : TEXCOORD3;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings Vertex (Attributes IN)
            {
                Varyings OUT = (Varyings)0;

                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                VertexPositionInputs pos = GetVertexPositionInputs(IN.positionOS.xyz);
                VertexNormalInputs nrm = GetVertexNormalInputs(IN.normalOS);

                OUT.positionCS = pos.positionCS;
                OUT.positionWS = pos.positionWS;
                OUT.normalWS = nrm.normalWS;
                OUT.uv = IN.uv;
                OUT.fogFactor = ComputeFogFactor(pos.positionCS.z);

                return OUT;
            }

            // Painted colour wins over the base map, scaled by the mask's alpha.
            // Alpha 0 means unpainted, so the child can never "erase" their work by
            // painting a neutral colour — the base creature is always reachable.
            half4 SampleSurface (float2 uv)
            {
                half4 baseSample = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv) * _BaseColor;
                half4 paintSample = SAMPLE_TEXTURE2D(_PaintMap, sampler_PaintMap, uv);

                half mask = saturate(paintSample.a) * _PaintStrength;
                half3 albedo = lerp(baseSample.rgb, paintSample.rgb, mask);

                return half4(albedo, baseSample.a);
            }

            half4 Fragment (Varyings IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);

                half4 surface = SampleSurface(IN.uv);

                float3 normalWS = normalize(IN.normalWS);
                float3 viewDirWS = SafeNormalize(GetWorldSpaceViewDir(IN.positionWS));

                Light mainLight = GetMainLight();
                float NdotL = dot(normalWS, mainLight.direction);
                half wrapped = saturate((NdotL + _Wrap) / (1.0 + _Wrap));

                half attenuation = mainLight.shadowAttenuation;
                half3 diffuse = mainLight.color * wrapped * attenuation;

                #if defined(_ADDITIONAL_LIGHTS)
                    uint lightCount = GetAdditionalLightsCount();
                    float4 additionalShadowMask = half4(1.0, 1.0, 1.0, 1.0);
                    LIGHT_LOOP_BEGIN(lightCount)
                        Light light = GetAdditionalLight(lightIndex, IN.positionWS, additionalShadowMask);
                        half NdotLA = saturate(dot(normalWS, light.direction) * 0.5 + 0.5);
                        diffuse += light.color * NdotLA * light.shadowAttenuation;
                    LIGHT_LOOP_END
                #endif

                // Candy Sunrise: lift the floor so nothing reads as shadow-dark.
                // Half-lambert ambient keeps the terminator soft and high-key.
                half3 ambient = SampleSH(normalWS) + _AmbientBoost;
                half3 lighting = ambient + diffuse;

                // Rim from behind and above. viewDirWS faces the camera, so a
                // negative dot puts the rim on the silhouette edge — which is what
                // keeps the creature readable against a bright background.
                half rim = pow(saturate(1.0 - dot(normalWS, viewDirWS)), _RimPower);
                half3 rimLight = _RimColor.rgb * rim * _RimStrength;

                half3 finalColor = surface.rgb * lighting + rimLight;

                half3 halfDir = SafeNormalize(mainLight.direction + viewDirWS);
                half specular = pow(saturate(dot(normalWS, halfDir)), max(1.0, _Smoothness * 128.0));
                finalColor += _SpecularColor.rgb * specular * _Smoothness;

                finalColor = MixFog(finalColor, IN.fogFactor);

                return half4(finalColor, surface.a);
            }
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Lit"
}