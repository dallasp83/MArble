using UnityEngine;
using System.Collections;

/* This class is primarily used to hook into the checkpoint manager system */
public class IPGameObject : MonoBehaviour {

	private Vector3 savedPosition;
	private Vector3 savedAngles;
	private bool savedActive;

	/* This is the base Start method, which will be overridden and called from all
	 * objects that inherit from this class. It adds the object to the checkpoint 
	 * manager and saves the initial state */
	public virtual void Start () {
		IPCheckpointManager.AddIPGameObject(this);

		SaveCheckpointState();
	}

	/* The following two base class methods will also be overridden and called from
	 * all objects that inherit from this one. At a minimum, the position, rotation,
	 * and active status are saved and loaded for all IPGameObjects. Anything else
	 * object specific can be added in that object's override methods. */
	public virtual void SaveCheckpointState() {
		savedPosition = transform.position;
		savedAngles = transform.eulerAngles;
		savedActive = gameObject.activeSelf;
	}

	public virtual void RestoreCheckpointState() {
		transform.position = savedPosition;
		transform.eulerAngles = savedAngles;
		gameObject.SetActive(savedActive);
	}

}
