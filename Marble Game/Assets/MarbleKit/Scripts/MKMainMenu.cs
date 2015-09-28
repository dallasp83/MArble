using UnityEngine;
using System.Collections;

public class MKMainMenu : MonoBehaviour {

	[Tooltip("The GUISkin to use.")]
	public GUISkin skin;

	public Vector3 center;
	public float distance;
	public float height;
	public float speed;

	private float hAngle;

	void Start () {
		// This tells the scene manager not to show the lives remaining label
		// in the GUI
		MKSceneManager.instance.mainMenu = true;
	}

	/* The camera should be constantly rotating around the chosen center point at the 
	 * chosen speed */
	void Update () {		
		hAngle += speed * Time.deltaTime;
		TransformAndLook();
	}

	void OnGUI() {
		GUI.skin = skin;
		// Mobile games don't need the quit button
#if UNITY_STANDALONE || UNITY_WEBPLAYER
		GUILayout.BeginArea(new Rect(0,0,Screen.width,Screen.height));
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button ("Quit")) {
			MKGameManager.instance.GetComponent<AudioSource>().Play ();
			Application.Quit();
		}
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
#endif
		GUILayout.BeginArea(new Rect(0,0,Screen.width,Screen.height));
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.BeginVertical();
		GUILayout.FlexibleSpace();
		
		if (GUILayout.Button ("New Game")) {
			MKGameManager.instance.GetComponent<AudioSource>().Play ();
			MKGameManager.instance.ResetGame();
			MKGameManager.instance.StartGame();
		}

		// If save functionality is built into the game manager, this will give them
		// a continue option once they've completed the first scene.
		if (MKGameManager.instance.currentScene > 0 && GUILayout.Button ("Continue")) {
			MKGameManager.instance.GetComponent<AudioSource>().Play ();
			MKGameManager.instance.StartGame();
		}

		
		GUILayout.FlexibleSpace();
		GUILayout.EndVertical();
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}

	/* This moves the camera away from the center point by the distance chosen and rotates
	 * it around that point by the angle that is incremented in the Update method */
	void TransformAndLook() {
		// Transform position to match angles
		float x = center.x + distance * Mathf.Cos(hAngle);
		float z = center.y + distance * Mathf.Sin(hAngle);
		float y = height;
		transform.position = new Vector3(x, y, z);
		
		transform.LookAt(center);
	}

}
