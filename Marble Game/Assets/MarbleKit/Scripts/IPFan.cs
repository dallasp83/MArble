using UnityEngine;
using System.Collections;

public class IPFan : IPSwitchableObject {

	public GameObject blades;
	public IPForceTrigger forceTrigger;

	private float bladeSpeed;
	private float bladeSlowSpeed = 300.0f;

	/* Because we inherit from IPGameObject, the class that handles checkpoint objects,
	 * we need to override that class's Start method and call the base class Start 
	 * method, as well as our IPFan specific checkpoint save method. */
	public override void Start () {
		base.Start();
	}
	
	void Update () {
		// set the force trigger that pushes the player to enabled if the fan is activated
		// and disabled otherwise.
		forceTrigger.enabled = activated;

		// If the fan is activated, make sure the sound is playing and the blade rotation speed
		// is non zero. Else, turn off the sound and gradually slow the blades to zero.
		if (activated) {
			if (!GetComponent<AudioSource>().isPlaying)
				GetComponent<AudioSource>().Play();
			bladeSpeed = 850.0f;
		} else {
			if (bladeSpeed > 0)
				bladeSpeed -= bladeSlowSpeed * Time.deltaTime;
			if (GetComponent<AudioSource>().isPlaying)
			    GetComponent<AudioSource>().Stop();
			bladeSpeed = Mathf.Clamp(bladeSpeed, 0.0f, 850.0f);
		}

		blades.transform.Rotate(new Vector3(0, 0, 1), bladeSpeed * Time.deltaTime);
	}

	// Both of the below methods are also overrides from IPGameObject.
	public override void SaveCheckpointState () {
		base.SaveCheckpointState();
	}
	
	public override void RestoreCheckpointState () {
		base.RestoreCheckpointState();
	}
}
