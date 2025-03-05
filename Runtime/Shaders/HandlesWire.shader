Shader "DCFApixels/DebugX/Handles Wire"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        [Toggle] _VertexColor("Vertex Color", Float) = 0
        [Toggle] _FakeLight("Fake Light", Float) = 0
        [Toggle] _Billboard("Billboard", Float) = 0
        [Toggle] _Dot("Dot", Float) = 0
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
        #pragma geometry geom
        #pragma fragment frag
        #pragma multi_compile_instancing
        #pragma shader_feature_local _VERTEXCOLOR_ON
        #pragma shader_feature_local _FAKELIGHT_ON
        #pragma shader_feature_local _ONEPASS_ON
        #pragma shader_feature_local _BILLBOARD_ON
        #pragma shader_feature_local _DOT_ON
        #pragma instancing_options procedural:setup

        #include "UnityCG.cginc"

#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
        struct InstanceData
        {
            float4x4 m;
            float4 color;
        };
        StructuredBuffer<InstanceData> _DataBuffer;
#endif
        struct appdata_t
        {
            float4 vertex : POSITION;
#if _FAKELIGHT_ON
            float3 normal : NORMAL;
#endif
#if _VERTEXCOLOR_ON
            float4 color : COLOR;
#endif
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        struct v2g
        {
            float4 vertex : SV_POSITION;
            half4 color : COLOR;
        };
        struct g2f
        {
            float4 vertex : SV_POSITION;
            float4 color : COLOR;
        };

        float4 _Color;
        float4 _DebugX_GlobalColor;      

        void setup()
        {
#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
            InstanceData data = _DataBuffer[unity_InstanceID];
            
            UNITY_MATRIX_M = data.m; //UNITY_MATRIX_M
            _Color = data.color;
#endif
        }



        
#if _DOT_ON
        float _DebugX_GlobalDotSize;      
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
#endif



        v2g vert (appdata_t v)
        {
            v2g o;
            UNITY_SETUP_INSTANCE_ID(v);

            float4x4 M = UNITY_MATRIX_M;
#if _DOT_ON
            float scaleMultiplier = GetHandleSize(mul(UNITY_MATRIX_M, float4(0, 0, 0, 1)).xyz);
            M._m00 *= scaleMultiplier;
            M._m11 *= scaleMultiplier;
            M._m22 *= scaleMultiplier;
#endif


#if _BILLBOARD_ON
            float4 worldOrigin = mul(M, float4(0, 0, 0, 1));
            float4 viewOrigin = float4(UnityObjectToViewPos(float3(0, 0, 0)), 1);
            float4 worldPos = mul(M, v.vertex);

            float4 viewPos = worldPos - worldOrigin + viewOrigin;

            float4 clipsPos = mul(UNITY_MATRIX_P, viewPos);
            o.vertex = clipsPos;
#else
            o.vertex = UnityObjectToClipPos(v.vertex); //UNITY_MATRIX_VP
#endif


            half4 color = _Color;
#if _FAKELIGHT_ON
            float3 eyeNormal = normalize (mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal).xyz);
            float nl = saturate(eyeNormal.z);
            float lighting = 0.333 + nl * 0.667 * 0.5;
            color.rgb = lighting * color.rgb;
            color = saturate(color) * _DebugX_GlobalColor;
#endif

#if _VERTEXCOLOR_ON
            color *= v.color;
#endif
            o.color = color * _DebugX_GlobalColor;
                
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
                return i.color * half4(1, 1, 1, 0.1);
            }
            ENDCG
        }
    }
}