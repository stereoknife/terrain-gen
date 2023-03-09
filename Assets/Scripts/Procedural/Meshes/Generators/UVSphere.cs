using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

namespace Procedural.Meshes.Generators
{
    public struct UVSphere : IMeshGenerator
    {
        public int Resolution { get; set; }
        public int VertCount => (UResolution + 1) * (VResolution + 1);
        public int IdxCount => 6 * UResolution * VResolution;
        public int JobLength => UResolution + 1;
        public Bounds Bounds => new (Vector3.zero, new Vector3(2f, 2f, 2f));

        private int VResolution => Resolution * 2;
        private int UResolution => Resolution * 4;
        
        public void Execute<TS>(int u, TS streams) where TS : struct, IMeshStreams
        {
            int vi = u * (VResolution + 1),
                ti = 2 * VResolution * (u - 1);

            // Init vertex data
            var vert = new Vert();
            vert.Position.y = vert.Normal.y = -1f;
            vert.Tangent.w = -1f;

            float2 circle;
            circle.x = sin(2f * PI * u / UResolution);
            circle.y = cos(2f * PI * u / UResolution);

            vert.Tangent.xz = circle.yx;
            circle.y = -circle.y;
            
            //vert.position.x = sin(2f * PI * u / Resolution);
            //vert.position.z = -cos(2f * PI * u / Resolution);
            vert.UV0.x = (float) u / UResolution;
            
            
            vert.UV0.y = 0f;
            streams.SetVert(vi, vert);
            vi++;
            
            for (int v = 1; v <= VResolution; v++, vi++, ti += 2)
            {
                float rad = sin(PI * v / VResolution);
                vert.Position.xz = circle * rad;
                vert.Position.y = -cos(PI * v / VResolution);
                vert.Normal = vert.Position;
                vert.UV0.y = (float) v / VResolution;
                streams.SetVert(vi, vert);

                if (u > 0)
                {
                    streams.SetTriangle(
                        ti + 0, vi + int3(-VResolution - 2, -VResolution - 1, -1)
                    );
                    streams.SetTriangle(
                        ti + 1, vi + int3(-1, -VResolution - 1, 0)
                    );
                }
            }
        }
    }
}