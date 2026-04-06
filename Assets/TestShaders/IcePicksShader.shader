Shader "Custom/IcePicksShader"
{
    Properties
    {
        [BaseColor] _BaseColor("Base Color", Color) = (0.2, 0, 1, 0.1)
        [BaseTexture] _BaseMap("Base Map", 2D) = "white"
        [ScrollSpeed] _ScrollSpeed("Scroll Speed", Vector) = (0, 0, 0, 0)
        [OffsetMax] _OffsetMax("Max Offset", Vector) = (0, 0, 0)
    }

    SubShader
    {
        Tags 
        {  
            "RenderType" = "Opaque" 
            "RenderPipeline" = "UniversalPipeline" 
        }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _BaseMap_ST;
                float2 _ScrollSpeed;
                float3 _OffsetMax;
            CBUFFER_END

            v2f vert(appdata v)
            {
                v2f OUT = (v2f)0;
                v.positionOS.xz += float3(_OffsetMax.x * cos(_Time.y), _OffsetMax.y * sin(_Time.y), _OffsetMax.z * cos(_Time.y));
                OUT.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(v.uv, _BaseMap);
                return OUT;
            }

            float4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv + _Time.y * _ScrollSpeed;
                float4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv) + _BaseColor;
                return color;
            }
            ENDHLSL
        }
    }
}
