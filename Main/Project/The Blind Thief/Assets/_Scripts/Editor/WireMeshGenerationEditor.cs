using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(WireMeshGeneration))]
public class WireMeshGenerationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        WireMeshGeneration myScript = (WireMeshGeneration)target;

        if (GUILayout.Button("CreateMesh"))
        {
            myScript.CreateWire();
        }
    }
}
