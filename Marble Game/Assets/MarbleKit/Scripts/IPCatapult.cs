using UnityEngine;
using System.Collections;

public class IPCatapult : IPSwitchableObject {

	[Tooltip("The upward force exerted on the player by the catapult.")]
	public float upForce;
	[Tooltip("The forward force exerted on the player by the catapult.")]
	public float forwardForce;

	private bool launching = false;

	// Checkpoint variables
	private float savedUpForce;
	private float savedForwardForce;

	/* Because we inherit from IPGameObject, the class that handles checkpoint objects,
	 * we need to override that class's Start method and call the base class Start 
	 * method, as well as our IPCatapult specific checkpoint save method. */
	public override void Start () {
		base.Start();
		SaveCheckpointState();
	}

	void OnTriggerEnter(Collider other) {
		if (MKSceneManager.instance.inputLocked || !activated)
			return;

		// If the player enters the trigger, and the catapult is not already launching
		// then we activate the launch.
		if (other.gameObject.name.Contains("IPPlayer") && !launching) {
			launching = true;

			// This gets the relevant vectors from the direction that the catapult is facing
			Vector3 upVector = transform.TransformDirection(Vector3.up);
			Vector3 forwardVector = transform.TransformDirection(Vector3.forward);

			// Start the animation and add the forces to the player
			StartCoroutine("LaunchCatapult");
			other.attachedRigidbody.AddForce(upVector * upForce, ForceMode.Acceleration);
			other.attachedRigidbody.AddForce(forwardVector * forwardForce, ForceMode.Acceleration);
		}
	}

	IEnumerator LaunchCatapult() {
		GetComponent<AudioSource>().Play();

		Vector3 startRot = new Vector3(0.0f, 0.0f, 0.0f);
		Vector3 endRot = new Vector3(45.0f, 0.0f, 0.0f);

		float duration = 0.125f;
		float timer = 0.0f;

		// The pivot point of the top part of the catapult should be set to the location
		// of the hinge. This rotates it 45 degrees around that pivot point
		while (transform.localEulerAngles.x < 45.0f) {		
			timer += Time.deltaTime;
			transform.localEulerAngles = Vector3.Lerp (startRot,endRot,timer/duration);
			yield return null;
		}			

		// Bring the top of the catapult back down to its original position
		timer = 0.0f;
		while (transform.localEulerAngles.x > 0.0f) {		
			timer += Time.deltaTime;
			transform.localEulerAngles = Vector3.Lerp (endRot,startRot,timer/duration);
			yield return null;
		}

		// Make sure it is at 0 and reset the launch variable so it is ready to go again
		transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, transform.localEulerAngles.z);
		launching = false;
	}

	/* Both of the below methods are also overrides from IPGameObject. In addition to the 
	 * base class methods, we save and restore variables specific to this class. */
	public override void SaveCheckpointState () {
		base.SaveCheckpointState();

		savedUpForce = upForce;
		savedForwardForce = forwardForce;
	}
	
	public override void RestoreCheckpointState () {
		base.RestoreCheckpointState();

		upForce = savedUpForce;
		forwardForce = savedForwardForce;
	}
}
