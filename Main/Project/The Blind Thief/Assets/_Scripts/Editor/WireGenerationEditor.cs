using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(WireGeneration))]
[CanEditMultipleObjects]
public class WireGenerationEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        WireGeneration myScript = (WireGeneration)target;

        if(GUILayout.Button("Create Wires"))
        {
            myScript.CreateWires();
        }
    }
}
