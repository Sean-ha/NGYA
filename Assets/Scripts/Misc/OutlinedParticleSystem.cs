using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attach on GameObject that has a ParticleSystem.
// Existing ParticleSystem dictates the behavior. This script will give the particles an outline effect by duplicating it and using the same seed
[RequireComponent(typeof(ParticleSystem))]
public class OutlinedParticleSystem : MonoBehaviour
{
	[Tooltip("Larger value = thicker outline")]
	public float outlineSize;
	public ParticleSystem.MinMaxGradient fillColor;
	public Material fillMaterial;

	private ParticleSystem existingParticles;

	private ParticleSystem fillParticles;

	private void Awake()
	{
		existingParticles = GetComponent<ParticleSystem>();
	}

	private void Start()
	{
		CreateOutlineParticleSystem();
	}

	public void CreateOutlineParticleSystem()
	{
		if (fillParticles != null)
			return;

		// Particles must be stopped and cleared (to set the seed properly)
		existingParticles.Stop();
		existingParticles.Clear();

		GameObject outlineParticlesGO = Instantiate(gameObject, transform, true);
		Destroy(outlineParticlesGO.GetComponent<OutlinedParticleSystem>());
		fillParticles = outlineParticlesGO.GetComponent<ParticleSystem>();
		ParticleSystem.MainModule settings = fillParticles.main;
		ParticleSystemRenderer renderSettings = fillParticles.GetComponent<ParticleSystemRenderer>();

		// Stop and clear these particles as well
		fillParticles.Stop();
		fillParticles.Clear();

		fillParticles.randomSeed = existingParticles.randomSeed;
		existingParticles.useAutoRandomSeed = false;
		settings.startColor = fillColor;
		settings.startSize = new ParticleSystem.MinMaxCurve(settings.startSize.constantMin - outlineSize, settings.startSize.constantMax - outlineSize);

		// Move fill particles in front of outline particles
		// Note: This might cause issues depending on use case...(i.e. object going between outline and fill)
		// if it does then this might need to go on its own layer
		renderSettings.sortingOrder += 1;

		// Might not be necessary for your use case; is necessary here
		renderSettings.material = fillMaterial;

		// Now play both outline and fill particle systems
		existingParticles.Play();
		fillParticles.Play();
	}
}
