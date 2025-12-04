Shader "Custom/IntersectionGradient"
{
    Properties
    {
        _GradientColor ("Gradient Color", Color) = (0, 1, 1, 1)
        _GradientHeight ("Gradient Height", Float) = 2.0
        _GradientPower ("Gradient Power", Float) = 1.0
        _IntersectionWidth ("Intersection Width", Float) = 0.5
        _IntersectionSoftness ("Intersection Softness", Float) = 0.1
        
        [Enum(Off,0,Front,1,Back,2)] _Cull ("Cull Mode", Float) = 2
        [Toggle] _FogEnabled ("Enable Fog", Float) = 1
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent" 
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }
        
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite On
        ZTest LEqual
        Cull [_Cull]
        
        Pass
        {
            Name "IntersectionGradient"
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma target 3.0
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 screenPos : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 viewDirWS : TEXCOORD2;
                float fogFactor : TEXCOORD3;
            };
            
            CBUFFER_START(UnityPerMaterial)
                half4 _GradientColor;
                float _GradientHeight;
                float _GradientPower;
                float _IntersectionWidth;
                float _IntersectionSoftness;
                float _FogEnabled;
            CBUFFER_END
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                
                output.positionCS = vertexInput.positionCS;
                output.screenPos = ComputeScreenPos(vertexInput.positionCS);
                output.positionWS = vertexInput.positionWS;
                output.viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);
                
                // Calculate fog
                output.fogFactor = ComputeFogFactor(vertexInput.positionCS.z);
                
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                // Sample depth texture
                float2 screenUV = input.screenPos.xy / input.screenPos.w;
                
                float rawDepth = SampleSceneDepth(screenUV);
                float sceneDepthEye = LinearEyeDepth(rawDepth, _ZBufferParams);
                float surfaceDepthEye = input.screenPos.w;
                
                // Calculate depth difference (positive when behind scene geometry)
                float depthDiff = sceneDepthEye - surfaceDepthEye;
                
                // If we're in front of scene geometry, show nothing
                if (depthDiff < 0.0)
                {
                    discard;
                }
                
                // Calculate intersection intensity (1 at intersection, 0 far away)
                float intersectionFade = 1.0 - saturate(depthDiff / _IntersectionWidth);
                intersectionFade = smoothstep(0.0, _IntersectionSoftness, intersectionFade);
                
                // Reconstruct world position of scene geometry
                float3 viewDir = normalize(input.viewDirWS);
                float depthDelta = depthDiff;
                float3 sceneWorldPos = input.positionWS - viewDir * depthDelta;
                
                // Calculate vertical distance from intersection point
                float verticalDist = input.positionWS.y - sceneWorldPos.y;
                
                // Only show gradient above the intersection
                if (verticalDist < 0.0)
                {
                    discard;
                }
                
                // Calculate gradient based on vertical distance
                float gradientFactor = saturate(verticalDist / _GradientHeight);
                gradientFactor = 1.0 - pow(gradientFactor, _GradientPower);
                
                // Combine intersection and gradient factors
                float alpha = intersectionFade * gradientFactor * _GradientColor.a;
                
                // Early discard for fully transparent pixels
                if (alpha < 0.01)
                {
                    discard;
                }
                
                half4 finalColor = half4(_GradientColor.rgb, alpha);
                
                // Apply fog if enabled
                if (_FogEnabled > 0.5)
                {
                    finalColor.rgb = MixFog(finalColor.rgb, input.fogFactor);
                }
                
                return finalColor;
            }
            ENDHLSL
        }
    }
    
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}