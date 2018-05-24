using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class CubeBuilder : MonoBehaviour {
	static readonly float InversePI = 1/Mathf.PI;

	public float detailLevel = 3;

	static Vector2 V3toUV(Vector3 p)
	{
		var d = p.normalized;
		var u = Mathf.Atan2(-d.z, -d.x) * InversePI * 0.5f + 0.5f;
		var v = 0.5f - Mathf.Asin(-d.y) * InversePI;
		return new Vector2(u, v);
	}

	void FlipNormals(List<int> triangles, Vector3[] normals) {
		for (int i=0;i<normals.Length;i++)
			normals[i] = -normals[i];
	}

	void FlipWinding(List<int> triangles) {

		for (int i=0;i<triangles.Count;i+=3)
		{
			int temp = triangles[i + 0];
			triangles[i + 0] = triangles[i + 1];
			triangles[i + 1] = temp;
		}
	}

	public void Build() {
		var renderer = GetComponent<MeshRenderer> ();
		var filter = GetComponent<MeshFilter> ();


		var cubeMesh = PrimitiveUtil.GetPrimitiveMesh (PrimitiveType.Cube);

		var vertexList = new List<Vector3>(cubeMesh.vertices);
		var indexList = new List<int>(cubeMesh.triangles);

		for (var i = 0; i < detailLevel; i++) {
			GeometryUtil.Subdivide (vertexList, indexList, true);
		}

		// compute UV coords
		var uvList = new List<Vector2> ();
		for (var i = 0; i < vertexList.Count; i++) {
			//var v = vertexList [i] = Vector3.Normalize (vertexList [i]);
			uvList.Add (V3toUV(vertexList[i]));
		}

		var vertices = vertexList.ToArray ();

		//FlipWinding (indexList);

		var mesh = filter.mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.uv = uvList.ToArray();
		mesh.SetTriangles(indexList, 0, true);

		mesh.RecalculateNormals ();
		mesh.RecalculateTangents ();

	}
}