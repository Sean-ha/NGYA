using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HealthSystem : MonoBehaviour
{
	public static HealthSystem instance;

	public SpriteRenderer healthOutline;
	public SpriteRenderer healthBar;
	public SpriteRenderer whiteBar;

	// Each upgrade increase max health by 25 (outline width increases by 0.5)
	private int healthUpgradeCount;
	private float maxHealth = 50f;
	private float currentHealth;

	private int whiteBarTweenId;

	private const float OUTLINE_BAR_DIFF = 0.07f;

	private void Awake()
	{
		instance = this;

		UpdateMaxBar();

		currentHealth = maxHealth;

		UpdateCurrentBar();
	}

	public void TakeDamage(float damage)
	{
		DOTween.Kill(gameObject);
		currentHealth -= damage;

		// Handles white bar
		float oldCurrHealth = healthBar.size.x;
		UpdateCurrentBar();
		float newCurrHealth = healthBar.size.x;

		float whiteBarPosition = healthBar.transform.position.x - newCurrHealth * 2;
		whiteBar.transform.position = new Vector2(whiteBarPosition, whiteBar.transform.position.y);
		float newOldDiff = oldCurrHealth - newCurrHealth;
		whiteBar.size = new Vector2(newOldDiff, whiteBar.size.y);

		DOTween.To(() => newOldDiff, (float val) => whiteBar.size = new Vector2(val, whiteBar.size.y), 0, 1).SetEase(Ease.OutQuad).target = gameObject;

		if (currentHealth <= 0)
		{
			// Die
		}
	}

	// Update current health bar (red part)
	private void UpdateCurrentBar()
	{
		float newSize = Mathf.Max(0, (currentHealth / maxHealth) * healthOutline.size.x - OUTLINE_BAR_DIFF);

		healthBar.size = new Vector2(newSize, healthBar.size.y);
	}

	// Update max health bar (outline part)
	private void UpdateMaxBar()
	{
		maxHealth = 50 + 25 * healthUpgradeCount;
		healthOutline.size = new Vector2(1 + 0.5f * healthUpgradeCount, healthOutline.size.y);
	}
}
