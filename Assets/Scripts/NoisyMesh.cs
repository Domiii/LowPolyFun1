using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class NoisyMesh : MonoBehaviour
{
	Biome[] biomes;
	public float speed = 2;
	public float scale = .1f;
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
	Perlin perlin;
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

		perlin = new Perlin ();

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
		Deform3 ();

		if (isMoving) {
			time += Time.deltaTime;
		}
	}

	float min = 100, max = -100;

	/// <summary>
	/// Extend mesh along normal
	/// </summary>
	void Deform3 ()
	{
		var mesh = meshFilter.sharedMesh;

		var vs = new Vector3[baseVertices.Length];
		var colors = new Color[baseVertices.Length];
		//var vs = mesh.vertices;

		var offsetX = speed * time + seedX;
		var offsetY = speed * time + seedY;
		var offsetZ = speed * time + seedZ;

		for (var i = 0; i < vs.Length; ++i) {
			var u = baseVertices [i];
			var inX = u.x;
			var inY = u.y;
			var inZ = u.z;

			float h = 0;
			var gain = 1.0f;
			for (var o = 0; o < octaves; o++) {
				h += perlin.Noise (offsetX + inX * gain / frequency, offsetY + inY * gain / frequency, offsetZ + inZ * gain / frequency) / gain;
				gain *= 2.0f;
			}

			// re-scale, so delta will always be between 0 and 1
			min = Mathf.Min (min, h);
			max = Mathf.Max (max, h);
		}

		for (var i = 0; i < vs.Length; ++i) {
			var u = baseVertices [i];
			var v = vs [i];

			var inX = u.x;
			var inY = u.y;
			var inZ = u.z;

			float h = 0;
			var gain = 1.0f;
			for (var o = 0; o < octaves; o++) {
				h += perlin.Noise (offsetX + inX * gain / frequency, offsetY + inY * gain / frequency, offsetZ + inZ * gain / frequency) / gain;
				gain *= 2.0f;
			}

			// re-scale, so delta will always be between 0 and 1
			h = (h - min) / (max - min);


//			if (h < 0.8f) {
//				//h = h-.1f;
//				h = 1f;
//			} else if (h < 0.4f) {
//				h = 0;
//			} else {
//				h = -1f;
//			}


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

		mesh.RecalculateNormals ();
		mesh.RecalculateTangents ();
		mesh.RecalculateBounds ();
	}
	#region Old Deform
	void Deform2 ()
	{
		var mesh = meshFilter.mesh;
		//var vs = mesh.vertices;

		if (baseVertices == null) {
			baseVertices = mesh.vertices;
		}

		var vs = new Vector3[baseVertices.Length];
		//var vs = mesh.vertices;


		var noiseDx = time * speed + seedX;
		var noiseDy = time * speed + seedY;
		var noiseDz = time * speed + seedZ;


		for (var i = 0; i < vs.Length; ++i) {
			var v = baseVertices [i];

			// convert to spherical coordinates
			var r = v.magnitude;
			var theta = Mathf.Acos (v.z / r);
			//var phi = Mathf.Atan (v.y / (v.x + .1f));

			var inX = r;
			var inY = theta;
			var inZ = Mathf.PI;

			var dx = perlin.Noise (inX + noiseDx, inY + noiseDx, inZ + noiseDx);
			var dy = perlin.Noise (inX + noiseDy, inY + noiseDy, inZ + noiseDy);
			var dz = perlin.Noise (inX + noiseDz, inY + noiseDz, inZ + noiseDz);

			v.x += dx * scale;
			v.y += dy * scale;
			v.z += dz * scale;
			vs [i] = v;
		}

		mesh.vertices = vs;

		mesh.RecalculateNormals ();
		mesh.RecalculateTangents ();
		mesh.RecalculateBounds ();
	}


	/// <summary>
	/// The overall direction obviously is following the opposite of the positive x,y,z direction.
	/// There is some odd correlation and regularity to this method: The same pattern repeats all the time.
	/// For speed = 1, the pattern can be observed to repeat for every multiple of 1.
	/// For speed = .6, the pattern can be observed to repeat for every multiple of 5.
	/// For speed = .8, the pattern can be observed to repeat for every multiple of 4.
	/// </summary>
	void Deform1 ()
	{
		var mesh = meshFilter.mesh;
		//var vs = mesh.vertices;

		if (baseVertices == null) {
			baseVertices = mesh.vertices;
		}

		//var vs = new Vector3[baseVertices.Length];
		var vs = mesh.vertices;

		var noiseDx = time * speed + seedX;
		var noiseDy = time * speed + seedY;
		var noiseDz = time * speed + seedZ;


		for (var i = 0; i < vs.Length; ++i) {
			var v = baseVertices [i];

			var inX = v.x;
			var inY = v.y;
			var inZ = v.z;

			var dx = perlin.Noise (inX + noiseDx, inY + noiseDx, inZ + noiseDx);
			var dy = perlin.Noise (inX + noiseDy, inY + noiseDy, inZ + noiseDy);
			var dz = perlin.Noise (inX + noiseDz, inY + noiseDz, inZ + noiseDz);

			v.x += dx * scale;
			v.y += dy * scale;
			v.z += dz * scale;
			vs [i] = v;
		}

		mesh.vertices = vs;

		mesh.RecalculateNormals ();
		mesh.RecalculateTangents ();
		mesh.RecalculateBounds ();
	}
	#endregion
}
