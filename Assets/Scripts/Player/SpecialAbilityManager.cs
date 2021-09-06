using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpecialAbilityManager : MonoBehaviour
{
	public static SpecialAbilityManager instance;
	public Abilities ability;

	private UnityEvent onAbilityCast = new UnityEvent();
	private ShootManager shootManager;

	private float abilityCooldown;
	private float currCooldown;

	public enum Abilities
	{
		Unnamed,
	}

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		shootManager = ShootManager.instance;
		AddAbilityListener();
		onAbilityCast.AddListener(() => currCooldown = abilityCooldown);
	}

	private void Update()
	{
		if (currCooldown <= 0)
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				onAbilityCast.Invoke();
			}
		}
		else
		{
			currCooldown -= Time.deltaTime;
		}
	}

	private void AddAbilityListener()
	{
		if (ability == Abilities.Unnamed)
		{
			abilityCooldown = 12f;
			onAbilityCast.AddListener(UnnamedAbility);
		}
	}

	public float GetCooldownFraction()
	{
		float ratio = currCooldown / abilityCooldown;
		if (ratio >= 1) return 1;
		else if (ratio <= 0) return 0;
		else return ratio;
	}

	// Angle in degrees
	private float GetAngleTowardsMouse()
	{
		// Get angle towards mouse
		Vector2 pos = Input.mousePosition;
		Vector2 worldPos = Camera.main.ScreenToWorldPoint(pos);

		// Get angle between buddy and mouse location
		Vector2 diff = worldPos - (Vector2)transform.position;

		float angle = Mathf.Rad2Deg * Mathf.Atan2(diff.y, diff.x);
		return angle;
	}

	// Shoot a shotgun blast
	public void UnnamedAbility()
	{
		SoundManager.instance.PlaySound(SoundManager.Sound.BigShoot);
		float dangle = GetAngleTowardsMouse();

		int bulletCount = 14;

		for (int i = 0; i < bulletCount; i++)
		{
			float thisDangle = dangle + Random.Range(-30f, 30f);

			GameObject proj = ObjectPooler.instance.Create(Tag.PlayerProjectile, transform.position, Quaternion.AngleAxis(thisDangle, Vector3.forward));
			proj.GetComponent<BasicProjectile>().SetProjectile(Random.Range(15f, 20f), thisDangle, shootManager.damage / 2, shootManager.pierceCount + 2, 
				shootManager.BulletDistance + Random.Range(-0.6f, 0.6f));
		}
	}
}
