using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LandscapeMesh))]
public class TranslateHelperEditor : Editor
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