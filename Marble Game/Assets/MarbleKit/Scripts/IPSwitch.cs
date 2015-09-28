using UnityEngine;
using System.Collections;

public enum SwitchAction {
	SwitchOn,
	SwitchOff,
	Toggle,
	Momentary
}

public class IPSwitch : IPGameObject {

	[Tooltip("The position of the switch in the up position. Needed only when creating your own models.")]
	public Vector3 upPosition;
	[Tooltip("The position of the switch in the down position. Needed only when creating your own models.")]
	public Vector3 downPosition;
	[Tooltip("The amount of time the switch takes to fully depress or release.")]
	public float moveTime;
	[Tooltip("The switchable object that this switch controls.")]
	public IPSwitchableObject target;
	[Tooltip("The action to perform when the switch is pressed.")]
	public SwitchAction action;
	[Tooltip("The camera to show when the switch is activated. Leave null if no camera should be shown.")]
	public Camera targetCamera;
	[Tooltip("The time in seconds to show the target camera when the switch is activated.")]
	public float cutsceneTime;

	private bool switchDown = false;

	/* Because we inherit from IPGameObject, the class that handles checkpoint objects,
	 * we need to override that class's Start method and call the base class Start 
	 * method, as well as our IPSwitch specific checkpoint save method. */
	public override void Start () {
		base.Start();
		// start the switch in the up position
		transform.localPosition = upPosition;
		SaveCheckpointState();
	}
	

	void OnCollisionEnter(Collision other) {
		if (MKSceneManager.instance.inputLocked || !enabled)
			return;

		// When the player collides with the switch, start the move down animation
		if (other.gameObject.name.Contains("IPPlayer")) {
			MKPlayer player = other.gameObject.GetComponent<MKPlayer>();
			StartCoroutine("MoveDown", player);
		}
	}

	void OnCollisionStay(Collision other) {
		// Slowing the player makes it easier for them to stay on the switch.
		other.rigidbody.velocity *= 0.95f;
	}

	void OnCollisionExit(Collision other) {
		// Start the move up animation when the player leaves the switch
		if (other.gameObject.name.Contains("IPPlayer")) {
			MKPlayer player = other.gameObject.GetComponent<MKPlayer>();
			StartCoroutine("MoveUp", player);
		}
	}

	IEnumerator MoveDown(MKPlayer player) {		
		StopCoroutine("MoveUp");

		float duration = moveTime;
		float timer = 0.0f;

		// Move the switch to the down position
		while (transform.localPosition != downPosition) {		
			timer += Time.deltaTime;
			transform.localPosition = Vector3.Lerp (upPosition,downPosition,timer/duration);
			yield return null;
		}	

		// If the switch isn't already down, handle the calls to tell the target that it was 
		// pressed.
		if (!switchDown) {
			GetComponent<AudioSource>().Play();

			// Switch to the target camera if it exists
			if (targetCamera != null)
				StartCoroutine("ShowCamera", player);

			switchDown = true;
			switch(action) {
			case SwitchAction.SwitchOn:
				target.SwitchOn();
				break;
			case SwitchAction.SwitchOff:
				target.SwitchOff();
				break;
			default:
				target.SwitchToggle();
				break;
			}
		}
	}

	IEnumerator MoveUp(MKPlayer player) {		
		if (player.GetComponent<Rigidbody>().isKinematic)
			yield break;

		StopCoroutine("MoveDown");

		float duration = moveTime;
		float timer = 0.0f;

		// Move to the move up position
		while (transform.localPosition != upPosition) {		
			timer += Time.deltaTime;
			transform.localPosition = Vector3.Lerp (downPosition,upPosition,timer/duration);
			yield return null;
		}	

		// If it is a momentary switch, we need to tell the target object that is has been
		// switched again
		if (switchDown) {
			if (targetCamera != null && action == SwitchAction.Momentary)
				StartCoroutine("ShowCamera", player);

			switchDown = false;
			if (action == SwitchAction.Momentary) {
				GetComponent<AudioSource>().Play();
				target.SwitchToggle();
			}
		}
	}

	IEnumerator ShowCamera(MKPlayer player) {
		MKSceneManager.instance.inputLocked = true;

		// Stop the player so they aren't controlling themselves while they
		// are not visible.
		player.GetComponent<Rigidbody>().isKinematic = true;
		player.playerCamera.enabled = false;
		targetCamera.enabled = true;

		yield return new WaitForSeconds(cutsceneTime);

		targetCamera.enabled = false;
		player.playerCamera.enabled = true;
		player.GetComponent<Rigidbody>().isKinematic = false;
		MKSceneManager.instance.inputLocked = false;
	}

	/* Both of the below methods are also overrides from IPGameObject. */
	public override void SaveCheckpointState () {
		base.SaveCheckpointState();
	}
	
	public override void RestoreCheckpointState () {
		base.RestoreCheckpointState();
	}


}
