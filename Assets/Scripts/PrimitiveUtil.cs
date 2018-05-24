/**
 * Source: http://answers.unity3d.com/questions/514293/changing-a-gameobjects-primitive-mesh.html
 */

using System.Collections.Generic;
using UnityEngine;

public static class PrimitiveUtil
{
	private static Dictionary<PrimitiveType, Mesh> primitiveMeshes = new Dictionary<PrimitiveType, Mesh>();
	private static Dictionary<PrimitiveType, GameObject> primitives = new Dictionary<PrimitiveType, GameObject>();

	public static GameObject CreatePrimitive(PrimitiveType type, bool withCollider)
	{
		GameObject gameObject = new GameObject(type.ToString());

		DecoratePrimitive (gameObject, type, withCollider);

		return gameObject;
	}


	public static void DecoratePrimitive(GameObject gameObject, PrimitiveType type, bool withCollider = true)
	{
		// add renderer
		//var renderer = gameObject.AddComponent<MeshRenderer>();

		// add material (if you want to...)
		//renderer.sharedMaterial = new Material(Shader.Find("Standard"));

		var tmp = GetOrCreatePrimitive (type);

		// add mesh
		var meshFilter = gameObject.AddComponent<MeshFilter>();
		meshFilter.sharedMesh = PrimitiveUtil.primitiveMeshes[type];


		if (withCollider) {
			// add collider
			ComponentUtil.CopyComponent (tmp.GetComponent<Collider>(), gameObject);
		}

		GameObject.DestroyImmediate (tmp);
	}

	public static Mesh GetPrimitiveMesh(PrimitiveType type) {
		var tmp = GetOrCreatePrimitive (type);
		var mesh = tmp.GetComponent<MeshFilter> ().sharedMesh;

		GameObject.DestroyImmediate (tmp);
		return mesh;
	}


	private static GameObject GetOrCreatePrimitive(PrimitiveType type)
	{
		GameObject go;
		if (!PrimitiveUtil.primitives.TryGetValue(type, out go) || go == null)
		{
			go = GameObject.CreatePrimitive(type);
//			var tmp = go.transform.parent = new GameObject("temp").transform;
//			tmp.gameObject.SetActive (true);

			PrimitiveUtil.primitiveMeshes[type] = go.GetComponent<MeshFilter>().sharedMesh;
			PrimitiveUtil.primitives[type] = go;

		}
		return go;
	}
}