using UnityEngine;
using System.Collections;

public class IPVelocityTrigger : MonoBehaviour {
	[Tooltip("The orientation of the force.")]
	public ForceType forceType;
	[Tooltip("The number to multiply the velocity by.")]
	public float velocityFactor;

	void OnTriggerEnter(Collider other) {
		if (MKSceneManager.instance.inputLocked || !enabled)
			return;
		
		Vector3 forceVector = Vector3.zero;

		if (other.attachedRigidbody && other.gameObject.name.Contains("IPPlayer")) {
			// If forcetype is relative to the pad, get the direction of the pad and add the velocity to the player.
			// Otherwise, just multiply the player's velocity by the set amount
			if (forceType == ForceType.RelativeToPad) {
				forceVector = transform.TransformDirection(Vector3.forward);
				forceVector = forceVector.normalized;
				forceVector *= velocityFactor;
				Vector3 oldVelocity = other.attachedRigidbody.velocity;
				Vector3 newVelocity = new Vector3(oldVelocity.x*forceVector.x, oldVelocity.y * forceVector.y, oldVelocity.z*forceVector.z);
				other.attachedRigidbody.velocity = newVelocity;
			} else {
				other.attachedRigidbody.velocity *= velocityFactor;
			}
		}
		
	}
}