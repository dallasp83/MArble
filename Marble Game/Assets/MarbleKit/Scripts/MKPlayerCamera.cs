using UnityEngine;
using System.Collections;

public class MKPlayerCamera : MonoBehaviour {
	public GameObject player;

	[Tooltip("The sensitivity of the joystick controls.")]
	public float joystickSensitivity;
	[Tooltip("The sensitivity of the mouse controls.")]
	public float mouseSensitivity;
	[Tooltip("The distance between the player and the camera.")]
	public float distance;
	[Tooltip("The initial vertical angle between the player and the camera.")]
	public float verticalAngle;
	[Tooltip("Damping value for smoothing camera. Ignored for mouse controls.")]
	public float damping;

	[Tooltip("The virtual joystick for mobile control.")]
	public Joystick rightStick;
	[Tooltip("The sensitivity of the virtual joystick.")]
	public float touchJoySensitivity;
	
	private bool usingJoystick;
	private float vAngle;
	private float hAngle;
	private bool levelCleared = false;

	void Start () {
#if UNITY_STANDALONE || UNITY_WEBPLAYER

#else
		rightStick.gameObject.SetActive(true);
#endif
	}

	void Update() {
		if (levelCleared) {
			hAngle += Time.deltaTime;
			TransformAndLook(true);
		}

		if (MKSceneManager.instance.inputLocked)
			return;

		float x, y;

		// Handle joystick input
		x = Input.GetAxis("Camera Horizontal");
		y = Input.GetAxis("Camera Vertical");

		if (Input.GetButton("Control Camera")) {
			x = Input.GetAxis("Horizontal");
			y = Input.GetAxis("Vertical");
		} 

		if (x != 0 || y != 0)
			usingJoystick = true;

		hAngle += x * joystickSensitivity * Time.deltaTime;
		vAngle -= y * joystickSensitivity * Time.deltaTime;


#if UNITY_STANDALONE || UNITY_WEBPLAYER
		// Handle mouse input
		x = Input.GetAxis("Mouse X");
		y = Input.GetAxis("Mouse Y");
#else
		x = -rightStick.position.x * touchJoySensitivity;
		y = -rightStick.position.y * touchJoySensitivity;
#endif

		if (x != 0 || y != 0)
			usingJoystick = false;

		hAngle += x * mouseSensitivity * Time.deltaTime;
		vAngle -= y * mouseSensitivity * Time.deltaTime;


		// Clamp results to acceptable values
		vAngle = Mathf.Clamp(vAngle, 0.0f, 1.0f);

	}
	
	void LateUpdate () {
		// Damping makes the mouse movements jerky, only use it for joystick movement
		TransformAndLook(usingJoystick);
	}

	void TransformAndLook(bool damp) {
		// Transform position to match angles
		float x = player.transform.position.x + distance * Mathf.Cos(hAngle) * Mathf.Cos(vAngle);
		float z = player.transform.position.z - distance * Mathf.Sin(hAngle) * Mathf.Cos(vAngle);
		float y = player.transform.position.y + distance * Mathf.Sin(vAngle);
		transform.position = new Vector3(x, y, z);

		if (damp) {
			Quaternion rotation = Quaternion.LookRotation(player.transform.position - transform.position);
			transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
		} else {
			transform.LookAt(player.transform.position);
		}
	}

	public void ResetCamera(GameObject obj) {
		vAngle = verticalAngle * Mathf.Deg2Rad;
		hAngle = (obj.transform.eulerAngles.y + 90) * Mathf.Deg2Rad;
		
		// Do an initial transform with no damping so the view is right at the start
		TransformAndLook(false);
	}

	public void ClearLevel() {
		levelCleared = true;
	}
}
