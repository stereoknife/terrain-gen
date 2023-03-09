using Procedural.Meshes;
using Procedural.Meshes.Generators;
using Procedural.Meshes.Streams;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProcMesh : MonoBehaviour
{
    [SerializeField] private GizmoMode gizmos;
    [SerializeField] private MeshType meshType;
    [SerializeField, Range(1, 50)] private int resolution = 1;

    private Mesh mesh;
    private Vector3[] verts, normals;
    private Vector4[] tangents;

    public enum MeshType 
    {  
        TiledSquareGrid, SquareGrid, TriangleGrid, UVSphere
    };

    [System.Flags]
    public enum GizmoMode
    {
        Nothing = 0, Vertices = 1, Normals = 2, Tangents = 4
    }

    private static readonly MeshJobScheduleDelegate[] Jobs = {
        MeshJob<TiledSquareGrid, SingleStream>.ScheduleParallel,
        MeshJob<SquareGrid, SingleStream>.ScheduleParallel,
        MeshJob<TriangleGrid, SingleStream>.ScheduleParallel,
        MeshJob<UVSphere, SingleStream>.ScheduleParallel,
    };

    public void Generate()
    {
#if UNITY_EDITOR
        mesh = new Mesh();
        mesh.name = "New Mesh";
        GetComponent<MeshFilter>().mesh = mesh;
#endif
        Mesh.MeshDataArray meshDataArray = Mesh.AllocateWritableMeshData(1);
        Mesh.MeshData meshData = meshDataArray[0];

        Jobs[(int)meshType](
            mesh, meshData, resolution, default
        ).Complete();

        Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, mesh);
    }

    private void Awake()
    {
        mesh = new Mesh();
        mesh.name = "New Mesh";
        GetComponent<MeshFilter>().mesh = mesh;
        Generate();
    }

    private void OnValidate()
    {
        enabled = true;
    }

    private void Update()
    {
        Generate();
        enabled = false;
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (gizmos == GizmoMode.Nothing) return;
#else
        if (gizmos == GizmoMode.Nothing || _mesh == null) return;
#endif
        bool drawVertices = (gizmos & GizmoMode.Vertices) != 0;
        bool drawNormals = (gizmos & GizmoMode.Normals) != 0;
        bool drawTangents = (gizmos & GizmoMode.Tangents) != 0;
        
        verts = mesh.vertices;
        if (drawNormals) {
            normals = mesh.normals;
        }
        if (drawTangents) {
            tangents = mesh.tangents;
        }
        
        var t = transform;
        for (int i = 0; i < verts.Length; i++) {
            var position = t.TransformPoint(verts[i]);
            if (drawVertices) {
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(position, 0.02f);
            }
            if (drawNormals) {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(position, t.TransformDirection(normals[i]) * 0.2f);
            }
            if (drawTangents) {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(position, t.TransformDirection(tangents[i]) * 0.2f);
            }
        }
    }
}