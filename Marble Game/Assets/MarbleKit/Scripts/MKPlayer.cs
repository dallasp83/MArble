using UnityEngine;
using System.Collections;

public class MKPlayer : MonoBehaviour {

	public MKPlayerCamera playerCamera;
	[Tooltip("The force added by the directional controls.")]
	public int controlForce;
	[Tooltip("The upward force added when the player jumps. Set to 0 to disable jumping.")]
	public int jumpForce;

	[Tooltip("Whether or not to use the accelerometer for mobile control.")]
	public bool useAccelerometer;

	[Tooltip("The sensitivity of the accelerometer in the side to side direction.")]
	public float accelSensitivityH;
	[Tooltip("The sensitivity of the accelerometer in the forward / backward direction.")]
	public float accelSensitivityV;
	[Tooltip("The dead zone before accelerometer controls register.")]
	public float accelDeadZone;

	[Tooltip("The virtual joystick for mobile control.")]
	public Joystick leftStick;
	[Tooltip("The virtual joystick for mobile control.")]
	public Joystick rightStick;
	[Tooltip("The sensitivity of the virtual joystick.")]
	public float touchJoySensitivity;

	[HideInInspector] 
	public bool dead = false;

	private float distToGround;
	private bool moveCamera = false;

	private float touchDuration;
	private Touch touch;
	
#if UNITY_STANDALONE || UNITY_WEBPLAYER

#else
	[HideInInspector] 
	public Vector3 zeroAc;
	private Vector3 curAc;
	private float smooth = 0.5f;

	private bool jumping = false;
#endif

	void Awake() {
		gameObject.name = "IPPlayer";
	}

	void Start () {
		distToGround = GetComponent<Collider>().bounds.extents.y;

#if UNITY_STANDALONE || UNITY_WEBPLAYER
		
#else
		if (!useAccelerometer)
			leftStick.gameObject.SetActive(true);
		curAc = Vector3.zero;
#endif

		if (MKSceneManager.instance == null)
			Debug.LogError("You haven't added a SceneManager to the scene!");
	}

	void Update () {
		if (MKSceneManager.instance.inputLocked)
			return;

		if (transform.position.y < MKSceneManager.instance.floor && !dead) {
			Kill ();
		}
		
#if UNITY_STANDALONE || UNITY_WEBPLAYER
		if (Input.GetButtonDown("Jump") && IsGrounded()) {
			GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce);
		}

		moveCamera = Input.GetButton("Control Camera");
#else
		if (IsGrounded()) {
			if (rightStick.tapCount == 2) {
				if (!jumping) {
					jumping = true;
					rigidbody.AddForce(Vector3.up * jumpForce);
				} 
			} else {
				jumping = false;
			}
		} 
#endif

	}
	
	void FixedUpdate() {
		if (MKSceneManager.instance.inputLocked)
			return;

		if (!moveCamera && IsGrounded()) {
			Vector3 forward = playerCamera.transform.TransformDirection(Vector3.forward);
			forward.y = 0;
			forward = forward.normalized;

			Vector3 right = playerCamera.transform.TransformDirection(Vector3.right);
			right.y = 0;
			right = right.normalized;

			float x = 0;
			float z = 0;
#if UNITY_STANDALONE || UNITY_WEBPLAYER
			x = Input.GetAxis("Horizontal");
			z = Input.GetAxis("Vertical");

#else
			if (!useAccelerometer) {
				x = leftStick.position.x * touchJoySensitivity;
				z = leftStick.position.y * touchJoySensitivity;
			} else {
				curAc = Vector3.Lerp(curAc, Input.acceleration-zeroAc, Time.deltaTime/smooth);
				if (curAc.x > accelDeadZone || curAc.x < -accelDeadZone)
					x = Mathf.Clamp(curAc.x * accelSensitivityH, -1, 1);
				if (curAc.z > accelDeadZone || curAc.z < -accelDeadZone)
					z = -Mathf.Clamp(curAc.z * accelSensitivityV, -1, 1);
			}

//			Vector3 movement = playerCamera.transform.TransformDirection(new Vector3(x, 0, z));
//			rigidbody.velocity = (movement * touchJoySensitivity * speedLoss) * Time.deltaTime;
#endif
			GetComponent<Rigidbody>().AddForce(forward * z * controlForce * Time.deltaTime);
			GetComponent<Rigidbody>().AddForce(right * x * controlForce * Time.deltaTime);

		}
	}

	
	bool IsGrounded(){
		return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
	}	

	public void Stop() {
		GetComponent<Rigidbody>().velocity = Vector3.zero;
		GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
	}

	public void Reposition(GameObject target, Vector3 offset) {
		transform.position = target.transform.position + offset;
		playerCamera.ResetCamera(target);
	}

	public void Reset(GameObject target, Vector3 offset) {
		Stop();
		Reposition(target, offset);
		dead = false;
	}

	public void Kill() {
		if (!dead) {
			dead = true;
			AudioSource[] aSources = GetComponents<AudioSource>();
			MKGameManager.instance.playerLives--;
			if (MKGameManager.instance.playerLives > 0) {
				aSources[0].Play();
				MKSceneManager.instance.HandlePlayerDeath();
			} else { 
				aSources[1].Play();
				MKSceneManager.instance.HandleGameOver();
			}
		}
	}

	public void ClearLevel() {
		playerCamera.ClearLevel();
		StartCoroutine("AnimateLevelEnd");
	}

	IEnumerator AnimateLevelEnd() {
		GetComponent<Rigidbody>().velocity = Vector3.zero;
		GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

		float targetY = transform.position.y + 3.5f;
		float startY = transform.position.y;

		float timer = 0.0f;
		while (MKSceneManager.instance.inputLocked) {		
			timer += Time.deltaTime;
			float newY = Mathf.Lerp (startY ,targetY, timer/1.75f);
			transform.position = new Vector3(transform.position.x, newY, transform.position.z);
			yield return null;
		}
	}

}
