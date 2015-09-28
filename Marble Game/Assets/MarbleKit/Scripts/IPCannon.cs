using UnityEngine;
using System.Collections;

public class IPCannon :IPSwitchableObject {

	[Tooltip("The projectile object to fire from the cannon.")]
	public GameObject cannonball;
	[Tooltip("The game object that holds the smoke particles.")]
	public GameObject smokeHolder;
	[Tooltip("The smoke particles.")]
	public ParticleSystem smoke;

	[Tooltip("The initial delay before firing in seconds.")]
	public float fireDelay;
	[Tooltip("The number of seconds between firings.")]
	public float fireInterval;
	[Tooltip("The force of the projectile.")]
	public float force;

	private bool firing = true;
	private bool delayPassed = false;
	private float savedFireDelay;
	private float savedFireInterval;
	private float savedForce;

	/* Because we inherit from IPGameObject, the class that handles checkpoint objects,
	 * we need to override that class's Start method and call the base class Start 
	 * method, as well as our IPCannon specific checkpoint save method. */
	public override void Start () {
		base.Start();
		SaveCheckpointState();
		smokeHolder.GetComponent<Light>().range = 0;
	}
	
	void Update () {
		if (!MKSceneManager.instance.inputLocked && activated) {
			if (!delayPassed)
				StartCoroutine("WaitForFireDelay");

			if (!firing)
				StartCoroutine("Fire");

			// The 'Fire' coroutine will set the light range to 10. This gradually moves it back
			// down to 0.
			if (smokeHolder.GetComponent<Light>().range > 0)
				smokeHolder.GetComponent<Light>().range = Mathf.Lerp(smokeHolder.GetComponent<Light>().range, 0, Time.deltaTime);
		}
	}

	/* The next two methods are also overrides from IPGameObject. In addition to the 
	 * base class methods, we save and restore variables specific to this class. */
	public override void SaveCheckpointState () {
		base.SaveCheckpointState();

		savedFireDelay = fireDelay;
		savedFireInterval = fireInterval;
		savedForce = force;
	}
	
	public override void RestoreCheckpointState () {
		base.RestoreCheckpointState();

		fireDelay = savedFireDelay;
		fireInterval = savedFireInterval;
		force = savedForce;

		firing = true;
		delayPassed = false;

		smokeHolder.GetComponent<Light>().range = 0;
		StopAllCoroutines();
	}

	// This allows us to control the delay before the cannon first fires.
	IEnumerator WaitForFireDelay() {
		delayPassed = true;
		yield return new WaitForSeconds(fireDelay);
		firing = false;
	}

	IEnumerator Fire() {
		firing = true;
		GetComponent<AudioSource>().Play();

		// Set the light range to non-zero to represent the muzzle flash.
		smokeHolder.GetComponent<Light>().range = 10;

		// Create a smoke particle system to represent the powder burning
		ParticleSystem smokeObject = (ParticleSystem)Instantiate(smoke, smokeHolder.transform.position, transform.rotation);
		// Destroy the smoke after 5 seconds so they don't pile up in memory
		Destroy(smokeObject, 5.0f);

		// Create a connonball object inside the cannon and apply the force to shoot it out
		GameObject ball = (GameObject)Instantiate(cannonball, transform.position, transform.rotation);
		ball.GetComponent<Rigidbody>().AddForce((transform.forward * force), ForceMode.Impulse);
		// Destroy the cannonball object after 2 seconds so they don't pile up in memory.
		Destroy(ball, 2.0f);

		// Wait until the fire interval has passed to reset the sytem and signal it to fire again
		yield return new WaitForSeconds(fireInterval);
		firing = false;
	}
}
