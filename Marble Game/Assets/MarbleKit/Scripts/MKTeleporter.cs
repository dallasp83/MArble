using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MKTeleporter : IPSwitchableObject {

	[Tooltip("The height offset from the position of this object to teleport the player to. Useful for positioning an object on the teleporter mesh.")]
	public float offsetY;
	[Tooltip("The particle system that plays when the teleporter is active but not in use.")]
	public ParticleSystem idleParticles;
	[Tooltip("The particle system that plays when the teleporter is teleporting something.")]
	public ParticleSystem actionParticles;
	[Tooltip("The index into the targets list that the teleporter will transport to. Ignored if Random Target is set.")]
	public int destination;
	[Tooltip("Whether or not the teleporter should randomly choose a destination from the list of targets.")]
	public bool randomTarget;
	[Tooltip("A list of other teleporter objects that this one can choose as a destination.")]
	public MKTeleporter[] targets;

	private int savedDestination;
	private bool savedRandomTarget;

	[HideInInspector]
	public List<Collider> recieving = new List<Collider>();

	private float particleSpinSpeed = 500;
	private Vector3 offsetVector;

	private System.Random random = new System.Random();

	// Use this for initialization
	public override void Start () {
		base.Start();
		offsetVector = new Vector3(0, offsetY, 0);

		if (activated && targets.Length > 0)
			idleParticles.Play();

		SaveCheckpointState();
	}
	
	// Update is called once per frame
	void Update () {
		actionParticles.gameObject.transform.Rotate(Vector3.forward, particleSpinSpeed * Time.deltaTime);	
		idleParticles.gameObject.transform.Rotate(Vector3.forward, particleSpinSpeed * Time.deltaTime);	

		if (activated && targets.Length != 0 && !idleParticles.isPlaying)
			idleParticles.Play();
		else if (!activated && idleParticles.isPlaying)
			idleParticles.Stop();
	}

	void OnTriggerEnter(Collider other) {
		if (recieving.Contains(other) || targets.Length < 1 || !activated)
			return;

		if (randomTarget)
			destination = random.Next(0, targets.Length);

		if (destination < targets.Length && other.gameObject.name.Contains("IPPlayer")) {
			targets[destination].recieving.Add(other);
			MKPlayer player = (MKPlayer)other.GetComponent<MKPlayer>();
			if (player != null)
				StartCoroutine("TeleportPlayer", player);
		}
	}
	
	void OnTriggerExit(Collider other) {
		if (recieving.Contains(other))
			recieving.Remove(other);
	}

	public override void SaveCheckpointState () {
		base.SaveCheckpointState();

		savedDestination = destination;
		savedRandomTarget = randomTarget;
	}
	
	public override void RestoreCheckpointState () {
		StopCoroutine("WaitForDelay");
		base.RestoreCheckpointState();

		destination = savedDestination;
		randomTarget = savedRandomTarget;
	}

	IEnumerator TeleportPlayer(MKPlayer player) {
		MKSceneManager.instance.inputLocked = true;
		actionParticles.Play();
		GetComponent<AudioSource>().Play();
		player.Stop();
		yield return new WaitForSeconds(0.5f);
		targets[destination].actionParticles.Play();
		targets[destination].GetComponent<AudioSource>().Play();
		player.Reposition(targets[destination].gameObject, offsetVector);
		yield return new WaitForSeconds(0.5f);
		MKSceneManager.instance.inputLocked = false;
	}
}
