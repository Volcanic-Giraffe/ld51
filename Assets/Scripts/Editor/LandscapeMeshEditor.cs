using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LandscapeMesh))]
public class LandscapeMeshEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Generate"))
        {
            (target as LandscapeMesh).Generate();
        }
    }
}