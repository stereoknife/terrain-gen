using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ProcMesh))]
class ProcMeshEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Generate"))
        {
            (target as ProcMesh)?.Generate();
        }
    }
}