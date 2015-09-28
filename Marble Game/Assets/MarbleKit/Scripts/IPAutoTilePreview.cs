using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class IPAutoTilePreview : MonoBehaviour {

	[Tooltip("The size of the tiles.")]
	public float tileScale;

	/* This assumes that the texture represents one Unity unit of space, so it calculates
	 * the size of the object and sets the UV coordinates to tile accordingly. This 
	 * class is designed to work in the editor, but will assign the same UV scale to all
	 * objects that share the material. For that reason it should only be used for testing.
	 * IPAutoTilePreview is interchangeable with IPAutoTile. The latter should be switched
	 * to for actual game use. */
	void Update () {
		float x = transform.lossyScale.x * tileScale;
		float y = transform.lossyScale.z * tileScale;
		GetComponent<Renderer>().sharedMaterial.mainTextureScale = new Vector2(x, y);
	}
}
