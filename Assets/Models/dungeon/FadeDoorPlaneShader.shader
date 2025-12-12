Shader "Custom/StarfieldDoor"
{
    Properties
    {
        
        [Header(Starfield Settings)]
        _FadeDistance ("Fade Distance", Float) = 5.0
        _StarDensity ("Star Density", Float) = 50.0
        _TwinkleSpeed ("Twinkle Speed", Float) = 0.2
        _StarBrightness ("Star Brightness", Float) = 1.0
        _StarSize ("Star Size", Range(0.1, 5.0)) = 1.0
        _FadeTarget ("Fade Target Position", Vector) = (0, 0, 0, 0)
        _FadeOffset ("Fade Offset (Controlled by Script)", Float) = 0.0
        
        [Header(Debug)]
        [Toggle] _DebugFade("Debug Fade (Red=Far, Green=Close)", Float) = 0
        
        [Header(Rendering)]
        [Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull Mode", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "UniversalMaterialType" = "Unlit"
            "RenderPipeline" = "UniversalPipeline"
        }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite On
        Cull [_Cull]

        Pass
        {
            Name "Unlit"

            HLSLPROGRAM
            #pragma target 2.0

            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 viewDirWS : TEXCOORD2;
                float fogCoord : TEXCOORD3;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            CBUFFER_START(UnityPerMaterial)
                float _FadeDistance;
                float _FadeOffset;
                float _StarDensity;
                float _TwinkleSpeed;
                float _StarBrightness;
                float _StarSize;
                float4 _FadeTarget;
                float _DebugFade;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vertexInput.positionCS;
                output.positionWS = vertexInput.positionWS;
                output.uv = input.uv;
                
                // Calculate view direction from camera to this pixel (normalized)
                output.viewDirWS = normalize(vertexInput.positionWS - GetCameraPositionWS());
                
                output.fogCoord = ComputeFogFactor(vertexInput.positionCS.z);

                return output;
            }

            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453123);
            }

            float stars(float3 viewDir, float density, float size)
            {
                // Use view direction for 3D star field
                float3 gridUV = viewDir * density;
                float3 gridID = floor(gridUV);
                float3 gridLocal = frac(gridUV);
                
                float star = 0.0;
                
                // Check 3x3x3 grid
                for(int z = -1; z <= 1; z++)
                {
                    for(int y = -1; y <= 1; y++)
                    {
                        for(int x = -1; x <= 1; x++)
                        {
                            float3 offset = float3(x, y, z);
                            float3 cellID = gridID + offset;
                            
                            // Generate pseudo-random 3D position in cell
                            float3 starPos = float3(
                                hash(cellID.xy),
                                hash(cellID.yz + 13.7),
                                hash(cellID.xz + 27.3)
                            );
                            
                            float brightness = hash(cellID.xy + cellID.z * 41.3);
                            
                            // Very slow twinkle
                            float twinkle = sin(_TimeParameters.x * _TwinkleSpeed + hash(cellID.xy) * 6.28) * 0.2 + 0.8;
                            brightness *= twinkle;
                            
                            float3 toStar = starPos + offset - gridLocal;
                            float dist = length(toStar);
                            
                            float starValue = 1.0 - smoothstep(0.0, 0.05 * size, dist);
                            star += starValue * brightness;
                        }
                    }
                }
                
                return saturate(star * _StarBrightness);
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                // Use _FadeTarget if set (w > 0.5), otherwise use camera position
                float3 targetPos = _FadeTarget.w > 0.5 ? _FadeTarget.xyz : GetCameraPositionWS();
                float dist = distance(targetPos, input.positionWS);
                
                // Subtract offset to make transparent area larger
                // When dist is less than offset, fade should be 0 (fully transparent)
                float adjustedDist = max(0, dist - _FadeOffset);
                
                // Fade: 0 when close (transparent), 1 when far (opaque)
                float fade = saturate(adjustedDist / _FadeDistance);
                
                // Debug mode: show distance as colors
                if (_DebugFade > 0.5)
                {
                    float3 debugCol = lerp(float3(0, 1, 0), float3(1, 0, 0), fade);
                    return half4(debugCol, 1.0);
                }
                
                // Generate 3D starfield based on view direction
                float starfield = stars(input.viewDirWS, _StarDensity, _StarSize);
                
                float3 starColor = float3(1, 1, 1);
                float colorVar = hash(input.viewDirWS.xy * 100.0);
                if(colorVar > 0.9) starColor = float3(0.8, 0.9, 1.0);
                else if(colorVar > 0.8) starColor = float3(1.0, 0.9, 0.8);
                
                float3 col = starColor * starfield;
                
                // Alpha: fully transparent (0) when close, opaque (1) when far
                // Discard pixels that are nearly transparent to avoid rendering issues
                float alpha = fade;
                if (alpha < 0.01) discard;
                
                half4 finalColor = half4(col, alpha);
                finalColor.rgb = MixFog(finalColor.rgb, input.fogCoord);
                
                return finalColor;
            }
            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }

            ZWrite On
            ColorMask R
            Cull [_Cull]

            HLSLPROGRAM
            #pragma target 2.0
            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            #pragma multi_compile_instancing
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 position : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings DepthOnlyVertex(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                output.positionCS = TransformObjectToHClip(input.position.xyz);
                return output;
            }

            half4 DepthOnlyFragment(Varyings input) : SV_TARGET
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                return 0;
            }
            ENDHLSL
        }

        Pass
        {
            Name "DepthNormalsOnly"
            Tags { "LightMode" = "DepthNormalsOnly" }

            ZWrite On
            Cull [_Cull]

            HLSLPROGRAM
            #pragma target 2.0
            #pragma vertex DepthNormalsVertex
            #pragma fragment DepthNormalsFragment

            #pragma multi_compile_instancing
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings DepthNormalsVertex(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                return output;
            }

            half4 DepthNormalsFragment(Varyings input) : SV_TARGET
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                return half4(normalize(input.normalWS), 0);
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}