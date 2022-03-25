using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoSystem : MonoBehaviour
{
	public static AmmoSystem instance;

	public SpriteRenderer ammoOutline;
	public SpriteRenderer ammoBackground;
	public SpriteRenderer ammoBar;

	private float firstBulletPosition;

	// Each ammo upgrade gives 5 additional max ammo
	private int ammoUpgradeCount;
	public int AmmoUpgradeCount 
	{
		get { return ammoUpgradeCount; }
		set { ammoUpgradeCount = value; UpdateMaxAmmoBar(); FillCurrentAmmo(); }
	}

	private int maxAmmo;
	private float currentAmmo;

	// Amount of ammo to get per second
	private float ammoRegenPerSecond = 4f;

	private float[] ammoOutlineWidths = { 1.02f, 1.52f, 1.98f, 2.45f, 2.9f, 3.37f, 3.85f, 4.32f, 4.78f, 5.26f, 5.73f, 6.19f, 6.66f, 7.12f, 7.59f };

	public float AmmoRegenPerSecond
	{
		get { return ammoRegenPerSecond; }
		set { ammoRegenPerSecond = Mathf.Max(1, value); CalculateAmmoRegenRate(); }
	}

	// Amount of ammo to get per frame
	private float ammoRegenRate;

	// Spells cost 100% of their cost
	public float ammoCostPercentage { get; set; } = 1f;

	private const float OUTLINE_BAR_DIFF = 0.07f;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		CalculateAmmoRegenRate();

		UpdateMaxAmmoBar();
		FillCurrentAmmo();
	}

	// Updates max ammo (outline)
	private void UpdateMaxAmmoBar()
	{
		maxAmmo = 50 + 5 * ammoUpgradeCount;
		float ySize = 1 + 0.1f * ammoUpgradeCount;
		ammoBackground.size = new Vector2(ySize - 0.07f, ammoBackground.size.y);
		ammoOutline.size = new Vector2(ySize, ammoOutline.size.y);
	}

	/// <summary>
	/// Removes the specified number of ammo. Returns whether or not the ammo system has at least 'amount' number of bullets currently
	/// </summary>
	public bool RemoveAmmo(float amount)
	{
		float newAmount = amount * ammoCostPercentage;

		if (currentAmmo < newAmount)
			return false;

		currentAmmo -= newAmount;
		return true;
	}

	private void Update()
	{
#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.O))
		{
			AmmoRegenPerSecond += 2;
		}
#endif

		currentAmmo = Mathf.Clamp(currentAmmo + ammoRegenRate, 0f, maxAmmo);

		// Update current ammo UI every frame
		UpdateAmmoUI();
	}

	private void UpdateAmmoUI()
	{
		float newSize = Mathf.Max(0, (currentAmmo / maxAmmo) * ammoOutline.size.x - OUTLINE_BAR_DIFF);

		ammoBar.size = new Vector2(newSize, ammoBar.size.y);
	}

	private void CalculateAmmoRegenRate()
	{
		ammoRegenRate = AmmoRegenPerSecond / Application.targetFrameRate;
	}

	public void FillCurrentAmmo()
	{
		currentAmmo = maxAmmo;

		UpdateAmmoUI();
	}
}
