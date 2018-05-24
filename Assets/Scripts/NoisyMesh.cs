using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class NoisyMesh : MonoBehaviour
{
	Biome[] biomes;
	public float speed = 2;
	public bool dontUpdate;

	/// <summary>
	/// The higher, the less detail you will see, unless you have a lot of octaves.
	/// </summary>
	public float frequency = 1;

	/// <summary>
	/// The amount of levels of detail you want to see (each twice as finely scaled as the previous)
	/// </summary>
	public int octaves = 8;
	public int seed = 23455898;

	public bool isMoving = true;
	public float time;

	float seedX, seedY, seedZ;
	float totalHeight;

	MeshFilter meshFilter;
	Mesh originalMesh;
	PerlinNoise noise;
	Vector3[] baseVertices;
	Vector3[] baseNormals;

	#if UNITY_EDITOR
	[UnityEditor.Callbacks.DidReloadScripts]
	static void OnScriptsReloaded ()
	{
		var os = GameObject.FindObjectsOfType<NoisyMesh> ();

		foreach (var o in os) {
			o.FixMe ();
		}
	}
	#endif

	void Awake ()
	{
		meshFilter = GetComponent<MeshFilter> ();

		var mesh = meshFilter.sharedMesh;
		baseVertices = mesh.vertices;
		baseNormals = mesh.normals;

		Random.InitState (seed);
		seedX = Random.Range (1.0f, 2) * 14486;
		seedY = Random.Range (1.0f, 2) * 47940;
		seedZ = Random.Range (1.0f, 2) * 98593;

		noise = new PerlinNoise() { 
			frequency = frequency 
		};

		//time = Time.time;
		time = 0;
	}

	public void FixMe ()
	{
		print ("FixMe!");

		dontUpdate = true;
		enabled = false;
		isMoving = false;
		var builder = GetComponent<SphereBuilder> ();
		if (builder) {
			builder.BuildSphere ();
		}
		Awake ();
		dontUpdate = false;
		enabled = true;
	}

	void UpdateBiomeData ()
	{
		biomes = GetComponents<Biome> ();

		totalHeight = 0;
		for (var i = 0; i < biomes.Length; ++i) {
			var h = biomes [i].height;
			totalHeight += h;
		}
	}

	void Update ()
	{
		if (dontUpdate) {
			return;
		}

		UpdateBiomeData ();
		Deform ();

		if (isMoving) {
			time += Time.deltaTime;
		}
	}

	float min, max;

	/// <summary>
	/// Extend mesh along normal
	/// </summary>
	void Deform ()
	{
		var mesh = meshFilter.sharedMesh;

		var vs = new Vector3[baseVertices.Length];
		var colors = new Color[baseVertices.Length];
		//var vs = mesh.vertices;

		var offsetX = speed * time + seedX;
		var offsetY = speed * time + seedY;
		var offsetZ = speed * time + seedZ;

		// compute min and max height
		float min = 100, max = -100;
		for (var i = 0; i < vs.Length; ++i) {
			var u = baseVertices [i];
			var inX = u.x;
			var inY = u.y;
			var inZ = u.z;

			float h = noise.SampleNoise(inX, inY, inZ, offsetX, offsetY, offsetZ);

			min = Mathf.Min (min, h);
			max = Mathf.Max (max, h);
		}

		// compute actual height
		for (var i = 0; i < vs.Length; ++i) {
			var u = baseVertices [i];
			var v = vs [i];

			var inX = u.x;
			var inY = u.y;
			var inZ = u.z;

			float h = noise.SampleNoise(inX, inY, inZ, offsetX, offsetY, offsetZ);

			// re-scale, so delta will always be between 0 and 1
			h = (h - min) / (max - min);


			int bi = biomes.Length - 1;
			var realH = 0.0f;
			for (var b = 0; b < biomes.Length; ++b) {
				h -= biomes [b].height;
				realH += biomes [b].scale;
				if (h < 0) {
					bi = b;

					//var dh = h - baseH;
					//h = baseH; //+ biomes [bi].scale;
					break;
				}
			}
			var biome = biomes [bi];
			colors [i] = biome.color;


			// randomly move along the normal
			var n = baseNormals [i];
			v = u + realH * n;

			vs [i] = v;
		}

		mesh.vertices = vs;
		mesh.colors = colors;

		//mesh.RecalculateNormals ();
		NormalSolver.RecalculateNormals(mesh, 0);
		mesh.RecalculateTangents ();
		mesh.RecalculateBounds ();
	}
}
