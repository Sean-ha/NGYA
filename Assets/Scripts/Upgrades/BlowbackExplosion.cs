using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BlowbackExplosion : MonoBehaviour
{
	public ParticleSystem explosionParticles;

	private void OnEnable()
	{
		GetComponent<CircleHitEffect>().SetHitEffect(Color.red, 1.25f, 0.3f);

		// Play explosion effects
		explosionParticles.Play();

		// Set color to white after some time
		ExecuteAction.instance.Execute(0.05f, () => GetComponent<SpriteRenderer>().color = Color.white);

		// Disable collider after some time
		ExecuteAction.instance.Execute(0.05f, () => GetComponent<Collider2D>().enabled = false);

		// Set color of particles to white after some time
		ExecuteAction.instance.Execute(0.05f, () => explosionParticles.GetComponent<FlashWhite>().InitiateWhiteFlash(0.5f));

		explosionParticles.transform.parent = null;

		// Destroy self after some time
		ExecuteAction.instance.Execute(0.3f, () => Destroy(gameObject));
	}
}
