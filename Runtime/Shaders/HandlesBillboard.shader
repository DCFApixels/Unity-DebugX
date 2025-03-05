Shader "DCFApixels/DebugX/Handles Buillboard"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" "ForceNoShadowCasting"="True" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Fog { Mode Off }
	    Lighting Off 
        Offset -1, -1

        ZTest On

        CGINCLUDE
        #pragma vertex vert
        #pragma fragment frag
        #pragma multi_compile_instancing
        #include "UnityCG.cginc"

        struct appdata_t
        {
            float4 vertex : POSITION;
            float4 color : COLOR;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        struct v2f
        {
            float4 vertex : SV_POSITION;
            half4 color : COLOR;
        };

        UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
        UNITY_INSTANCING_BUFFER_END(Props)

        float4 _DebugX_GlobalColor;

        v2f vert (appdata_t v)
        {
            v2f o;
            UNITY_SETUP_INSTANCE_ID(v);

            float4 worldOrigin = mul(UNITY_MATRIX_M, float4(0, 0, 0, 1));
            float4 viewOrigin = float4(UnityObjectToViewPos(float3(0, 0, 0)), 1);
            float4 worldPos = mul(UNITY_MATRIX_M, v.vertex);

            float4 viewPos = worldPos - worldOrigin + viewOrigin;

            float4 clipsPos = mul(UNITY_MATRIX_P, viewPos);
            o.vertex = clipsPos;


            o.color = v.color * UNITY_ACCESS_INSTANCED_PROP(Props, _Color) * _DebugX_GlobalColor;
            return o;
        }
        ENDCG

        Pass
        {
            ZTest LEqual
            CGPROGRAM
            half4 frag (v2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
        Pass
        {
            ZTest Greater
            CGPROGRAM
            half4 frag (v2f i) : SV_Target
            {
                return i.color * half4(1, 1, 1, 0.1);
            }
            ENDCG
        }
    }
}