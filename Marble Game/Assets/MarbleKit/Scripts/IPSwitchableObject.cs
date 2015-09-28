using UnityEngine;
using System.Collections;

public class IPSwitchableObject : IPGameObject {

	[Tooltip("Whether the object is active or not. This is controllable by the IPSwitch script or the Switch prefab.")]
	public bool activated = true;

	private bool savedActivated;

	/* Because we inherit from IPGameObject, the class that handles checkpoint objects,
	 * we need to override that class's Start method and call the base class Start 
	 * method, as well as our IPSwitchableObject specific checkpoint save method. */
	public override void Start () {
		base.Start();
		SaveCheckpointState();
	}

	/* The next three methods are virtual methods that get called by the switch when any
	 * object that inherits this one activate that switch. They can be overridden by the 
	 * object in question to perform functions specific to that object */
	public virtual void SwitchOn() {
		activated = true;
	}

	public virtual void SwitchOff() {
		activated = false;
	}

	public virtual void SwitchToggle() {
		activated = !activated;
	}

	/* Both of the below methods are also overrides from IPGameObject. In addition to the 
	 * base class methods, we save and restore variables specific to this class. */
	public override void SaveCheckpointState () {
		base.SaveCheckpointState();
		savedActivated = activated;
	}

	public override void RestoreCheckpointState () {
		base.RestoreCheckpointState();
		activated = savedActivated;
	}
}
