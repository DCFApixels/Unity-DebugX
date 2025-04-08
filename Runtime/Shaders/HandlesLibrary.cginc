


float GetHandleSize(float3 objectPosition, float scaleM)
{
    float3 viewDir = normalize(_WorldSpaceCameraPos - objectPosition);

    float distance = length(_WorldSpaceCameraPos - objectPosition);
    float isOrthographic = UNITY_MATRIX_P[3][3];
    distance = lerp(distance, 1, isOrthographic);
            
    float fov = radians(UNITY_MATRIX_P[1][1] * 2.0);
    float scale = distance * (1 / fov) * 0.015;

    return scale * scaleM;
}

float4x4 GetHandleSizeMatrix(float3 objectPosition, float scaleM)
{
    float4x4 M = UNITY_MATRIX_M;
    
    float scale = GetHandleSize(objectPosition, scaleM);
    M._m00 *= scale;
    M._m11 *= scale;
    M._m22 *= scale;
    
    return M;
}



//Macros
#if _DOT_ON
float _DebugX_GlobalDotSize;
#endif
float4 _Color;

float4x4 GET_HANDLE_UNITY_MATRIX_M()
{
#if _DOT_ON
    return GetHandleSizeMatrix(mul(UNITY_MATRIX_M, float4(0, 0, 0, 1)).xyz, _DebugX_GlobalDotSize);
#else
    return UNITY_MATRIX_M;
#endif
}


