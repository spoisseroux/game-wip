Shader "Custom/URP/SpriteEchoShader"
{
    Properties
    {
        [MainTexture] _BaseMap("Base Texture", 2D) = "white" {}
        _BaseColor("Base Color", Color) = (1,1,1,1)
        _MainAlpha("Base Alpha (static)", Range(0,1)) = 1.0

        _GlowColor("Glow Color", Color) = (1,1,1,1)
        _PulseFrequency("Pulse Frequency (per second)", Float) = 0.6
        _MaxScale("Max Pulse Scale", Range(1,4)) = 2.5
        _PulseAlpha("Pulse Alpha (max)", Range(0,2)) = 0.6
        _FadePower("Fade Curve Power", Range(0.1,6)) = 1.5
        _EchoDirection("Echo Direction (XY)", Vector) = (0, 0, 0, 0)
        
        // NEW: Overall opacity control
        _OverallOpacity("Overall Opacity", Range(0,1)) = 1.0
    }

    SubShader
    {
        Tags 
        { 
            "RenderPipeline"="UniversalPipeline"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        ZWrite Off
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "Glow"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma target 3.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float fogFactor : TEXCOORD1;
            };

            sampler2D _BaseMap;
            float4 _BaseMap_ST;

            float4 _BaseColor;
            float _MainAlpha;

            float4 _GlowColor;
            float _PulseFrequency;
            float _MaxScale;
            float _PulseAlpha;
            float _FadePower;
            float4 _EchoDirection;
            
            // NEW: Overall opacity uniform
            float _OverallOpacity;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                VertexPositionInputs posInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionCS = posInputs.positionCS;

                // Transform UV using tiling/offset
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);

                // Compute fog factor for URP
                OUT.fogFactor = ComputeFogFactor(posInputs.positionCS.z);

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;

                // Base sample
                float4 baseSample = tex2D(_BaseMap, uv) * _BaseColor;
                baseSample.a *= _MainAlpha;

                // Continuous echo system
                float time = _Time.y;
                float freq = max(_PulseFrequency, 0.0001);
                
                float3 glowRGB = 0.0;
                float glowA = 0.0;

                // UV center
                float2 centerUV = float2(0.5, 0.5);
                
                // Sample multiple continuous echo layers
                // Each layer is phase-shifted in time for smooth overlap
                const int ECHO_LAYERS = 5;
                
                for (int i = 0; i < ECHO_LAYERS; i++)
                {
                    // Phase shift for this layer
                    float phaseShift = (float)i / (float)ECHO_LAYERS;
                    float layerTime = (time * freq + phaseShift);
                    
                    // Get fractional part for 0-1 cycle
                    float cycle = frac(layerTime);
                    
                    // Calculate scale - grows from 1 to MaxScale
                    float scale = lerp(1.0, _MaxScale, cycle);
                    
                    // Apply directional offset based on cycle progress
                    // This shifts the echo center over time in the specified direction
                    float2 echoOffset = _EchoDirection.xy * cycle * 0.5;
                    float2 echoCenter = centerUV + echoOffset;
                    
                    // Scale UV from the moving echo center
                    float2 uvPulse = (uv - echoCenter) / scale + echoCenter;
                    
                    // Sample texture
                    float4 s = tex2D(_BaseMap, uvPulse);
                    
                    // CRITICAL: Smooth fade using sine wave for perfect looping
                    // sin goes 0->1->0 smoothly without discontinuity
                    float fade = sin(cycle * 3.14159265359);
                    
                    // Apply fade power for shaping
                    fade = pow(fade, _FadePower);
                    
                    // Accumulate this layer
                    float layerAlpha = fade * _PulseAlpha;
                    glowRGB += s.rgb * _GlowColor.rgb * layerAlpha;
                    glowA += s.a * layerAlpha;
                }
                
                // Average the layers
                glowRGB /= (float)ECHO_LAYERS;
                glowA /= (float)ECHO_LAYERS;

                // Blend echoes behind the base texture
                float3 finalRGB = glowRGB * (1.0 - baseSample.a) + baseSample.rgb * baseSample.a;
                float finalA = saturate(glowA * (1.0 - baseSample.a) + baseSample.a);

                // Apply URP fog
                finalRGB = MixFog(finalRGB, IN.fogFactor);

                // NEW: Apply overall opacity
                finalA *= _OverallOpacity;

                return half4(finalRGB, finalA);
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}