using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IPCheckpointManager : MonoBehaviour {

	// A list of IPGameObjects that will be managed by the checkpoint system.
	static private List<IPGameObject> gameObjects = new List<IPGameObject>();

	// This is called by each IPGameObject when it starts up. It adds the object
	// to the manager if it is not already there.
	static public void AddIPGameObject(IPGameObject obj) {
		if (!gameObjects.Contains(obj))
			gameObjects.Add(obj);
	}

	// Removes an IPGameObject from the manager if it exists
	static public void RemoveIPGameObject(IPGameObject obj) {
		if (gameObjects.Contains(obj))
			gameObjects.Remove(obj);
	}

	// This loops through all of the objects in the manager and calls their
	// SaveCheckPointState methods
	static public void SaveCheckpointState() {
		foreach (IPGameObject obj in gameObjects) {
			if (obj != null)
				obj.SaveCheckpointState();
		}
	}

	// This loops through all of the objects in the manager and calls their
	// RestoreCheckPointState methods
	static public void RestoreCheckpointState() {
		foreach (IPGameObject obj in gameObjects) {
			if (obj != null)
				obj.RestoreCheckpointState();
		}
	}

}
