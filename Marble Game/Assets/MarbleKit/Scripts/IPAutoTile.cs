using UnityEngine;
using System.Collections;

public class IPAutoTile : MonoBehaviour {

	[Tooltip("The size of the tiles.")]
	public float tileScale;

	private Vector3 prevScale;
	private float prevTileScale;

	/* This assumes that the texture represents one Unity unit of space, so it calculates
	 * the size of the object and sets the UV coordinates to tile accordingly. This 
	 * class does not work in the editor, but can be interchanged with IPAutoTilePreview
	 * which does work in the editor. The preview class should not be used for the actual
	 * game because it assigns the same UV scale to all objects which share the material */
	void Update () {
		if(gameObject.transform.lossyScale != prevScale || tileScale != prevTileScale) {
			float x = transform.lossyScale.x * tileScale;
			float y = transform.lossyScale.z * tileScale;
			GetComponent<Renderer>().material.mainTextureScale = new Vector2(x, y);

			prevScale = transform.lossyScale;
			prevTileScale = tileScale;
		}
	}
}
