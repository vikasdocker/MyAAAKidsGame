Shader "Dab/Paint/PaintStamp"
{
    // Additive brush stamp used to composite a dab of colour into the paint
    // texture. Deliberately unlit and unlit-only — this never appears in a frame.

    Properties
    {
        _BrushColor ("Brush Colour", Color) = (1, 1, 1, 1)
        _BrushRadius ("Brush Radius (UV)", Range(0.001, 0.5)) = 0.05
        _BrushHardness ("Brush Hardness", Range(0, 1)) = 0.5
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
        }

        Pass
        {
            Name "PaintStamp"
            Tags { "LightMode" = "SRPDefaultUnlit" }

            Cull Off
            ZWrite Off
            ZTest Always
            Blend One One

            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // _BrushUV is a global set per stamp by CreaturePaintController, not a
            // material property — a single brush material is reused across every
            // dab in a stroke without re-instantiating it.
            float2 _BrushUV;

            CBUFFER_START(UnityPerMaterial)
                float4 _BrushColor;
                float  _BrushRadius;
                float  _BrushHardness;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
            };

            Varyings Vertex (Attributes IN)
            {
                Varyings OUT = (Varyings)0;

                // Full-viewport triangle in clip space. No camera transform: the
                // stamp is composited straight into the paint texture, which is
                // UV space, not world space.
                OUT.positionCS = float4(IN.uv * 2.0 - 1.0, 0.0, 1.0);
                OUT.positionCS.y = -OUT.positionCS.y;
                OUT.uv = IN.uv;

                return OUT;
            }

            half4 Fragment (Varyings IN) : SV_Target
            {
                // Radius measured with wraparound so a stroke crossing the UV
                // seam deposits on both edges instead of clipping at the border.
                float2 delta = IN.uv - _BrushUV;
                delta -= round(delta);
                float dist = length(delta);

                float falloff = saturate(1.0 - dist / max(_BrushRadius, 1e-5));
                float mask = smoothstep(_BrushHardness, 1.0, falloff);

                // Colour in rgb, coverage in alpha. Alpha accumulates to 1 where
                // paint has been laid down; the canvas shader reads it as the
                // blend factor, so overlap never darkens and never double-blends.
                return half4(_BrushColor.rgb * mask, mask * _BrushColor.a);
            }
            ENDHLSL
        }
    }
}