using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

namespace Procedural.Meshes.Generators
{
    public struct TiledSquareGrid : IMeshGenerator
    {
        public int Resolution { get; set; }
        public int VertCount => 4 * Resolution * Resolution;
        public int IdxCount => 6 * Resolution * Resolution;
        public int JobLength => Resolution;
        public Bounds Bounds => new (Vector3.zero, new Vector3(1f, 0f, 1f));
        
        public void Execute<TS>(int u, TS streams) where TS : struct, IMeshStreams
        {
            int vi = 4 * Resolution * u,
                ti = 2 * Resolution * u;

            for (int x = 0; x < Resolution; x++, vi += 4, ti += 2)
            {
                var xCoords = new float2(x, x + 1f) / Resolution - 0.5f;
                var zCoords = new float2(u, u + 1f) / Resolution - 0.5f;
            
                var vertex = new Vert();
                vertex.Normal.y = 1f;
                vertex.Tangent.xw = float2(1f, -1f);

                vertex.Position.x = xCoords.x;
                vertex.Position.z = zCoords.x;
                streams.SetVert(vi + 0, vertex);
            
                vertex.Position.x = xCoords.y;
                vertex.Position.z = zCoords.x;
                vertex.UV0 = float2(1f, 0f);
                streams.SetVert(vi + 1, vertex);

                vertex.Position.x = xCoords.x;
                vertex.Position.z = zCoords.y;
                vertex.UV0 = float2(0f, 1f);
                streams.SetVert(vi + 2, vertex);

                vertex.Position.x = xCoords.y;
                vertex.Position.z = zCoords.y;
                vertex.UV0 = 1f;
                streams.SetVert(vi + 3, vertex);
            
                streams.SetTriangle(ti + 0, vi + int3(0, 2, 1));
                streams.SetTriangle(ti + 1, vi + int3(1, 2, 3));
            }
        }
    }
}