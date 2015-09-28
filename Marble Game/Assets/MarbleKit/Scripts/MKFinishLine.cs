using UnityEngine;
using System.Collections;

public class MKFinishLine : MonoBehaviour {

	private bool finished = false;

	void OnTriggerEnter(Collider other) {
		// When the player crosses the finish line, activate the levelcleared
		// method of the scene manager. The boolean, finished, ensures that it
		// only gets activated once
		if (other.gameObject.name.Contains("IPPlayer") && !finished) {
			finished = true;
			GetComponent<AudioSource>().Play();
			MKSceneManager.instance.HandleLevelCleared();
		}
	}
}
