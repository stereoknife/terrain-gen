using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

namespace Procedural.Meshes.Generators
{
    public struct SquareGrid : IMeshGenerator
    {
        public int Resolution { get; set; }
        public int VertCount => VtxResolution * VtxResolution;
        public int IdxCount => 6 * Resolution * Resolution;
        public int JobLength => VtxResolution;
        public Bounds Bounds => new (Vector3.zero, new Vector3(1f, 0f, 1f));

        private int VtxResolution => Resolution + 1;
        
        public void Execute<TS>(int u, TS streams) where TS : struct, IMeshStreams
        {
            int vi = u * VtxResolution,
                ti = 2 * Resolution * (u - 1);

            // Init vertex data
            var v = new Vert();
            v.Normal.y = 1f;
            v.Tangent.xw = float2(1f, -1f);
            v.Position.z = (float) u / Resolution - 0.5f;

            for (int x = 0; x < VtxResolution; x++, vi++)
            {
                v.Position.x = (float) x / Resolution - 0.5f;
                v.UV0.y = (float) u / Resolution;
                streams.SetVert(vi, v);

                if (u > 0)
                {
                    streams.SetTriangle(
                        ++ti, vi + int3(-Resolution - 2, -1, -Resolution - 1)
                    );
                    streams.SetTriangle(
                        ++ti, vi + int3(-Resolution - 1, -1, 0)
                    );
                }
            }
        }
    }
}