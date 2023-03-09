using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

namespace Procedural.Meshes.Generators
{
    public struct TriangleGrid : IMeshGenerator
    {
        public int Resolution { get; set; }
        public int VertCount => VtxResolution * VtxResolution;
        public int IdxCount => 6 * Resolution * Resolution;
        public int JobLength => VtxResolution;
        public Bounds Bounds => new (Vector3.zero, 
            new Vector3(1f + 0.5f / Resolution, 0f, sqrt(3) / 2f));

        private int VtxResolution => Resolution + 1;
        
        public void Execute<TS>(int u, TS streams) where TS : struct, IMeshStreams
        {
            int vi = u * VtxResolution,
                ti = 2 * Resolution * (u - 1);
            
            int iSW = -Resolution - 2, iSe = -Resolution - 1, iNw = -1, iNe = 0;
            var tA = int3(iSW, iNw, iNe);
            var tB = int3(iSW, iNe, iSe);

            // Init vertex data
            var v = new Vert();
            v.Normal.y = 1f;
            v.Tangent.xw = float2(1f, -1f);
            v.Position.z = (float) u / Resolution - 0.5f * sqrt(3) / 2f;

            for (int x = 0; x < VtxResolution; x++, vi++)
            {
                float xoff = -0.25f, uoff = 0f;
                if (Odd(u))
                {
                    xoff = 0.25f;
                    uoff = 0.5f / (Resolution + 0.5f);
                    tA = int3(iSW, iNw, iSe);
                    tB = int3(iSe, iNw, iNe);
                }
                xoff = xoff / Resolution - 0.5f;
                
                v.Position.x = (float) x / Resolution - xoff;
                v.UV0.x = x / (Resolution + 0.5f) + uoff;
                v.UV0.y = v.Position.z / (1f + 0.5f / Resolution) + 0.5f;
                streams.SetVert(vi, v);

                if (u > 0)
                {
                    streams.SetTriangle(
                        ++ti, vi + tA
                    );
                    streams.SetTriangle(
                        ++ti, vi + tB
                    );
                }
            }
        }

        bool Odd(int i) => (i & 1) == 1;
    }
}