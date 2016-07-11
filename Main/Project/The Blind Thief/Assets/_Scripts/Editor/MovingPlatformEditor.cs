using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(MovingPlatformBehaviour))]
[CanEditMultipleObjects]
public class MovingPlatformEditor : Editor
{
	SerializedProperty _firstPosition;
	SerializedProperty _secondPosition;

	void OnEnable()
	{
		//Setup Serialized Properties
		_firstPosition = serializedObject.FindProperty("firstPosition");
		_secondPosition = serializedObject.FindProperty("secondPosition");
	
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		DrawDefaultInspector();

		MovingPlatformBehaviour platformScript = (MovingPlatformBehaviour)target;

		GameObject _platform = platformScript.gameObject;



		if(GUILayout.Button("Set First Position"))
		{
			Debug.Log(_platform.name); 
			_firstPosition.vector3Value = _platform.transform.position;
			platformScript.SetFirstPosition(_firstPosition.vector3Value);
		}

		if(GUILayout.Button("Set Second Position"))
		{
			_secondPosition.vector3Value = platformScript.transform.position;
			platformScript.SetSecondPosition(_secondPosition.vector3Value);
		}

		if(GUILayout.Button("Set To First Position"))
		{
			platformScript.SetToFirstPosition();
		}

		serializedObject.ApplyModifiedProperties();

	}
}
