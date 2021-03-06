using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenderShield : MonoBehaviour
{
	[Tooltip("Time it takes for the shield to regenerate after being destroyed")]
	[System.NonSerialized] public float cooldown;

	private SpriteRenderer sr;

	// Whether or not the upgrade has been obtained yet
	private bool obtained = false;
	// Whether or not the shield is active
	private bool shieldActive = true;
	private float currCooldown;

	private void Awake()
	{
		sr = GetComponent<SpriteRenderer>();
	}

	private void Update()
	{
		if (!shieldActive)
		{
			currCooldown -= Time.deltaTime;

			if (currCooldown <= 0)
			{
				// Enable shield
				sr.enabled = true;
				shieldActive = true;
				SoundManager.instance.PlaySound(SoundManager.Sound.ShieldReady);
				ObjectPooler.instance.CreateCircleHitEffect(Color.white, transform.position, 1.2f);
			}
		}
	}

	public void ObtainShield()
	{
		obtained = true;
	}

	/// <summary>
	/// Returns true if the shield is active and breaks. Returns false if the shield is not currently active.
	/// </summary>
	public bool TryBreakShield()
	{
		if (shieldActive && obtained)
		{
			// Pop shield
			shieldActive = false;
			sr.enabled = false;
			currCooldown = cooldown;
			SoundManager.instance.PlaySound(SoundManager.Sound.ShieldHit);
			ObjectPooler.instance.CreateHitParticles(Color.white, transform.position);
			ObjectPooler.instance.CreateHitParticles(Color.white, transform.position);

			return true;
		}
		else
		{
			return false;
		}
	}

	public void DecrementRemainingShieldCooldown(float amount)
	{
		currCooldown -= amount;
	}
}
