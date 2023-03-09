using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

using static Unity.Mathematics.math;

namespace Noise
{
    public class HashVisualization : MonoBehaviour
    {
        [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
        struct HashJob : IJobFor {

            [WriteOnly]
            public NativeArray<uint> Hashes;

            public int seed;
            public int resolution;
            public float invResolution;
        
            public void Execute(int i) {
                int v = (int)floor(invResolution * i + 0.00001f);
                int u = i - resolution * v;

                u /= 4;
                v /= 4;
                
                Hashes[i] = SmallXXHash.Seed(seed).Eat(u).Eat(v);
            }
        }

        private static readonly int
            HashesId = Shader.PropertyToID("_Hashes"),
            ConfigId = Shader.PropertyToID("_Config");

        [SerializeField] private Mesh instanceMesh;
        [SerializeField] private Material material;
        [SerializeField, Range(1, 512)] private int resolution;
        [SerializeField, Range(-2f, 2f)] private float verticalOffset = 1f;
        [SerializeField] private int seed;

        private NativeArray<uint> hashes;

        private ComputeBuffer hashesBuffer;

        private MaterialPropertyBlock propertyBlock;
        
        void OnEnable () {
            int length = resolution * resolution;
            hashes = new NativeArray<uint>(length, Allocator.Persistent);
            hashesBuffer = new ComputeBuffer(length, 4);

            new HashJob {
                Hashes = hashes,
                resolution = resolution,
                invResolution = 1f / resolution,
                seed = seed,
            }.ScheduleParallel(hashes.Length, resolution, default).Complete();

            hashesBuffer.SetData(hashes);

            propertyBlock ??= new MaterialPropertyBlock();
            propertyBlock.SetBuffer(HashesId, hashesBuffer);
            propertyBlock.SetVector(ConfigId, new Vector4(resolution, 1f / resolution, verticalOffset / resolution));
        }
        
        void OnDisable () {
            hashes.Dispose();
            hashesBuffer.Release();
            hashesBuffer = null;
        }

        void OnValidate () {
            if (hashesBuffer != null && enabled) {
                OnDisable();
                OnEnable();
            }
        }
        
        void Update ()
        {
            Graphics.DrawMeshInstancedProcedural(
                instanceMesh, 0, material, new Bounds(Vector3.zero, Vector3.one),
                hashes.Length, propertyBlock
            );
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            /*
            for (int x = 0; x < resolution; x++)
            {
                for (int y = 0; y < resolution; y++)
                {
                    Gizmos.DrawSphere(new(
                        1f / resolution * (x + 0.5f) - 0.5f, 0, 
                        1f / resolution * (y + 0.5f) - 0.5f), 0.01f);
                }
            }
            /*/
            for (int i = 0; i < resolution * resolution; i++)
            {
                float v = floor((1f / resolution) * i + 0.00001f);
                float u = i - resolution * v;
                
                Gizmos.DrawSphere(new(
                    1f / resolution * (u + 0.5f) - 0.5f, 0, 
                    1f / resolution * (v + 0.5f) - 0.5f), 0.01f);
            }
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(new(0.4f, 0f, 0.4f), 0.01f);
            //*/
            //Gizmos.color = Color.white;
        }
    }
}
