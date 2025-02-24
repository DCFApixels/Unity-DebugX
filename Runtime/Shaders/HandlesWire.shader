Shader "DCFApixels/DebugX/Handles Wire"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "ForceSupported" = "True" "Queue" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off Cull Off Fog { Mode Off }
        Offset -1, -1
	    Lighting Off 

        ZTest Off

        CGINCLUDE
        #pragma vertex vert
        #pragma geometry geom
        #pragma fragment frag
        #pragma multi_compile_instancing
        #include "UnityCG.cginc"

        struct appdata
        {
            float4 vertex : POSITION;
            float4 color : COLOR;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        struct v2g
        {
            float4 vertex : POSITION;
            float4 color : COLOR;
        };

        struct g2f
        {
            float4 vertex : SV_POSITION;
            float4 color : COLOR;
        };

        UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
        UNITY_INSTANCING_BUFFER_END(Props)

        float4 _DebugX_GlobalColor;      

        v2g vert(appdata v)
        {
            v2g o;
            UNITY_SETUP_INSTANCE_ID(v);
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.color = v.color * UNITY_ACCESS_INSTANCED_PROP(Props, _Color) * _DebugX_GlobalColor;
            return o;
        }

        [maxvertexcount(6)]
        void geom(triangle v2g input[3], inout LineStream<g2f> lineStream)
        {
            g2f o;


            for (uint i = 0; i < 3; i++)
            {
                o.vertex = input[i].vertex;
                o.color = input[i].color;
                lineStream.Append(o);

                uint next = (i + 1) % 3;
                o.vertex = input[next].vertex;
                o.color = input[next].color;
                lineStream.Append(o);

                lineStream.RestartStrip();
            }
        }
        ENDCG

        Pass
        {
            ZTest LEqual
            CGPROGRAM
            half4 frag (g2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
        Pass
        {
            ZTest Greater
            CGPROGRAM
            half4 frag (g2f i) : SV_Target
            {
                return i.color * half4(1, 1, 1, 0.05);
            }
            ENDCG
        }
    }
}