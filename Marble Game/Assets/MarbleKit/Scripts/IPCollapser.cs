using UnityEngine;
using System.Collections;

public class IPCollapser : IPSwitchableObject {

	[Tooltip("The delay in seconds before the object falls after it is touched.")]
	public float fallDelay;
	[Tooltip("The angle that the object rocks back and forth at before falling.")]
	public float rockAngle;
	[Tooltip("The time in seconds between rocking motions.")]
	public float rockTime;

	private float savedFallDelay;
	private float savedRockAngle;
	private float savedRockTime;

	private bool falling;

	/* Because we inherit from IPGameObject, the class that handles checkpoint objects,
	 * we need to override that class's Start method and call the base class Start 
	 * method, as well as our IPCollapser specific checkpoint save method. */
	public override void Start () {
		base.Start();
		SaveCheckpointState();
	}

	// While the collapser is not touched, it is kinematic. Upon being touched and shaken
	// it switches to non-kinematic. This sets it back after it has fallen below the floor
	// threshold. Note that this only matters if there is no ground for the collapser to 
	// collide with.
	void Update () {
		if (transform.position.y < MKSceneManager.instance.floor) 
			GetComponent<Rigidbody>().isKinematic = true;
	}

	void OnCollisionEnter(Collision other) {
		if (MKSceneManager.instance.inputLocked || !activated)
			return;

		// If the player has collided with the collapser and it is not already falling,
		// start the fall.
		if (other.gameObject.name.Contains("IPPlayer") && !falling) {
			GetComponent<AudioSource>().Play();
			falling = true;
			StartCoroutine("Collapse");
		}
	}

	IEnumerator Collapse() {
		float startTime = Time.time;
		float rockTimer = 0;

		// This starts rocking the collapser back and forth for a time before switching it to
		// non-kinematic so it can fall with gravity.
		while (GetComponent<Rigidbody>().isKinematic) {
			rockTimer += Time.deltaTime;
			float phase = Mathf.Sin(rockTimer / rockTime);
			transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, phase * rockAngle);
			if (Time.time - startTime > fallDelay)
				GetComponent<Rigidbody>().isKinematic = false;
			yield return null;
		}
	}

	/* Both of the below methods are also overrides from IPGameObject. In addition to the 
	 * base class methods, we save and restore variables specific to this class. */
	public override void SaveCheckpointState () {
		base.SaveCheckpointState();

		savedRockTime = rockTime;
		savedRockAngle = rockAngle;
		savedFallDelay = fallDelay;
		
	}
	
	public override void RestoreCheckpointState () {
		base.RestoreCheckpointState();
	
		fallDelay = savedFallDelay;
		rockAngle = savedRockAngle;
		rockTime = savedRockTime;

		GetComponent<Rigidbody>().isKinematic = true;
		falling = false;
	}

}
