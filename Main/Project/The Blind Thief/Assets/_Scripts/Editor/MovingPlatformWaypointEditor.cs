using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(MovingPlatformWaypoints))]
[CanEditMultipleObjects]
public class MovingPlatformWaypointEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        MovingPlatformWaypoints thisScript = (MovingPlatformWaypoints) target;

        DrawDefaultInspector();

        if (GUILayout.Button("Set To Waypoints"))
        {
            thisScript.SetStartAndEndPoint();
        }

        if (GUILayout.Button("Move Behind"))
        {
            thisScript.SetToBehind();
        }
        if (GUILayout.Button("Re Align line"))
        {
            thisScript.RealignLine();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
