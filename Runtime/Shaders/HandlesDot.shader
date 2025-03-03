Shader "DCFApixels/DebugX/Handles Dot"
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

        float _DebugX_GlobalDotSize;      
        float4 _DebugX_GlobalColor;      

        float GetHandleSize(float3 objectPosition)
        {
            float3 viewDir = normalize(_WorldSpaceCameraPos - objectPosition);

            float distance = length(_WorldSpaceCameraPos - objectPosition);
            float isOrthographic = UNITY_MATRIX_P[3][3];
            distance = lerp(distance, 1, isOrthographic);
            
            float fov = radians(UNITY_MATRIX_P[1][1] * 2.0);

            float scale = distance * (1 / fov) * 0.015;

            return scale * _DebugX_GlobalDotSize;
        }

        v2f vert (appdata_t v)
        {
            v2f o;
            UNITY_SETUP_INSTANCE_ID(v);

            float scaleMultiplier = GetHandleSize(mul(UNITY_MATRIX_M, float4(0, 0, 0, 1)).xyz);
            float4x4 M = UNITY_MATRIX_M;
            M._m00 *= scaleMultiplier;
            M._m11 *= scaleMultiplier;
            M._m22 *= scaleMultiplier;

            float4 worldOrigin = mul(M, float4(0, 0, 0, 1));

            float4 viewOrigin = float4(UnityObjectToViewPos(float3(0, 0, 0)), 1);
            float4 worldPos = mul(M, v.vertex);

            float4 viewPos = worldPos - worldOrigin + viewOrigin;

            float4 clipsPos = mul(UNITY_MATRIX_P, viewPos);
            o.vertex = clipsPos;

            //o.vertex = UnityObjectToClipPos(v.vertex);
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