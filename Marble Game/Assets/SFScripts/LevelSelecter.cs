using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelSelecter : MonoBehaviour 
{
	public Button level;

	void Start()
	{
		level = level.GetComponent<Button> ();
	}

	public void StartLevel()
	{
		Application.LoadLevel (2);
	}
}