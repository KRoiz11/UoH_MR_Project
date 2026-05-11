Shader "Custom/RetailShelf"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.8, 0.75, 0.6, 1)
        _SpecularColor ("Specular Color", Color) = (1, 1, 1, 1)
        _Shininess ("Shininess", Float) = 32
    }

    SubShader
    {
        Tags 
        { 
            "RenderPipeline"="UniversalPipeline" 
            "RenderType"="Opaque" 
        }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct appdata
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct v2f
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS    : TEXCOORD0;
                float3 positionWS  : TEXCOORD1;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _SpecularColor;
                float  _Shininess;
            CBUFFER_END

            v2f vert(appdata IN)
            {
                v2f OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.normalWS    = TransformObjectToWorldNormal(IN.normalOS);
                OUT.positionWS  = TransformObjectToWorld(IN.positionOS.xyz);
                return OUT;
            }

            half4 frag(v2f IN) : SV_Target
            {
                Light mainLight = GetMainLight();

                float3 normal   = normalize(IN.normalWS);
                float3 lightDir = normalize(mainLight.direction);
                float3 viewDir  = normalize(GetWorldSpaceViewDir(IN.positionWS));
                float3 halfDir  = normalize(lightDir + viewDir);

                // Diffuse
                float diff = max(dot(normal, lightDir), 0.0);

                // Specular
                float spec = pow(max(dot(normal, halfDir), 0.0), _Shininess);

                float3 col = _BaseColor.rgb * diff
                           + _SpecularColor.rgb * spec;

                return half4(col, 1.0);
            }
            ENDHLSL
        }
    }
}