using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IPMover : IPSwitchableObject {

	[Tooltip("Attaches the player to the platform.")]
	public bool attachPlayer;
	[Tooltip("Makes the object pause at all stops instead of just the endpoints.")]
	public bool pauseAtAllStops;
	[Tooltip("When set, stops the object from going back and forth between stops.")]
	public bool oneWay;
	[Tooltip("An array of GameObjects representing the waypoints to stop at. They must be in order.")]
	public GameObject[] stops;
	[Tooltip("The number of seconds to pause at a stop.")]
	public float stopDelay;
	[Tooltip("The speed of the object.")]
	public float speed;

	private bool delayed = true;
	private bool reverse = false;
	private int currStop;
	private Vector3 motionVector;
	private Vector3 target;

	// Checkpoint variables
	private bool savedAttachPlayer;
	private bool savedPauseAtAllStops;
	private bool savedOneWay;
	private float savedStopDelay;
	private float savedSpeed;

	private bool savedDelayed;
	private bool savedReverse;
	private int savedCurrStop;
	private Vector3 savedMotionVector;
	private Vector3 savedTarget;
	private bool finished, savedFinished;
	
	/* Because we inherit from IPGameObject, the class that handles checkpoint objects,
	 * we need to override that class's Start method. */
	public override void Start () {
		currStop = 0;
		savedFinished = false;
		if (stops.Length > 0) {
			transform.position = stops[0].transform.position;
			SetupNextWaypoint();
		} else {
			Debug.LogError("You have not setup any waypoints for this moving object!");
		}

		if (stops.Length == 1)
			Debug.LogWarning("This moving object only has 1 waypoint. It must have at least 2 in order to move.");

		base.Start();
		StartCoroutine("WaitForDelay");
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (MKSceneManager.instance.inputLocked)
			return;

		// Move toward the next position and, one reached, set up the next one
		if (!delayed && activated && stops.Length > 1) {
			Vector3 movementAmount = motionVector * speed * Time.deltaTime;
			Vector3 newPosition = transform.position + movementAmount;

			if ((motionVector.x > 0 && newPosition.x > target.x) || (motionVector.x < 0 && newPosition.x < target.x))
				newPosition.x = target.x;

			if ((motionVector.y > 0 && newPosition.y > target.y) || (motionVector.y < 0 && newPosition.y < target.y))
				newPosition.y = target.y;

			if ((motionVector.z > 0 && newPosition.z > target.z) || (motionVector.z < 0 && newPosition.z < target.z))
				newPosition.z = target.z;

			transform.position = newPosition;
			if (newPosition == target) {
				SetupNextWaypoint();
			}
		}
	}

	void SetupNextWaypoint() {
		if (reverse) {
			if (currStop == 0) {
				reverse = false;
				currStop = 1;
				if (!pauseAtAllStops)
					StartCoroutine("WaitForDelay");
			} else {
				currStop--;
			}
		} else {
			if (currStop == stops.Length - 1) {
				if (oneWay) {
					activated = false;
					finished = true;
				}
				reverse = true;
				currStop--;
				if (!pauseAtAllStops)
					StartCoroutine("WaitForDelay");
			} else {
				currStop++;
			}
		}

		target = stops[currStop].transform.position;
		motionVector = target - transform.position;
		motionVector = motionVector.normalized;

		if (pauseAtAllStops)
			StartCoroutine("WaitForDelay");

	}

	/* The following two methods handle parenting the player to the moving object when
	 * it enters the collider, and removing it upon exit. This makes it easier for the 
	 * player to stay on the platform if needed */
	private void OnCollisionEnter(Collision c)
	{
		if (attachPlayer && c.gameObject.name.Contains("IPPlayer"))
			c.transform.parent = transform;
	}
	
	private void OnCollisionExit(Collision c)
	{
		c.transform.parent = null;
	}

	/* The next two methods are overrides from IPGameObject. In addition to the 
	 * base class methods, we save and restore variables specific to this class. */
	public override void SaveCheckpointState () {
		base.SaveCheckpointState();

		savedPauseAtAllStops = pauseAtAllStops;
		savedReverse = reverse;
		savedOneWay = oneWay;
		savedCurrStop = currStop;
		savedMotionVector = motionVector;
		savedTarget = target;

		savedAttachPlayer = attachPlayer;
		savedStopDelay = stopDelay;
		savedSpeed = speed;		
		savedDelayed = delayed;
		savedFinished = finished;
	}
	
	public override void RestoreCheckpointState () {
		StopCoroutine("WaitForDelay");
		base.RestoreCheckpointState();
		
		attachPlayer = savedAttachPlayer;
		stopDelay = savedStopDelay;
		speed = savedSpeed;		

		pauseAtAllStops = savedPauseAtAllStops;
		oneWay = savedOneWay;
		if (!savedFinished) {
			if (oneWay) {
				currStop = 0;
				if (stops.Length > 0) {
					transform.position = stops[0].transform.position;
					SetupNextWaypoint();
					delayed = true;
				}
			} else {
				reverse = savedReverse;
				currStop = savedCurrStop;
				motionVector = savedMotionVector;
				target = savedTarget;			
				delayed = savedDelayed;
			}
		}
		if (delayed)
			StartCoroutine("WaitForDelay");
	}

	// Wait for the desired delay between stops
	IEnumerator WaitForDelay() {
		delayed = true;
		yield return new WaitForSeconds(stopDelay);
		delayed = false;
	}


}
