using UnityEngine;
using System.Collections;

public class MKGround : MonoBehaviour {
	
	void OnCollisionEnter(Collision other) {
		// If the player collides with the ground, call its Kill function to
		// decrease its lives and check for the game over state
		if (other.gameObject.name.Contains("IPPlayer")) {
			MKPlayer player = other.gameObject.GetComponent<MKPlayer>();
			player.Kill();
		}
	}
}
