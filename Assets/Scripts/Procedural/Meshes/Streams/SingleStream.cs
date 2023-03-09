using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace Procedural.Meshes.Streams
{
    public struct SingleStream : IMeshStreams
    {
        [StructLayout(LayoutKind.Sequential)]
        struct Stream0 {
            public float3 position, normal;
            public float4 tangent;
            public float2 uv0;
        }
        
        [NativeDisableContainerSafetyRestriction]
        NativeArray<Stream0> s0;
        [NativeDisableContainerSafetyRestriction]
        NativeArray<int3> tris;
        
        public void Setup(Mesh.MeshData data, Bounds bounds, int vertCount, int idxCount)
        {
            var descriptor = new NativeArray<VertexAttributeDescriptor>(
                4, Allocator.Temp, NativeArrayOptions.UninitializedMemory
            );
            
            descriptor[0] = new (VertexAttribute.Position, dimension: 3);
            descriptor[1] = new (VertexAttribute.Normal, dimension: 3);
            descriptor[2] = new (VertexAttribute.Tangent, dimension: 4);
            descriptor[3] = new (VertexAttribute.TexCoord0, dimension: 2);
            
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
            
            s0 = data.GetVertexData<Stream0>();
            tris = data.GetIndexData<int>().Reinterpret<int3>(sizeof(int));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVert(int idx, Vert data) => s0[idx] = new Stream0
        {
            position = data.Position,
            normal = data.Normal,
            tangent = data.Tangent,
            uv0 = data.UV0,
        };

        public void SetTriangle(int idx, int3 triangle) => tris[idx] = triangle;
    }
}