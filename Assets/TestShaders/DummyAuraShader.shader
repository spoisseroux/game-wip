Shader "Custom/DummyAuraShader"
{
    Properties
    {
        // Base material properties
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}

        // Aura properties
        [AuraColor] _AuraColor("Aura Color", Color) = (1, 1, 1, 1)
        [Intensity] _Intensity("Intensity", Range(0, 2)) = 0.75
        [EdgeCutoff] _EdgeCutoff ("Edge Cutoff",    Range(0, 1))  = 0.3
        [RimPower] _RimPower("Rim Sharpness",  Range(0.5, 8)) = 4.0
        [AuraAlpha] _AuraAlpha("Aura Alpha",     Range(0, 1))   = 0.0
        [PulseSpeed] _PulseSpeed("Pulse Speed",    Range(0, 5))   = 1.5
        [PulseMin] _PulseMin("Pulse Min",      Range(0, 1))   = 0.4
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent" 
            "RenderPipeline" = "UniversalPipeline" 
            "Queue"         = "Transparent+1"  // renders after the base mesh
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            Blend SrcAlpha One
            ZWrite Off
            Cull Back

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float3 viewDirWS : TEXCOORD1;
                float2 uv : TEXCOORD2;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _BaseMap_ST;
                float4 _AuraColor;
                float _RimPower;
                float _EdgeCutoff;
                float _Intensity;
                float _AuraAlpha;
                float _PulseSpeed;
                float _PulseMin;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                VertexPositionInputs posInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                VertexNormalInputs normInputs = GetVertexNormalInputs(IN.normalOS);

                OUT.positionCS = posInputs.positionCS;
                OUT.normalWS = normInputs.normalWS;
                OUT.viewDirWS = GetWorldSpaceViewDir(posInputs.positionWS);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                float3 N = normalize(IN.normalWS);
                float3 V = normalize(IN.viewDirWS);

                // Base lighting
                float4 baseTex   = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                float4 baseColor = _BaseColor;

                // Fresnel rim — peaks at silhouette edges
                float fresnel = pow(1.0 - saturate(dot(N, V)), _RimPower);

                // Hard discard below the cutoff threshold —
                // interior fragments never reach the output
                clip(fresnel - _EdgeCutoff);

                // Pulse
                float pulse = lerp(_PulseMin, 1.0, sin(_Time.y * _PulseSpeed) * 0.5 + 0.5);

                // Additive aura on top of base color
                float3 color = _AuraColor.rgb * _Intensity;
                float  alpha = fresnel;

                return float4(pulse * color, alpha);
            }
            ENDHLSL
        }
    }
}
