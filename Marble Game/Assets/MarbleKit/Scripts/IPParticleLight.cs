using UnityEngine;
using System.Collections;

/* This class will set the light attached to it to match the color of the target
 * particle system and apply a flickering effect */
public class IPParticleLight : MonoBehaviour {

	public ParticleSystem targetParticles;
	public float baseIntensity;
	public float intensityChange;
	public float baseRange;
	public float rangeChange;


	private ParticleSystem.Particle[] particles;
	private float duration;

	// Initialize the array that will be filled with the particles from the particle
	// system targeted.
	void Start () {
		particles = new ParticleSystem.Particle[targetParticles.maxParticles];
	}
	
	void Update () {
		// Fill the array with the particles, and if none are present set the light
		// intensity to 0 so that it is not shown
		if (targetParticles.GetParticles(particles) == 0) {
			GetComponent<Light>().intensity = 0;
			return;
		}

		// Grab the first particle in the array and gradually ping pong between the current
		// color, and the color of that particle
		float t = Mathf.PingPong(Time.time, targetParticles.duration) / targetParticles.duration;
		GetComponent<Light>().color = Color.Lerp(GetComponent<Light>().color, particles[0].color, t);

		// Randomly adjust the intensity and range based on the thresholds provided to enhance 
		// the flickering effect
		GetComponent<Light>().intensity = baseIntensity + Random.Range( 0.0f, intensityChange );
		GetComponent<Light>().range = baseRange + Random.Range( 0.0f, rangeChange );
	}

}
