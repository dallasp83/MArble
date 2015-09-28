using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum SpawnMethod {
	SpawnFurthest,
	SpawnRecent,
	RandomSpawn
}

public class MKSceneManager : MonoBehaviour {
	public static MKSceneManager instance;

	[Tooltip("The GUISkin to use.")]
	public GUISkin skin;
	[Tooltip("Reference to the player in the scene.")]
	public MKPlayer player;
	[Tooltip("This is the Y value that represents the point where the player dies if they fall past it.")]
	public float floor;
	[Tooltip("Indicates which spawn point to use.")]
	public SpawnMethod spawnMethod;
	[Tooltip("A list of points for the player to spawn at.")]
	public List<GameObject> spawnPoints;
	[Tooltip("The number of seconds to pause to show text when a player falls off.")]
	public float deathPause;
	[Tooltip("The text to display when a player falls off but has lives left.")]
	public string fallText;
	[Tooltip("The text to display when a player falls off and has no lives left.")]
	public string gameOverText;
	[Tooltip("The text to display when a player crosses the finish line.")]
	public string levelClearedText;

	[HideInInspector] 
	public bool inputLocked = true;
	[HideInInspector] 
	public bool mainMenu = false;

	private int currSpawnPoint;
	private System.Random random = new System.Random();
	private string labelText = "";
	private string buttonText = "";
	private bool showLabel;
	private bool showButton;
	private bool showMainOption;
	private bool done;
	
	void Awake()
	{
		if (instance == null )
		{
			instance = this;
		}
	}


	// Use this for initialization
	void Start () {
		currSpawnPoint = 0;
		if (spawnPoints.Count > 0) {
			Respawn();
		} else {
			Debug.LogError("You have not setup any spawn points for this scene!");
		}

	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnGUI () {

		GUI.skin = skin;

		GUILayout.BeginArea(new Rect(0,0,Screen.width,Screen.height));
		if (MKGameManager.instance != null && !mainMenu)
			GUILayout.Label("Lives: " + MKGameManager.instance.playerLives, "HUD" );

		if (showLabel) {
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.BeginVertical();
			GUILayout.FlexibleSpace();

			GUILayout.Label(labelText);
			if (showButton && GUILayout.Button (buttonText)) {
				MKGameManager.instance.GetComponent<AudioSource>().Play ();
				done = true;
			}
			if (showMainOption && GUILayout.Button ("Main Menu")) {
				MKGameManager.instance.GetComponent<AudioSource>().Play ();
				if (MKGameManager.instance.playerLives == 0)
					MKGameManager.instance.ResetGame();
				MKGameManager.instance.MainScene();
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}
		GUILayout.EndArea();
	}

	void Respawn() {
		if (spawnMethod == SpawnMethod.RandomSpawn)
			currSpawnPoint = random.Next(0, spawnPoints.Count);

		ResetScene(spawnPoints[currSpawnPoint]);
	}

	public void ResetScene(GameObject target) {
		if (player == null)
			return;

		float offset = player.GetComponent<Collider>().bounds.extents.y - target.GetComponent<Collider>().bounds.extents.y + 0.1f;
		player.Reset(target, new Vector3(0, offset, 0));
		IPCheckpointManager.RestoreCheckpointState();
		StartCoroutine("StartScene");
	}

	public void HandleCheckpointPass(GameObject checkpoint) {
		int spawnIndex = spawnPoints.IndexOf(checkpoint);
		if ((spawnMethod == SpawnMethod.SpawnRecent) || (spawnMethod == SpawnMethod.SpawnFurthest && spawnIndex > currSpawnPoint) ) {
			currSpawnPoint = spawnIndex;
			IPCheckpointManager.SaveCheckpointState();
		}

	}

	public void HandlePlayerDeath() {
		StartCoroutine("EndScene");
	}

	public void HandleGameOver() {
		StartCoroutine("EndGame");
	}

	public void HandleLevelCleared() {
		StartCoroutine("EndLevel");
	}

	IEnumerator EndScene() {
		inputLocked = true;
		showLabel = true;
		labelText = fallText;
		yield return new WaitForSeconds(deathPause);
		showLabel = false;
		Respawn();
	}

	IEnumerator EndGame() {
		inputLocked = true;
		showLabel = true;
		labelText = gameOverText;

		showMainOption = true;
		done = false;
		while (!done) {
			yield return null;
		}

		showLabel = false;
	}

	IEnumerator EndLevel() {
		inputLocked = true;
		showLabel = true;
		labelText = levelClearedText;
		player.ClearLevel();

		showMainOption = true;
		if (MKGameManager.instance.currentScene < MKGameManager.instance.levels.Length-1) {
			showButton = true;
			buttonText = "Next Level";
		} else {
			MKGameManager.instance.ResetGame();
		}

		done = false;
		while (!done) {
			yield return null;
		}
		
		showLabel = false;
		MKGameManager.instance.NextScene();
	}

	IEnumerator StartScene() {
		AudioSource[] aSources = GetComponents<AudioSource>();

		yield return new WaitForSeconds(1);
		showLabel = true;
		labelText = "3";
		aSources[0].Play();
		yield return new WaitForSeconds(1);
		labelText = "2";
		aSources[0].Play();
		yield return new WaitForSeconds(1);
		labelText = "1";
		aSources[0].Play();
		yield return new WaitForSeconds(1);
		labelText = "GO!";
		aSources[1].Play();
#if UNITY_STANDALONE || UNITY_WEBPLAYER

#else
		player.zeroAc = Input.acceleration;
#endif
		inputLocked = false;
		yield return new WaitForSeconds(1);
		labelText = "";
		showLabel = false;
	}
}
