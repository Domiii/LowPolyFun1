using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PerlinNoise
{
	public float[] octaveScales = new float[]{ 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f };
	public float frequency = 1;
	public float scale = 1;

	[HideInInspector]
	public float height;

	float ComputeHeight ()
	{
		return octaveScales.Max () * scale * 2;
	}

	public void RandomizeOctaves ()
	{
		octaveScales = new float[Random.Range (2, 8)];		// between 2 and 8 octaves
		octaveScales = octaveScales.Select (o => Random.Range (0.0f, 5)).ToArray ();

		height = ComputeHeight ();
	}

	//public float min = 1, max= 0;


	public float val;

	public float SampleNoise (float x, float y, float x0 = 0, float y0 = 0)
	{
		val = 0;
		var maxScale = 0.0f;
		var gain = 1.0f;
		for (var o = 0; o < octaveScales.Length; o++) {
			var s = octaveScales [o] / gain;
			val += s * Mathf.Clamp01 (Mathf.PerlinNoise (x0 + x * gain * frequency, y0 + y * gain * frequency));
			maxScale += s;
			gain *= 2.0f;
		}

		// re-scale, so delta will (mostly) be between 0 and 1
//		val = (val - min) / (max-min);
		val = val / maxScale;

//		min = Mathf.Min (min, val);
//		max = Mathf.Max (max, val);

		return val * scale;
	}
}
