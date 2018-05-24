using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SphereBuilder))]
public class SphereBuilderEditor : Editor {
	public override void OnInspectorGUI ()
	{
		var b = (SphereBuilder)target;

		if (GUILayout.Button ("Build!")) {
			b.BuildSphere ();
		}

		base.OnInspectorGUI ();
	}
}
