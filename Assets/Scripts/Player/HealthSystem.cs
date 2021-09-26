using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;

public class HealthSystem : MonoBehaviour
{
	public static HealthSystem instance;

	public SpriteRenderer healthOutline;
	public SpriteRenderer healthBar;
	public SpriteRenderer whiteBar;
	public SpriteRenderer background;

	// Each upgrade increase max health by 10 (outline width increases by 0.2)
	private int healthUpgradeCount;
	private float maxHealth = 50f;
	private float currentHealth;

	public float damageReduction { get; set; }

	private LoveBalloon balloon;

	private const float OUTLINE_BAR_DIFF = 0.07f;

	private void Awake()
	{
		instance = this;

		UpdateMaxBar();
		currentHealth = maxHealth;
		UpdateCurrentBar();
	}

	private void Start()
	{
		balloon = PlayerController.instance.transform.Find("HeartBalloon").GetComponent<LoveBalloon>();
	}

	public void TakeDamage(float damage)
	{
		damage *= (1 - damageReduction);

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

	// Update max health bar (outline and background part)
	private void UpdateMaxBar()
	{
		maxHealth = 50 + 10 * healthUpgradeCount;
		float ySize = 1 + 0.2f * healthUpgradeCount;
		background.size = new Vector2(ySize - 0.07f, background.size.y);
		healthOutline.size = new Vector2(ySize, healthOutline.size.y);
	}

	public void RestoreMaxHealth()
	{
		currentHealth = maxHealth;
		UpdateCurrentBar();
	}
}
