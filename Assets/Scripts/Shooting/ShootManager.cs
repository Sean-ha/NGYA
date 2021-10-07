using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

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


	#region UPGRADE_VARS
	// UPGRADE VARIABLES
	public float tendrilChance;
	public float tendrilDamage;

	public float vultureClawChance;
	public int vultureClawAmount;

	#endregion

	// Call whenever a projectile hits an enemy to invoke on hit effects
	public void OnProjectileHitEnemy(Transform projectile, Transform enemy)
	{
		if (MyRandom.RollProbability(tendrilChance))
		{
			ObjectPooler.instance.CreateTendril(PlayerController.instance.transform.position, enemy.position);
			enemy.GetComponent<EnemyHealth>().TakeDamage(tendrilDamage);
		}
	}

	// Call whenever an enemy dies to invoke on death effects
	public void OnProjectileKillEnemy(Transform enemy)
	{
		if (MyRandom.RollProbability(vultureClawChance))
		{
			AmmoSystem.instance.RegenerateBullet(vultureClawAmount);
		}
	}
}
