using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Procedural.Meshes
{
    public delegate JobHandle MeshJobScheduleDelegate (
        Mesh mesh, Mesh.MeshData meshData, int resolution, JobHandle dependency
    );
    
    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
    public struct MeshJob<TG, TS> : IJobFor
        where TG : struct, IMeshGenerator
        where TS : struct, IMeshStreams
    {
        [WriteOnly]
        private TS streams;
        private TG generator;
        public void Execute(int i) => generator.Execute(i, streams);
        
        public static JobHandle ScheduleParallel (
            Mesh mesh, Mesh.MeshData meshData, int resolution, JobHandle dependency
        ) {
            var job = new MeshJob<TG, TS>();
            job.generator.Resolution = resolution;
            mesh.bounds = job.generator.Bounds;
            job.streams.Setup(
                meshData, mesh.bounds, job.generator.VertCount, job.generator.IdxCount
            );
            return job.ScheduleParallel(job.generator.JobLength, 1, dependency);
        }
    }
}