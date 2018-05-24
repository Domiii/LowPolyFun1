using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A Biome provides a unique stylization of Terrain
/// </summary>
public class Biome : MonoBehaviour
{
	public Color color;
	public float height = 1;

	/// <summary>
	/// Variation of height within the biome is multipled by this scale
	/// </summary>
	public float scale = .3f;

	void Reset() {
		Randomize ();
	}

	public void Randomize ()
	{
		color = new Color (Random.Range (0.0f, 1), Random.Range (0.0f, 1), Random.Range (0.0f, 1));

		#if UNITY_EDITOR
		UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
		#endif
	}
}
