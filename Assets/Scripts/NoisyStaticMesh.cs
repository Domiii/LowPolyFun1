using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoisyStaticMesh : MonoBehaviour {
	public float speed = 2;
	public float scale = .1f;

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

	MeshFilter meshFilter;
	Perlin perlin;
	Vector3[] baseVertices;
	Vector3[] baseNormals;

	void Awake() {
		meshFilter = GetComponent<MeshFilter> ();

		var mesh = meshFilter.mesh;
		baseVertices = mesh.vertices;
		baseNormals = mesh.normals;

		Random.InitState (seed);
		seedX = Random.Range (1.0f, 2) * 14486;
		seedY = Random.Range (1.0f, 2) * 47940;
		seedZ = Random.Range (1.0f, 2) * 98593;

		perlin = new Perlin ();

		time = Time.time;
	}

	void Update() {
		Deform3 ();

		if (isMoving) {
			time += Time.deltaTime;
		}
	}

	static float min = -.9f, max= .9f;

	/// <summary>
	/// Extend mesh along normal
	/// </summary>
	void Deform3() {
		var mesh = meshFilter.mesh;

		var vs = new Vector3[baseVertices.Length];
		//var vs = mesh.vertices;

		var offsetX = speed * time + seedX;
		var offsetY = speed * time + seedY;
		var offsetZ = speed * time + seedZ;

		for (var i = 0; i < vs.Length; ++i) {
			var u = baseVertices[i];
			var v = vs [i];

			var inX = u.x;
			var inY = u.y;
			var inZ = u.z;

			float delta = 0;
			var gain = 1.0f;
			for(var o = 0; o < octaves; o++)
			{
				delta += perlin.Noise (offsetX + inX*gain/frequency, offsetY + inY*gain/frequency, offsetZ + inZ*gain/frequency) / gain;
				gain *= 2.0f;
			}

			// re-scale, so delta will always be between 0 and 1
			min = Mathf.Min (min, delta);
			max = Mathf.Max (max, delta);
			delta = (delta - min) / (max-min);

			// randomly move along the normal
			var n = baseNormals [i];
			v = u + n * scale * delta;
			vs[i] = v;
		}

		mesh.vertices = vs;

		mesh.RecalculateNormals ();
		mesh.RecalculateTangents ();
		mesh.RecalculateBounds ();
	}

	void Deform2() {
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
			var v = baseVertices[i];

			// convert to spherical coordinates
			var r = v.magnitude;
			var theta = Mathf.Acos(v.z / r);
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
			vs[i] = v;
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
	void Deform1() {
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
			var v = baseVertices[i];

			var inX = v.x;
			var inY = v.y;
			var inZ = v.z;

			var dx = perlin.Noise (inX + noiseDx, inY + noiseDx, inZ + noiseDx);
			var dy = perlin.Noise (inX + noiseDy, inY + noiseDy, inZ + noiseDy);
			var dz = perlin.Noise (inX + noiseDz, inY + noiseDz, inZ + noiseDz);

			v.x += dx * scale;
			v.y += dy * scale;
			v.z += dz * scale;
			vs[i] = v;
		}

		mesh.vertices = vs;

		mesh.RecalculateNormals ();
		mesh.RecalculateTangents ();
		mesh.RecalculateBounds ();
	}
}
