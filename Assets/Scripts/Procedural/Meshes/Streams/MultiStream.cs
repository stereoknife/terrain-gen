using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace Procedural.Meshes.Streams
{
    public struct MultiStream : IMeshStreams
    {
        [NativeDisableContainerSafetyRestriction]
        NativeArray<float3> s0, s1;
        [NativeDisableContainerSafetyRestriction]
        NativeArray<float4> s2;
        [NativeDisableContainerSafetyRestriction]
        NativeArray<float2> s3;
        [NativeDisableContainerSafetyRestriction]
        NativeArray<int3> tris;
        
        public void Setup(UnityEngine.Mesh.MeshData data, Bounds bounds, int vertCount, int idxCount)
        {
            var descriptor = new NativeArray<VertexAttributeDescriptor>(
                4, Allocator.Temp, NativeArrayOptions.UninitializedMemory
            );
            
            descriptor[0] = new (VertexAttribute.Position, dimension: 3, stream: 0);
            descriptor[1] = new (VertexAttribute.Normal, dimension: 3, stream: 1);
            descriptor[2] = new (VertexAttribute.Tangent, dimension: 4, stream: 2);
            descriptor[3] = new (VertexAttribute.TexCoord0, dimension: 2, stream: 3);
            
            data.SetVertexBufferParams(vertCount, descriptor);
            descriptor.Dispose();

            data.SetIndexBufferParams(idxCount, IndexFormat.UInt32);
			
            data.subMeshCount = 1;
            data.SetSubMesh(
                0, new SubMeshDescriptor(0, idxCount)
                {
                    bounds = bounds,
                    vertexCount = vertCount,
                },
                MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices);
            
            s0 = data.GetVertexData<float3>(0);
            s1 = data.GetVertexData<float3>(1);
            s2 = data.GetVertexData<float4>(2);
            s3 = data.GetVertexData<float2>(3);
            tris = data.GetIndexData<int>().Reinterpret<int3>(4);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVert(int idx, Vert data)
        {
            s0[idx] = data.Position;
            s1[idx] = data.Normal;
            s2[idx] = data.Tangent;
            s3[idx] = data.UV0;
        }

        public void SetTriangle(int idx, int3 triangle) =>
            tris[idx] = triangle;
    }
}