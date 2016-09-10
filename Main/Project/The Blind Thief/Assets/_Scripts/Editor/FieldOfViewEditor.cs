using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{
    void OnSceneGUI()
    {
        FieldOfView fow = (FieldOfView)target;

        Handles.color = Color.white;
        Handles.DrawWireArc(fow.transform.position, Vector3.forward, Vector3.up, 360, fow.MaxViewRadius);

        Vector3 viewAngleA = fow.DirFromAngle(-fow.viewAngle / 2, false);
        Vector3 viewAngleB = fow.DirFromAngle(fow.viewAngle / 2, false);

        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * fow.MaxViewRadius);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.MaxViewRadius);

        Handles.color = Color.red;

        foreach(Transform visibleTargets in fow.visibleTargets)
        {
            Handles.DrawLine(fow.transform.position, visibleTargets.position);
        }
    }
	
}
