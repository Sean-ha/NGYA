using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
	public static HealthSystem instance;

	public SpriteRenderer healthOutline;
	public SpriteRenderer healthBar;

	// Each upgrade increase max health by 25 (outline width increases by 0.5)
	private int healthUpgradeCount = 2;
	private float maxHealth = 50f;
	private float currentHealth;

	private const float OUTLINE_BAR_DIFF = 0.07f;

	private void Awake()
	{
		instance = this;

		currentHealth = maxHealth;

		UpdateCurrentBar();
	}

	public void TakeDamage(float damage)
	{
		currentHealth -= damage;
		UpdateCurrentBar();

		if (currentHealth <= 0)
		{
			print("huh");
			print("die");
		}
	}

	// Update current health bar (red part)
	private void UpdateCurrentBar()
	{
		healthBar.size = new Vector2((currentHealth / maxHealth) * healthOutline.size.x - OUTLINE_BAR_DIFF, healthBar.size.y);
	}

	// Update max health bar (outline part)
	private void UpdateMaxBar()
	{
		healthOutline.size = new Vector2(1 + 0.5f * healthUpgradeCount, healthOutline.size.y);
	}
}
