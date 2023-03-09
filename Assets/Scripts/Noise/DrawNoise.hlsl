#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
StructuredBuffer<uint> _Hashes;
#endif

float4 _Config;

void ConfigureProcedural () {
#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
    float res = 5.0;
    float ires = 1 / 5.0;
    
    float v = floor(ires * unity_InstanceID + 0.00001);
    float u = unity_InstanceID - res * v;
    
    unity_ObjectToWorld = 0.0;
    unity_ObjectToWorld._m03_m13_m23_m33 = float4(
        0.4,
        0.0,//_Config.z * (0.05 * (_Hashes[unity_InstanceID] >> 24) - 0.5),
        0.4,
        1.0
    );
    unity_ObjectToWorld._m00_m11_m22 = _Config.y;
#endif
}

float3 GetHashColor () {
#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
    uint hash = _Hashes[unity_InstanceID];
    return (1.0 / 255.0) * float3(
            hash & 255,
            (hash >> 8) & 255,
            (hash >> 16) & 255
        );
#else
    return 1.0;
#endif
}

void ShaderGraphFunction_float (float3 In, out float3 Out, out float3 Color) {
    Out = In;
    Color = GetHashColor();
}

void ShaderGraphFunction_half (half3 In, out half3 Out, out half3 Color) {
    Out = In;
    Color = GetHashColor();
}