Shader "DCFApixels/DebugX/Handles Unlit"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        [Toggle] _VertexColor("Vertex Color", Float) = 0
        [Toggle] _FakeLight("Fake Light", Float) = 0
        [Toggle] _OnePass("One Pass", Float) = 0
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
        #pragma shader_feature_local _VERTEXCOLOR_ON
        #pragma shader_feature_local _FAKELIGHT_ON
        #pragma shader_feature_local _ONEPASS_ON
        #pragma instancing_options procedural:setup

        #include "UnityCG.cginc"

#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
        struct InstanceData
        {
            float4x4 m;
            float4 color;
        };
        StructuredBuffer<InstanceData> _DataBuffer;
#endif


        void setup()
        {
#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
            InstanceData data = _DataBuffer[unity_InstanceID];
            float4x4 m = data.m;

            float rotation = m.w * m.w * _Time.y * 0.5f;
            rotate2D(m.xz, rotation);

            unity_ObjectToWorld._11_21_31_41 = float4(m.w, 0, 0, 0);
            unity_ObjectToWorld._12_22_32_42 = float4(0, m.w, 0, 0);
            unity_ObjectToWorld._13_23_33_43 = float4(0, 0, m.w, 0);
            unity_ObjectToWorld._14_24_34_44 = float4(m.xyz, 1);
            unity_WorldToObject = unity_ObjectToWorld;
            unity_WorldToObject._14_24_34 *= -1;
            unity_WorldToObject._11_22_33 = 1.0f / unity_WorldToObject._11_22_33;

            _Color = data.color;
#endif
        }




        struct appdata_t
        {
            float4 vertex : POSITION;
#if _FAKELIGHT_ON
            float3 normal : NORMAL;
#endif
#if _VERTEXCOLOR_ON
            float4 color : COLOR;
#endif
            uint instanceID : SV_InstanceID;
            UNITY_VERTEX_INPUT_INSTANCE_ID 
        };

        struct v2f
        {
            float4 vertex : SV_POSITION;
            half4 color : COLOR;
        };

        float4 _Color;
        float4 _DebugX_GlobalColor;      

        v2f vert (appdata_t v)
        {
            v2f o;

#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
            InstanceData data = _DataBuffer[v.instanceID];
            float4 pos = mul(data.m, v.vertex);
            o.vertex = UnityObjectToClipPos(pos);
            half4 color = data.color;
#else
            o.vertex = UnityObjectToClipPos(v.vertex);
            half4 color = _Color;
#endif

#if _VERTEXCOLOR_ON
            color *= v.color;
#endif
            o.color = color * _DebugX_GlobalColor;
                
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