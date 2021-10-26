using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using NaughtyAttributes;

public class ShootManager : MonoBehaviour
{
	public static ShootManager instance;

	public Transform dotHolder;

	public float damage;

	public float bulletDistance = 6.5f;
	public float BulletDistance 
	{ 
		get { return bulletDistance; }
		set { bulletDistance = Mathf.Max(3.5f, value); }
	}

	public int pierceCount;

	private float shootCooldown = 0.25f;
	public float ShootCooldown
	{
		get { return shootCooldown; }
		set { shootCooldown = Mathf.Max(0.05f, value); }
	}

	public float critChance { get; set; } = .6f;
	// Percentage damage increase. E.g. 0.5 means crits deal 50% more damage, or 1.5x dmg
	public float critDamage { get; set; } = 0.5f;

	private float currShootCooldown;

	// Parameters: damage, distance, pierce
	public UnityEvent<float, float, int> onShoot { get; set; } = new UnityEvent<float, float, int>();

	private void Awake()
	{
		instance = this;
		foreach (Transform child in dotHolder)
		{
			Shooter currShooter = child.GetComponent<Shooter>();
			if (currShooter != null)
				onShoot.AddListener(currShooter.Shoot);
		}
	}

	private void Update()
	{
		if (TimeManager.IsPaused) return;

		currShootCooldown -= Time.deltaTime;

		if (Input.GetMouseButton(0))
		{
			if (currShootCooldown <= 0)
			{
				CursorManager.instance.AnimateCursorShoot();
				onShoot.Invoke(damage, bulletDistance, pierceCount);
				currShootCooldown = shootCooldown;
			}
		}
	}

	// Note: DM stands for "Damage Multiplier" :)
	#region UPGRADE_VARS
	// UPGRADE VARIABLES
	[HideInInspector] public float tendrilChance;
	[HideInInspector] public float tendrilDM;

	[HideInInspector] public float bustedToasterChance;
	[HideInInspector] public float bustedToasterDM;
	public GameObject bustedToasterExplosion;

	#endregion

	// Call whenever a projectile hits an enemy to invoke on hit effects
	public void OnProjectileHitEnemy(Transform projectile, Transform enemy, Collider2D collider)
	{
		if (MyRandom.RollProbability(tendrilChance))
		{
			ObjectPooler.instance.CreateTendril(PlayerController.instance.transform.position, enemy.position);
			enemy.GetComponent<EnemyHealth>().TakeDamage(damage * tendrilDM);
		}
		if (MyRandom.RollProbability(bustedToasterChance))
		{
			GameObject expl = Instantiate(bustedToasterExplosion, enemy.position, Quaternion.identity);
			expl.GetComponent<BustedToasterExplosion>().ActivateExplosion(damage * bustedToasterDM);
			// TODO: SFX here
		}
		
		
	}


	[HideInInspector] public float vultureClawChance;
	[HideInInspector] public int vultureClawAmount;

	// Call whenever an enemy dies to invoke on death effects
	public void OnProjectileKillEnemy(Transform enemy)
	{
		if (MyRandom.RollProbability(vultureClawChance))
		{
			AmmoSystem.instance.RegenerateBullet(vultureClawAmount);
		}
	}

	// Called from the enemy that was critted
	public void OnCrit(Transform enemy, Collider2D collider)
	{
		// TODO: Crit fx
		Vector2 topOfEnemy = (Vector2)collider.bounds.center + new Vector2(0, collider.bounds.extents.y + 0.35f);
		ObjectPooler.instance.Create(Tag.CritText, topOfEnemy, Quaternion.identity);
	}

	[HideInInspector] public int lastRegardsBulletCount;
	[HideInInspector] public float lastRegardsDM;

	// 2 arguments: player transform and the transform of the thing that hit you. Can be an enemy or a projectile or whatever
	public void OnTakeDamage(Transform player, Transform enemy)
	{
		if (lastRegardsBulletCount != 0)
		{
			// Opposite direction of enemy from player
			Vector2 dir = player.position - enemy.position;
			StartCoroutine(LastRegardsShoot(dir));
		}
	}


	private IEnumerator LastRegardsShoot(Vector2 direction)
	{
		int lastRegardsUpgradeCount = lastRegardsBulletCount / 8;
		for (int i = 0; i < lastRegardsBulletCount; i++)
		{
			Vector2 randomDir = Quaternion.AngleAxis(Random.Range(-40f, 40f), Vector3.forward) * direction;
			ObjectPooler.instance.CreateHomingProjectile(PlayerController.instance.transform.position, Quaternion.identity, damage, true,
			randomDir, 0.02f);
			yield return new WaitForSeconds(Random.Range(0.04f, 0.1f) / lastRegardsUpgradeCount);
		}
	}
}
