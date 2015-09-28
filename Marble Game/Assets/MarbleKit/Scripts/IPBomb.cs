using UnityEngine;
using System.Collections;

public class IPBomb : IPGameObject {

	public ParticleSystem explosion;
	public Material cleanMaterial;
	public Material burntMaterial;
	public float force;
	public float upwardForce;
	public float radius;


	private bool exploded = false;
	private bool savedExploded;

	/* Because we inherit from IPGameObject, the class that handles checkpoint objects,
	 * we need to override that class's Start method and call the base class Start 
	 * method, as well as our IPBomb specific checkpoint save method. */
	public override void Start () {
		base.Start();
		SaveCheckpointState();
	}

	void OnCollisionEnter(Collision other) {
		if (MKSceneManager.instance.inputLocked || !enabled || exploded)
			return;

		explosion.Play();
		GetComponent<AudioSource>().Play();

		// If a player collided, the bomb will explode.
		if (other.gameObject.name.Contains("IPPlayer")) {
			Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

			// Loop through the sphere representing the explosion and apply an explosion
			// force to all of the IPPlayers in its range.
			foreach(Collider c in colliders) {
				if (c.gameObject.name.Contains("IPPlayer")) {
					c.GetComponent<Rigidbody>().AddExplosionForce(force, transform.position, radius, upwardForce, ForceMode.Impulse);
				}
			}
		}

		// After the material is exploded, it should take on the burnt material. 
		GetComponent<Renderer>().material = burntMaterial;
		exploded = true;
	}


	/* Both of the below methods are also overrides from IPGameObject. In addition to the 
	 * base class methods, we save and restore variables specific to this class. */
	public override void SaveCheckpointState () {
		base.SaveCheckpointState();

		savedExploded = exploded;
	}
	
	public override void RestoreCheckpointState () {
		base.RestoreCheckpointState();

		exploded = savedExploded;

		if (exploded) 
			GetComponent<Renderer>().material = burntMaterial;
		else
			GetComponent<Renderer>().material = cleanMaterial;
	}
}
