using Unity.Mathematics;
using UnityEngine;

namespace Procedural.Meshes
{
    public interface IMeshStreams
    {
        void Setup(UnityEngine.Mesh.MeshData data, Bounds bounds, int vertCount, int idxCount);
        void SetVert(int idx, Vert data);
        void SetTriangle(int idx, int3 triangle);
    }
}