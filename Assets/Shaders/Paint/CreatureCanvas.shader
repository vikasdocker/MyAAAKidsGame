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

        // --- Tactile relief (Option 2) -------------------------------------
        // The paint mask is treated as a height field, so a stroke becomes a
        // physically raised contour rather than a flat decal.
        //
        // This is not decoration. palette.json critical-pairs C3 (wet vs dry)
        // and C4 (print mask vs painted mark, six value collisions at 0.1-1.8%
        // luma) both name "0.02-0.04 H relief" as the *declared* carrier that
        // makes those pairs legible when colour collapses. The default here is
        // 0.035 H, mid-band of that range. It is deliberately NOT 0.06: that
        // would exceed the specified band on a curve whose radius is not yet
        // authored, and an over-deep relief on a sphere self-shadows into a
        // crease. C4 also requires a 0.06 H feathered crayon edge, which is what
        // _ReliefFeather carries.
        [NoScaleOffset] _ReliefNormalMap ("Relief Normal Map", 2D) = "bump" {}
        _ReliefHeight ("Relief Height", Range(0, 0.06)) = 0.035
        _ReliefNormalStrength ("Relief Normal Strength", Range(0, 2)) = 0.9
        _ReliefNormalBlend ("Relief Normal Blend", Range(0, 1)) = 0.45
        _ReliefFeather ("Relief Feather", Range(0.001, 0.08)) = 0.012

        // Sheen is a *value* channel, not a colour channel. art-bible 1.6
        // requires redundancy across shape, motion, and audio for state changes,
        // and 2.1 hard law 1 forbids any surface dropping below 25% of base
        // luminance. A glossy highlight added on top of the albedo satisfies
        // both: it raises local luminance for legibility without shifting hue,
        // so it stays readable under protanopia, deuteranopia, and tritanopia,
        // and in a greyscale screenshot.
        _SheenColor ("Sheen Color", Color) = (1.0, 0.98, 0.94, 1)
        _SheenStrength ("Sheen Strength", Range(0, 1.5)) = 0.45
        _SheenPower ("Sheen Power", Range(1, 64)) = 18.0

        // One keyword strips the entire relief + sheen block for the lowest
        // device tier. Without it every mobile device pays for vertex texture
        // fetches and four extra taps, which is the single most expensive
        // decision in this shader.
        [Toggle(_CREATURE_RELIEF)] _CreatureRelief ("Enable Tactile Relief", Float) = 1
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
        #pragma shader_feature_local _ _CREATURE_RELIEF


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
                float  _ReliefHeight;
                float  _ReliefNormalStrength;
                float  _ReliefNormalBlend;
                float  _ReliefFeather;
                float4 _SheenColor;
                float  _SheenStrength;
                float  _SheenPower;
            CBUFFER_END

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_PaintMap);
            SAMPLER(sampler_PaintMap);
            TEXTURE2D(_ReliefNormalMap);
            SAMPLER(sampler_ReliefNormalMap);

            // Height at a UV, from the paint mask's alpha. Alpha 0 is unpainted
            // and therefore flat, so relief appears exactly where the child has
            // painted and nowhere else. This is what makes the relief read as
            // their own work rather than as part of the creature's skin.
            half SampleReliefHeight (float2 uv)
            {
                // Vertex-stage fetch: this is called from Vertex, so the sample
                // must carry an explicit mip. SAMPLE_TEXTURE2D lowers to an
                // implicit-LOD Sample which D3D11's vs_4_0 profile cannot map
                // ("cannot map expression to vs_4_0"); WebGL rakes it through the
                // same restriction. SAMPLE_TEXTURE2D_LOD issues textureLod /
                // samplelevel, which both vendor pipelines accept in the vertex
                // stage. UV mip 0 is deliberate: the mask is late-bind paint and
                // its texel footprint at model LOD is thinner than a full mip
                // pyramid would average.
                #if defined(_CREATURE_RELIEF)
                    half mask = SAMPLE_TEXTURE2D_LOD(_PaintMap, sampler_PaintMap, uv, 0).a;
                    return saturate(mask) * _PaintStrength;
                #else
                    return 0.0h;
                #endif
            }

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
                float3 reliefNormalWS : TEXCOORD4;
                half   reliefMask     : TEXCOORD5;
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

                half reliefMask = SampleReliefHeight(IN.uv);

                #if defined(_CREATURE_RELIEF)
                    // Displace along the normal. Sampling the mask in the vertex
                    // stage is the cost that makes this feature expensive, and it
                    // is why _CREATURE_RELIEF exists as a strip point: a vertex
                    // texture fetch is unsupported on the oldest GLES2-class
                    // hardware this project may still have to run on, and on
                    // tilers it is a bandwidth cost in a stage that is not
                    // bandwidth-rich.
                    float3 displacedOS = IN.positionOS.xyz + IN.normalOS * (reliefMask * _ReliefHeight);

                    // Re-derive the position inputs from the displaced vertex so
                    // the shadow, depth, and clip-space positions all agree with
                    // what is drawn. Skipping this is the classic version of this
                    // bug: the surface displaces, but its shadow does not, and
                    // the child sees a raised contour with a shadow cast from the
                    // undisplaced original.
                    VertexPositionInputs displacedPos = GetVertexPositionInputs(displacedOS);
                    pos = displacedPos;

                    // No tangent frame is built here. The mesh carries no
                    // tangents (CreatureTestMeshGenerator never calls
                    // SetTangents) and Attributes declares no tangentOS, so
                    // nrm.tangentWS would be undefined. The relief normal is
                    // reconstructed per-pixel from screen-space derivatives
                    // instead, which needs no authored tangents and therefore
                    // cannot silently break on a mesh that lacks them.
                    OUT.reliefNormalWS = float3(0.0, 0.0, 0.0);
                #else
                    OUT.reliefNormalWS = float3(0.0, 0.0, 0.0);
                #endif

                OUT.positionCS = pos.positionCS;
                OUT.positionWS = pos.positionWS;
                OUT.normalWS = nrm.normalWS;
                OUT.uv = IN.uv;
                OUT.reliefMask = reliefMask;
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

            // Relief normal, reconstructed per-pixel from screen-space
            // derivatives of the height field.
            //
            // Why derivatives rather than a tangent frame
            // -------------------------------------------
            // The correct treatment of a height field needs tangents to convert
            // the height gradient into world space. The creature mesh has no
            // tangents: CreatureTestMeshGenerator builds UVs but never calls
            // SetTangents, and the vertex Attributes declare no tangentOS. Reading
            // nrm.tangentWS anyway would compile and then produce an undefined
            // value, which is the worst failure mode available because it looks
            // correct on some hardware.
            //
            // Taking ddx/ddy of the height and the position gives the surface
            // gradient directly in screen space, and the same derivatives yield
            // the tangent frame. Cost is two extra texture taps when relief is on
            // and none at all when it is off, and it works on any mesh, with or
            // without tangents, including the low-poly prototype.
            //
            // The feather term keeps a single-texel step from becoming a hard
            // crease. Without it, bilinear-filtered paint texels produce a
            // visibly faceted surface at grazing angles.
            half3 ReliefNormalFromDerivatives (Varyings IN, half height, half feather)
            {
                float3 posDx = ddx(IN.positionWS);
                float3 posDy = ddy(IN.positionWS);
                float2 uvDx = ddx(IN.uv);
                float2 uvDy = ddy(IN.uv);

                // Standard cotangent-frame construction. Guarded because the
                // denominator is zero on a degenerate UV triangle, and a
                // divide-by-zero here produces NaN normals that show up as black
                // or magenta pixels on some mobile drivers.
                float det = uvDx.x * uvDy.y - uvDx.y * uvDy.x;
                float invDet = (abs(det) < 1e-8) ? 0.0 : (1.0 / det);

                float3 T = (posDx * uvDy.y - posDy * uvDx.y) * invDet;
                float3 B = (-posDx * uvDx.x + posDy * uvDx.x) * invDet;

                // Height gradient in the same screen space, scaled by relief height
                // so the perturbation is proportional to the actual displacement.
                float hDx = ddx(height) * _ReliefHeight;
                float hDy = ddy(height) * _ReliefHeight;

                float3 grad = (T * hDx + B * hDy) * _ReliefNormalStrength / max(feather, 1e-4);

                // Authored normal-map detail, added on top of the derived
                // gradient. Both terms are world-space tilts expressed in the
                // same tangent frame, so they sum rather than needing a blend:
                // the derivative term carries the real paint relief, and the map
                // carries surface tooth the height field cannot resolve at low
                // resolution. _ReliefNormalBlend holds the map back so it stays a
                // texture accent and cannot swamp the relief that actually
                // represents where the child painted.
                half3 tn = UnpackNormal(
                    SAMPLE_TEXTURE2D(_ReliefNormalMap, sampler_ReliefNormalMap, IN.uv));

                grad += (T * tn.x + B * tn.y) * _ReliefNormalBlend;

                return normalize(normalize(IN.normalWS) - grad);
            }


            half4 Fragment (Varyings IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);

                half4 surface = SampleSurface(IN.uv);

                float3 normalWS = normalize(IN.normalWS);
                half reliefMask = saturate(IN.reliefMask);

                #if defined(_CREATURE_RELIEF)
                    // Blend the relief normal in over the geometric one by the
                    // paint mask, so an unpainted area keeps its original shading
                    // exactly. Mixing by mask rather than replacing is what keeps
                    // the creature's base look intact where the child has not
                    // painted.
                    if (reliefMask > 0.001h)
                    {
                        half3 reliefN = ReliefNormalFromDerivatives(IN, reliefMask, _ReliefFeather);
                        normalWS = normalize(lerp(normalWS, reliefN, reliefMask));
                    }
                #endif

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

                #if defined(_CREATURE_RELIEF)
                    // Sheen: a tight achromatic highlight on the painted contour.
                    //
                    // This is an accessibility feature, not decoration, and the
                    // reason is specific. art-bible 1.6 requires that state be
                    // distinguishable without colour, and the greyscale check in
                    // 1.5 is a release gate. A painted stroke and unpainted skin
                    // are two different albedos, but two different albedos can
                    // easily land on the same luminance, and a colourblind child
                    // loses the distinction entirely. A specular sheen separates
                    // them on a channel that survives desaturation: the contour
                    // is brighter *and* glossier, so it still reads in greyscale.
                    //
                    // Deliberately near-white and desaturated. A tinted sheen would
                    // reintroduce hue as a distinguishing channel, which is the
                    // exact failure the rule exists to prevent. art-bible 2.1 hard
                    // law 1 also requires no surface to fall below 25% of base
                    // luminance; a sheen only ever adds, so it cannot violate that.
                    half sheenMask = pow(saturate(dot(normalWS, halfDir)), _SheenPower);
                    half sheenFresnel = pow(1.0h - saturate(dot(normalWS, viewDirWS)), 3.0h);
                    half sheen = (sheenMask * 0.7h + sheenFresnel * 0.3h) * reliefMask;
                    finalColor += _SheenColor.rgb * sheen * _SheenStrength;
                #endif

                finalColor = MixFog(finalColor, IN.fogFactor);

                return half4(finalColor, surface.a);
            }
            ENDHLSL
        }

        // ShadowCaster
        //
        // Present because the forward pass displaces vertices along the normal.
        // Without a matching caster pass the creature would cast the shadow of its
        // undisplaced silhouette, so a raised painted contour would sit inside a
        // shadow shaped like the flat original. That reads as a lighting bug and
        // would be blamed on the art, not the missing pass.
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull Back

            HLSLPROGRAM
        #pragma vertex ShadowPassVertex
        #pragma fragment ShadowPassFragment
        #pragma shader_feature_local _ _CREATURE_RELIEF


            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

            // This pass displaces vertices exactly like the forward pass, so it
            // reads the same material inputs and must declare them for itself:
            // passes compile as independent shader units and inherit nothing.
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
                float  _ReliefHeight;
                float  _ReliefNormalStrength;
                float  _ReliefNormalBlend;
                float  _ReliefFeather;
                float4 _SheenColor;
                float  _SheenStrength;
                float  _SheenPower;
            CBUFFER_END

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_PaintMap);
            SAMPLER(sampler_PaintMap);
            TEXTURE2D(_ReliefNormalMap);
            SAMPLER(sampler_ReliefNormalMap);

            half SampleReliefHeightShadow (float2 uv)
            {
                // Same explicit-mip rule as SampleReliefHeight in the forward
                // pass: this runs in ShadowPassVertex, so the fetch must map to
                // a vs_4_0/WebGL vertex-stage textureLod.
                #if defined(_CREATURE_RELIEF)
                    return saturate(SAMPLE_TEXTURE2D_LOD(_PaintMap, sampler_PaintMap, uv, 0).a) * _PaintStrength;
                #else
                    return 0.0h;
                #endif
            }

            float3 ApplyReliefShadow (float3 positionOS, float3 normalOS, float2 uv)
            {
                #if defined(_CREATURE_RELIEF)
                    return positionOS + normalOS * (SampleReliefHeightShadow(uv) * _ReliefHeight);
                #else
                    return positionOS;
                #endif
            }

            // Deliberately not using URP's ShadowCasterPass.hlsl, which defines
            // its own Attributes, GetShadowPositionHClip, and ShadowPassVertex.
            // Including it would put a second ShadowPassVertex in this pass and
            // the non-displacing one would win, silently undoing the relief in
            // the shadow while leaving the forward pass correct. The cost of
            // re-declaring the bias maths is duplicated code; the cost of the
            // include is a bug that looks like a lighting problem.
            struct ShadowAttributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 texcoord   : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct ShadowVaryings
            {
                float4 positionCS : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            float3 _LightDirection;
            float3 _LightPosition;

            float4 GetReliefShadowPositionHClip(ShadowAttributes input)
            {
                float3 displacedOS = ApplyReliefShadow(
                    input.positionOS.xyz, input.normalOS, input.texcoord);

                float3 positionWS = TransformObjectToWorld(displacedOS);
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);

                #if _CASTING_PUNCTUAL_LIGHT_SHADOW
                    float3 lightDirectionWS = normalize(_LightPosition - positionWS);
                #else
                    float3 lightDirectionWS = _LightDirection;
                #endif

                float4 positionCS = TransformWorldToHClip(
                    ApplyShadowBias(positionWS, normalWS, lightDirectionWS));

                return ApplyShadowClamping(positionCS);
            }

            ShadowVaryings ShadowPassVertex(ShadowAttributes input)
            {
                ShadowVaryings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);

                output.positionCS = GetReliefShadowPositionHClip(input);
                return output;
            }

            half4 ShadowPassFragment(ShadowVaryings input) : SV_TARGET
            {
                UNITY_SETUP_INSTANCE_ID(input);
                return 0;
            }
            ENDHLSL
        }

        // DepthOnly
        //
        // Required for the same reason as ShadowCaster. URP writes this when a
        // depth texture is requested; the Mobile pipeline asset currently sets
        // m_RequireDepthTexture to 0, so this pass is stripped on the low tier and
        // costs nothing there. It exists so that raising the requirement later, or
        // enabling SSAO or a depth-based effect, does not silently reintroduce a
        // depth and shadow mismatch.
        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }

            ZWrite On
            ColorMask R
            Cull Back

            HLSLPROGRAM
        #pragma vertex DepthOnlyVertex
        #pragma fragment DepthOnlyFragment
        #pragma shader_feature_local _ _CREATURE_RELIEF


            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

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
                float  _ReliefHeight;
                float  _ReliefNormalStrength;
                float  _ReliefNormalBlend;
                float  _ReliefFeather;
                float4 _SheenColor;
                float  _SheenStrength;
                float  _SheenPower;
            CBUFFER_END

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_PaintMap);
            SAMPLER(sampler_PaintMap);
            TEXTURE2D(_ReliefNormalMap);
            SAMPLER(sampler_ReliefNormalMap);

            struct DepthAttributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct DepthVaryings
            {
                float4 positionCS : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            half SampleReliefHeightDepth (float2 uv)
            {
                // Explicit-mip vertex fetch; see SampleReliefHeight.
                #if defined(_CREATURE_RELIEF)
                    return saturate(SAMPLE_TEXTURE2D_LOD(_PaintMap, sampler_PaintMap, uv, 0).a) * _PaintStrength;
                #else
                    return 0.0h;
                #endif
            }

            DepthVaryings DepthOnlyVertex (DepthAttributes IN)
            {
                DepthVaryings OUT = (DepthVaryings)0;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                #if defined(_CREATURE_RELIEF)
                    float3 displacedOS = IN.positionOS.xyz
                        + IN.normalOS * (SampleReliefHeightDepth(IN.uv) * _ReliefHeight);
                #else
                    float3 displacedOS = IN.positionOS.xyz;
                #endif

                OUT.positionCS = TransformObjectToHClip(displacedOS.xyz);
                return OUT;
            }

            half4 DepthOnlyFragment (DepthVaryings IN) : SV_Target
            {
                return 0;
            }
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Lit"
}