using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;

public class InspireCollider : MonoBehaviour
{
	public float bannerLifetime;
	public int reloadSpeedIncrease;
	public float shotCooldownDecrease;

	[HorizontalLine]

	public GameObject holderObject;
	public Transform spawnEffect;
	public Transform circleField;

	private float currLifetime;
	private bool destroyed;

	private void Update()
	{
		if (!destroyed)
		{
			currLifetime += Time.deltaTime;

			if (currLifetime >= bannerLifetime)
			{
				// Banner is out of time, exit the field
				destroyed = true;

				spawnEffect.GetComponent<ScaleByTween>().ActivateScale();
				holderObject.GetComponent<SpriteRenderer>().enabled = false;
				circleField.DOScale(new Vector3(0, 0, 1), 0.15f).OnComplete(() =>
				{
					// Move far away to call OnTriggerExit2D
					holderObject.transform.position = new Vector2(100, 100);
					Destroy(holderObject, 0.15f);
				});
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		// Collision with player
		if (collision.CompareTag("Player"))
		{
			ObjectPooler.instance.CreateHitParticles(Color.red, collision.transform.position);
			ObjectPooler.instance.CreateHitParticles(Color.white, collision.transform.position);
			SoundManager.instance.PlaySound(SoundManager.Sound.EnterField);

			AmmoSystem.instance.AmmoRegenPerSecond += reloadSpeedIncrease;
			ShootManager.instance.ShootCooldown -= shotCooldownDecrease;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		// Collision with player
		if (collision.CompareTag("Player"))
		{
			ObjectPooler.instance.CreateHitParticles(Color.red, collision.transform.position);
			ObjectPooler.instance.CreateHitParticles(Color.white, collision.transform.position);
			SoundManager.instance.PlaySound(SoundManager.Sound.ExitField);

			AmmoSystem.instance.AmmoRegenPerSecond -= reloadSpeedIncrease;
			ShootManager.instance.ShootCooldown += shotCooldownDecrease;
		}
	}
}
