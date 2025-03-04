Shader "DCFApixels/DebugX/Handles Line"
{
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

        struct InstanceData
        {
            float4x4 m;
            float4 color;
        };

        struct appdata_t
        {
            float4 vertex : POSITION;
        };

        struct v2f
        {
            float4 vertex : SV_POSITION;
            half4 color : COLOR;
        };

        StructuredBuffer<InstanceData> _DataBuffer;
        float4 _DebugX_GlobalColor;      

        v2f vert (appdata_t v, uint instanceID : SV_InstanceID)
        {
            v2f o;
            InstanceData data = _DataBuffer[instanceID];
            
            float4 pos = mul(data.m, v.vertex);
            o.vertex = UnityObjectToClipPos(pos);
            o.color = data.color * _DebugX_GlobalColor;
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