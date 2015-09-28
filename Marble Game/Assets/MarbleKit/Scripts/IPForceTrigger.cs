using UnityEngine;
using System.Collections;

public enum ForceType {
	RelativeToPad,
	RelativeToTarget
}

public enum ForceDirection {
	Forward,
	Up,
	Right
}

public class IPForceTrigger : MonoBehaviour {
	[Tooltip("The orientation of the force.")]
	public ForceType forceType;
	[Tooltip("The direction to apply the force.")]
	public ForceDirection forceDirection;
	[Tooltip("The force to apply to objects on the pad.")]
	public float force;
	[Tooltip("How much force is lost at the end of the trigger. 1.0 equals all of it.")]
	public float forceFalloff;

	void OnTriggerStay(Collider other) {
		if (MKSceneManager.instance.inputLocked || !enabled)
			return;

		Vector3 forceVector = Vector3.zero;

		// Calculate the distance from the start of the collider and decrease force according to
		// force falloff setting
		float distance = other.bounds.center.z - GetComponent<Collider>().bounds.min.z;
		float forceFactor = distance / GetComponent<Collider>().bounds.size.z;
		forceFactor = Mathf.Clamp(forceFactor, 0.0f, 1.0f);
		float forceLost = force * forceFactor * forceFalloff;

		if (forceType == ForceType.RelativeToPad) {
			if (forceDirection == ForceDirection.Forward)
				forceVector = transform.TransformDirection(Vector3.forward);
			else if (forceDirection == ForceDirection.Up)
				forceVector = transform.TransformDirection(Vector3.up);
			else if (forceDirection == ForceDirection.Right)
				forceVector = transform.TransformDirection(Vector3.right);
		} else {
			/* The ball rolling makes its local coordinates unreliable.
			 * We know it is travelling in the same X/Z direction as the camera
			 * and we know that it's 'Up' direction will match the trigger that
			 * it is on top of */
			if (forceDirection == ForceDirection.Forward)
				forceVector = Camera.main.transform.TransformDirection(Vector3.forward);
			else if (forceDirection == ForceDirection.Up)
				forceVector = transform.TransformDirection(Vector3.up);
			else if (forceDirection == ForceDirection.Right)
				forceVector = Camera.main.transform.TransformDirection(Vector3.right);
		}
		forceVector = forceVector.normalized;

		if (other.attachedRigidbody && other.gameObject.name.Contains("IPPlayer")) {
			other.attachedRigidbody.AddForce(forceVector * (force - forceLost));
		}
		
	}
}
