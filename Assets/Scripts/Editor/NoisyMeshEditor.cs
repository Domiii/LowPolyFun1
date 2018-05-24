using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NoisyMesh))]
public class NoisyMeshEditor : Editor {
	
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();

		var obj = (NoisyMesh) target;

		if (GUILayout.Button ("Fix me")) {
			obj.FixMe ();
		}

	}
}