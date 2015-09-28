using UnityEngine;
using System.Collections;

public class MKCheckPoint : MKSpawnPoint {

	public ParticleSystem particles1;
	public ParticleSystem particles2;

	private bool passed;

	void Start () {
		// If this is the starting checkpoint, then the player activates it on start
		if (MKSceneManager.instance.spawnPoints.IndexOf(gameObject) == 0)
			passed = true;
	}
	
	void Update () {
		// Activate the particle systems if the player has already passed the checkpoint
		// and they are not already active
		if (passed && particles1 != null && !particles1.isPlaying) {
			particles1.Play();
			if (particles2 != null)
				particles2.Play();
		}
	}

	void OnTriggerEnter(Collider other) {
		// If the player hasn't already passed the checkpoint when he enters the 
		// trigger, mark it as passed.
		if (other.gameObject.name.Contains("IPPlayer") && !passed) {
			passed = true;
			GetComponent<AudioSource>().Play();
			MKSceneManager.instance.HandleCheckpointPass(gameObject);
		}
	}
}
