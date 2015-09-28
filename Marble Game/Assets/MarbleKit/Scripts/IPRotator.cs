using UnityEngine;
using System.Collections;

public enum RotateAxis {
	Y,
	X,
	Z
}

public enum ClockDirection {
	Clockwise,
	Counterclockwise
}

public class IPRotator : IPSwitchableObject {

	[Tooltip("Attaches the player to the platform.")]
	public bool attachPlayer;
	[Tooltip("The rotation direction of the object.")]
	public ClockDirection direction;
	[Tooltip("The axis to rotate around.")]
	public RotateAxis rotationAxis;
	[Tooltip("The number of degress between pauses.")]
	public int stepDegrees;
	[Tooltip("The pause time between steps in seconds.")]
	public float stepDelay;
	[Tooltip("The speed of rotation. Needs to be a positive number.")]
	public float speed;

	private bool delayed = true;
	private float angle = 0;
	private Vector3 axis;

	// Checkpoint variables
	private bool savedAttachPlayer;
	private ClockDirection savedDirection;
	private RotateAxis savedRotationAxis;
	private int savedStepDegrees;
	private float savedStepDelay;
	private float savedSpeed;
	private bool savedDelayed;
	private float savedAngle;
	private Vector3 savedAxis;

	/* Because we inherit from IPGameObject, the class that handles checkpoint objects,
	 * we need to override that class's Start method and call the base class Start 
	 * method, as well as our IPRotator specific checkpoint save method. */
	public override void Start () {
		base.Start();
		// Setup the axis based on user settings
		if (rotationAxis == RotateAxis.X)
			axis = new Vector3(1,0,0);
		else if (rotationAxis == RotateAxis.Y)
			axis = new Vector3(0,1,0);
		else
			axis = new Vector3(0,0,1);

		SaveCheckpointState();

		// Wait for the first delay before moving
		StartCoroutine("WaitForDelay");
	}
	
	void Update () {
		if (MKSceneManager.instance.inputLocked)
			return;

		// If needed, rotate to the next target. Wait for the delay time
		// when it is reached before repeating the process
		if (!delayed && activated) {
			float amount = speed * Time.deltaTime;
			angle += amount;
			if (angle >= stepDegrees) {
				amount -= angle - stepDegrees;
				delayed = true;
				StartCoroutine("WaitForDelay");
			}

			if (direction != ClockDirection.Clockwise)
				amount = -amount;

			transform.Rotate(axis, amount);
		}
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

		savedAttachPlayer = attachPlayer;
		savedDirection = direction;
		savedRotationAxis = rotationAxis;
		savedStepDegrees = stepDegrees;
		savedStepDelay = stepDelay;
		savedSpeed = speed;

		savedAxis = axis;
		savedAngle = angle;
		savedDelayed = delayed;
	}
	
	public override void RestoreCheckpointState () {
		StopCoroutine("WaitForDelay");
		base.RestoreCheckpointState();

		attachPlayer = savedAttachPlayer;
		direction = savedDirection;
		rotationAxis = savedRotationAxis;
		stepDegrees = savedStepDegrees;
		stepDelay = savedStepDelay;
		speed = savedSpeed;

		delayed = savedDelayed;
		angle = savedAngle;
		axis = savedAxis;

		Debug.Log("Angle = " + angle + "Rot Angle = " + transform.eulerAngles.y);

		if (delayed)
			StartCoroutine("WaitForDelay");
	}

	// Wait for the desired delay between stops
	IEnumerator WaitForDelay() {
		yield return new WaitForSeconds(stepDelay);
		angle = 0;
		delayed = false;
	}
}
