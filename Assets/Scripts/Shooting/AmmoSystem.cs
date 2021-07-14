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

	// Each ammo upgrade gives 20 additional max ammo
	private int ammoUpgrades;

	private int maxAmmo;
	private int currentAmmo;
	private Stack<GameObject> ammoStack = new Stack<GameObject>();

	// Amount of ammo to get per second
	private int ammoRegen = 60;
	private float ammoRegenRate;
	private float currentAmmoRegen;

	private void Awake()
	{
		instance = this;

		ammoRegenRate = 1f / ammoRegen;
		maxAmmo = 20 + 20 * ammoUpgrades;

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
		ammoOutline.size = new Vector2(1 + ammoUpgrades, ammoOutline.size.y);
		ammoBackground.size = new Vector2(Mathf.FloorToInt(ammoOutline.size.x) - 0.04f, ammoBackground.size.y);
	}

	public bool RemoveAmmo()
	{
		if (RemoveAmmo(1) == 1)
			return true;
		else
			return false;
	}

	// Returns the number of ammo removed
	private int RemoveAmmo(int amount)
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
		currentAmmoRegen -= Time.deltaTime;
		if (currentAmmo < maxAmmo)
		{
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
					// gain an ammo
					currentAmmo++;
					GameObject bulletUI = Instantiate(bulletTemplate, new Vector2(firstBulletPosition + 0.0935f * (currentAmmo - 1), bulletTemplate.transform.position.y),
						Quaternion.identity);
					ammoStack.Push(bulletUI);
				}
				currentAmmoRegen += ammoRegenRate;
			}
		}
	}
}
