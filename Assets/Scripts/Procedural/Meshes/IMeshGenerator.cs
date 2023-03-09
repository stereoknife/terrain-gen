using UnityEngine;

namespace Procedural.Meshes
{
    public interface IMeshGenerator
    {
        int Resolution { get; set; }
        int VertCount { get; }
        int IdxCount { get; }
        int JobLength { get; }
        Bounds Bounds { get; }
        
        void Execute<TS>(int u, TS streams) where TS : struct, IMeshStreams;
    }
}