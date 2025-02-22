Shader "DCFApixels/DebugX/Handles Lit"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off Fog { Mode Off }
	    Lighting Off 

        ZTest On

        CGINCLUDE
        #pragma vertex vert
        #pragma fragment frag
        #pragma multi_compile_instancing
        #include "UnityCG.cginc"

        struct appdata_t
        {
            float3 vertex : POSITION;
            float3 normal : NORMAL;
            float4 color : COLOR;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };
        struct v2f
        {
            float4 vertex : SV_POSITION;
            half4 color : COLOR0;
        };

        UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
        UNITY_INSTANCING_BUFFER_END(Props)

        float4 _DebugX_GlobalColor;      

        v2f vert (appdata_t v)
        {
            v2f o;
            UNITY_SETUP_INSTANCE_ID(v);

            float4 icolor = UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
            if (icolor.a >= 0)
            {
                v.color = v.color * icolor;
            }
            float3 eyeNormal = normalize (mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal).xyz);
            float nl = saturate(eyeNormal.z);
            float lighting = 0.333 + nl * 0.667 * 0.5;
            float4 color;
            color.rgb = lighting * v.color.rgb;
            color.a = v.color.a;
            o.color = saturate(color) * _DebugX_GlobalColor;
            o.vertex = UnityObjectToClipPos(v.vertex);
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
                return i.color * half4(1, 1, 1, 0.2);
            }
            ENDCG
        }
    }
}
