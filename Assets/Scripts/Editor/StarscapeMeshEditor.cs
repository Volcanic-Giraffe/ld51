using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StarscapeMesh))]
public class StarscapeMeshEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Generate"))
        {
            (target as StarscapeMesh).Generate();
        }
    }
}