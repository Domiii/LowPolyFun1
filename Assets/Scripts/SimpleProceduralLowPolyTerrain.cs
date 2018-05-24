using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimpleProceduralLowPolyTerrain : MonoBehaviour {
	Biome[] biomes;
	int[] vertexBiome;

	float totalHeight;

	MeshFilter meshFilter;
	Vector3[] baseVertices;
	Vector3[] baseNormals;

	float seedX, seedY;
	public PerlinNoise biomeNoise;

	public float speed = 2;

	/// <summary>
	/// The amount of levels of detail you want to see (each twice as finely scaled as the previous)
	/// </summary>
	public int seed = 23455898;

	public bool isMoving = true;
	public float time;

	private Vector3[] vs;
	private Color[] colors;


	void Awake() {
		if (biomeNoise == null) {
			biomeNoise = new PerlinNoise ();
		}
		UpdateBiomeData ();

		meshFilter = GetComponent<MeshFilter> ();

		var mesh = meshFilter.mesh;
		baseVertices = mesh.vertices;
		baseNormals = mesh.normals;

//		print (baseVertices.Min(v => v.x));
//		print (baseVertices.Max(v => v.x));

		Random.InitState (seed);
		seedX = Random.Range (1.0f, 2) * 14486;
		seedY = Random.Range (1.0f, 2) * 47940;

		time = Time.time;
	}

	void UpdateBiomeData() {
		biomes = GetComponents<Biome> ();

		totalHeight = 0;
		for (var i = 0; i < biomes.Length; ++i) {
			var h = biomes [i].height;
			totalHeight += h;
		}
	}


	void OnMeshDebug(int i0) {
		// i0 is the first vertex of the triangle that was hit
		print(i0);
	}

	void Update() {
		UpdateBiomeData ();
		ReComputeHeight ();

		if (isMoving) {
			time += Time.deltaTime;
		}
	}

	//public float min = 999999, max = -999999;

	/// <summary>
	/// Extend mesh randomly along normal
	/// </summary>
	void ReComputeHeight() {
		var mesh = meshFilter.mesh;

		if (vs == null) {
			vs = new Vector3[baseVertices.Length];
			colors = new Color[baseVertices.Length];
		}

		//var vs = mesh.vertices;

		var xOffset = speed * time + seedX;
		//var yOffset = speed * time + seedY;
		var yOffset = seedY;

		for (var i = 0; i < vs.Length; ++i) {
			var u = baseVertices[i];

			var x = u.x + xOffset;
			var z = u.z + yOffset;

			// get the biome
			var h = biomeNoise.SampleNoise(x, z, 0, 0) * totalHeight / biomeNoise.scale;
			var biomeRand = h;
			int bi = biomes.Length-1;
			var baseH = 0.0f;
			for (var b = 0; b < biomes.Length; ++b) {
				biomeRand -= biomes [b].height;
				baseH += biomes [b].height;
				if (biomeRand < 0) {
					bi = b;

					var dh = h - baseH;
					h = baseH + dh * biomes [bi].scale;
					break;
				}
			}
			//var bi = (int)Mathf.Floor (biomeRand * biomes.Length);
			var biome = biomes[bi];

			// sample height from biome

//			min = Mathf.Min (min, h);
//			max = Mathf.Max (max, h);

			// pull vertex up by normal
			var n = baseNormals [i];
			var v = vs [i];
			v = u + n * h * biomeNoise.scale;
			vs[i] = v;

			// set color
			colors[i] = biome.color;
		}

		mesh.vertices = vs;
		mesh.colors = colors;

		mesh.RecalculateNormals ();
		mesh.RecalculateTangents ();
		mesh.RecalculateBounds ();
	}

}
