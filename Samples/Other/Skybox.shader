Shader "DCFApixels/DebugX/Samples/Skybox"
{
    Properties
    {
        _TopColor ("Top Color", Color) = (1,1,1,1)
        _ButtomColor ("Buttom Color", Color) = (1,1,1,1)
        _Factor ("Factor", Float) = 1
        _Offset ("Offset", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float4 _TopColor;
            float4 _ButtomColor;
            float _Factor;
            float _Offset;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 color = lerp(_ButtomColor, _TopColor, clamp(i.uv.y / _Factor + 0.5 + _Offset, 0, 1));
                return color;
            }
            ENDCG
        }
    }
}
