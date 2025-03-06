Shader "DCFApixels/DebugX/Samples/FakeLitShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _FakeLightDir ("Fake Light Direction", Vector) = (0,1,0,0)
        _FakeLightColor ("Fake Light Color", Color) = (1,1,1,1)
        _FakeAmbientColor ("Fake Ambient Color", Color) = (0.2,0.2,0.2,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float4 pos : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float4 _FakeLightDir;
            float4 _FakeLightColor;
            float4 _FakeAmbientColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Нормализуем нормаль и направление света
                float3 normal = normalize(i.worldNormal);
                float3 lightDir = normalize(_FakeLightDir.xyz);

                // Вычисляем фейковое освещение
                float NdotL = max(dot(normal, lightDir), 0.0);
                float3 lighting = _FakeAmbientColor.rgb + _FakeLightColor.rgb * NdotL;

                // Применяем текстуру и цвет
                fixed4 texColor = tex2D(_MainTex, i.uv) * _Color;
                fixed4 finalColor = texColor * float4(lighting, 1.0);

                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}