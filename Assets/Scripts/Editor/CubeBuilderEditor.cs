using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CubeBuilder))]
public class CubeBuilderEditor : Editor {
	public override void OnInspectorGUI ()
	{
		var b = (CubeBuilder)target;

		if (GUILayout.Button ("Build!")) {
			b.Build ();
		}

		base.OnInspectorGUI ();
	}
}
