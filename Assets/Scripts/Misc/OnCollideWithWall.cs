using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnCollideWithWall : MonoBehaviour
{
	[Tooltip("Play circle hit effect + hit particles when colliding with wall")]
	public bool playParticleEffectsOnHit;
	[EnableIf("playParticleEffectsOnHit")]
	public Color particleColor;
	[EnableIf("playParticleEffectsOnHit")]
	public float particleSize;

	public UnityEvent onCollideWithWall;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		// Wall layer
		if (collision.gameObject.layer == 9)
		{
			if (playParticleEffectsOnHit)
			{
				Vector2 closestPoint = collision.ClosestPoint(transform.position);
				ObjectPooler.instance.CreateCircleHitEffect(particleColor, closestPoint, particleSize);
				ObjectPooler.instance.CreateHitParticles(particleColor, closestPoint);
				SoundManager.instance.PlaySound(SoundManager.Sound.EnemyHit);
			}

			onCollideWithWall.Invoke();
		}
	}
}
