using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoSystem : MonoBehaviour
{
	public static AmmoSystem instance;

	public GameObject bulletTemplate;
	public SpriteRenderer ammoOutline;
	public SpriteRenderer ammoBackground;

	private float firstBulletPosition;

	// Each ammo upgrade gives 10 additional max ammo
	private int ammoUpgrades;
	public int AmmoUpgrades 
	{
		get { return ammoUpgrades; }
		set { ammoUpgrades = value; CalculateMaxAmmo(); UpdateMaxAmmoBar(); FillCurrentAmmo(); }
	}

	private int maxAmmo;
	private int currentAmmo;
	private Stack<GameObject> ammoStack = new Stack<GameObject>();

	// Amount of ammo to get per second
	private int ammoRegenPerSecond = 3;

	private float[] ammoOutlineWidths = { 1.02f, 1.52f, 1.98f, 2.45f, 2.9f, 3.37f, 3.85f, 4.32f, 4.78f, 5.26f, 5.73f, 6.19f, 6.66f, 7.12f, 7.59f };

	public int AmmoRegenPerSecond
	{
		get { return ammoRegenPerSecond; }
		set { ammoRegenPerSecond = Mathf.Max(1, value); CalculateAmmoRegenRate(); }
	}

	private float ammoRegenRate;
	private float currentAmmoRegen;

	private void Awake()
	{
		instance = this;

		CalculateAmmoRegenRate();
		CalculateMaxAmmo();

		firstBulletPosition = bulletTemplate.transform.position.x;

		for (int i = 0; i < maxAmmo; i++)
		{
			GameObject bulletUI = Instantiate(bulletTemplate, bulletTemplate.transform.position, Quaternion.identity);

			Vector2 pos = new Vector2(
			 Mathf.Round((bulletUI.transform.position.x + 0.09375f * i) * 32) / 32,
			 bulletTemplate.transform.position.y
			);

			bulletUI.transform.position = pos;
			ammoStack.Push(bulletUI);
		}
		bulletTemplate.transform.position = new Vector2(100, bulletTemplate.transform.position.y);
		currentAmmo = maxAmmo;

		UpdateMaxAmmoBar();
	}

	// Updates max ammo (outline)
	private void UpdateMaxAmmoBar()
	{
		ammoOutline.size = new Vector2(ammoOutlineWidths[ammoUpgrades], ammoOutline.size.y);
		ammoBackground.size = new Vector2(ammoOutlineWidths[ammoUpgrades] - 0.04f, ammoBackground.size.y);
	}

	/// <summary>
	/// Removes the specified number of ammo. Returns whether or not the ammo system has at least 'amount' number of bullets currently
	/// </summary>
	public bool RemoveAmmo(int amount)
	{
		if (currentAmmo < amount)
			return false;

		for (int i = 0; i < amount; i++)
		{
			Destroy(ammoStack.Pop());
		}
		currentAmmo -= amount;
		return true;
	}

	[Obsolete]
	// Returns the number of ammo removed
	private int TryRemoveAmmo(int amount)
	{
		if (currentAmmo < amount)
		{
			amount = currentAmmo;
		}

		for (int i = 0; i < amount; i++)
		{
			Destroy(ammoStack.Pop());
		}
		currentAmmo -= amount;

		return amount;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.O))
		{
			AmmoRegenPerSecond += 2;
		}

		if (currentAmmo < maxAmmo)
		{
			currentAmmoRegen -= Time.deltaTime;
			if (currentAmmoRegen <= 0)
			{
				int regenCount = 1;

				// Multiple ammo in a single frame
				if (-currentAmmoRegen / ammoRegenRate > 2)
				{
					currentAmmoRegen = -currentAmmoRegen;
					regenCount = Mathf.RoundToInt(currentAmmoRegen / ammoRegenRate);
					currentAmmoRegen %= ammoRegenRate;
				}

				if (currentAmmo + regenCount > maxAmmo)
					regenCount = maxAmmo - currentAmmo;

				for (int i = 0; i < regenCount; i++)
				{
					RegenerateBullet();
				}
				currentAmmoRegen += ammoRegenRate;
			}
		}
	}

	private void RegenerateBullet()
	{
		// gain an ammo
		currentAmmo++;
		GameObject bulletUI = Instantiate(bulletTemplate, new Vector2(firstBulletPosition + 0.0935f * (currentAmmo - 1), bulletTemplate.transform.position.y),
			Quaternion.identity);
		ammoStack.Push(bulletUI);
	}

	public void RegenerateBullet(int amount)
	{
		for (int i = 0; i < amount; i++)
			RegenerateBullet();
	}

	private void CalculateAmmoRegenRate()
	{
		ammoRegenRate = 1f / ammoRegenPerSecond;
	}

	private void CalculateMaxAmmo()
	{
		maxAmmo = 20 + 10 * ammoUpgrades;
	}

	public void FillCurrentAmmo()
	{
		while(currentAmmo < maxAmmo)
		{
			RegenerateBullet();
		}
	}
}
