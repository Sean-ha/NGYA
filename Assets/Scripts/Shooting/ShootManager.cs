using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using NaughtyAttributes;
using System;
using Random = UnityEngine.Random;

public class ShootManager : MonoBehaviour
{
	public static ShootManager instance;

	public Transform dotHolder;

	public float damage;
	public float damageMultiplier = 1f;

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

	// Amount of ammo to consume per shot
	public float ammoPerShot;

	public float critChance { get; set; } = .05f;
	// Percentage damage increase. E.g. 0.5 means crits deal 50% more damage, or 1.5x dmg
	public float critDamage { get; set; } = 0.5f;

	private float currShootCooldown;

	// Parameters: damage, distance, pierce
	public UnityEvent<float, float, int> onShoot { get; set; } = new UnityEvent<float, float, int>();

	private PlayerController pc;
	private HealthSystem hs;

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

	private void Start()
	{
		pc = PlayerController.instance;
		hs = HealthSystem.instance;
	}

	private void Update()
	{
		if (TimeManager.IsPaused) return;

		currShootCooldown -= Time.deltaTime;

		if (Input.GetMouseButton(0))
		{
			if (currShootCooldown <= 0)
			{
				if (!AmmoSystem.instance.RemoveAmmo(1)) // TODO: replace with ammoPerShot
				{
					SoundManager.instance.PlaySound(SoundManager.Sound.FailShoot);
					return;
				}

				CursorManager.instance.AnimateCursorShoot();
				onShoot.Invoke(damage, bulletDistance, pierceCount);
				currShootCooldown = shootCooldown;
			}
		}
	}

	// Note: DM stands for "Damage Multiplier" :)
	#region UPGRADE_VARS
	// UPGRADE VARIABLES
	[NonSerialized] public float abyssalHeadChance;
	[NonSerialized] public float abyssalHeadDM;

	[NonSerialized] public float bustedToasterChance;
	[NonSerialized] public float bustedToasterDM;
	public GameObject bustedToasterExplosion;

	[NonSerialized] public float sinisterCharmChance;
	[NonSerialized] public float sinisterCharmDM;
	[SerializeField] private GameObject sinisterCharmStarter;
	[SerializeField] private GameObject sinisterCharmProjectile;

	[NonSerialized] public float bananaChance;
	[NonSerialized] public float bananaDM;

	[NonSerialized] public float scissorsHealthPercent;
	[NonSerialized] public float scissorsCritChanceAddition;

	[NonSerialized] public float cloakedDaggerDM;

	[NonSerialized] public float thunderWandChance;
	[NonSerialized] public float thunderWandDM;

	[NonSerialized] public float weirdEyeballChance;
	[NonSerialized] public float weirdEyeballDM;
	[SerializeField] private GameObject weirdEyeballProjectile;

	[NonSerialized] public float moonlightScytheChance;
	[NonSerialized] public float moonlightScytheDM;
	[SerializeField] private GameObject moonlightScytheProjectile;

	[NonSerialized] public float voltHammerChance;
	[NonSerialized] public float voltHammerDM;
	[SerializeField] private GameObject voltHammerProjectile;
	#endregion

	// Call whenever a main player projectile (DIRECTLY shot by player) hits an enemy to invoke on hit effects
	public void OnMainProjectileHitEnemy(Transform projectile, Transform enemy, Collider2D collider)
	{
		if (MyRandom.RollProbability(abyssalHeadChance))
		{
			DOVirtual.DelayedCall(0.1f, () =>
			{
				if (enemy != null)
				{
					ObjectPooler.instance.CreateTendril(pc.transform.position, enemy.position);
					enemy.GetComponent<EnemyHealth>().TakeDamage(damage * abyssalHeadDM);
					SoundManager.instance.PlaySound(SoundManager.Sound.PaperRip);
				}
			}, ignoreTimeScale: false);
		}
		if (MyRandom.RollProbability(bustedToasterChance))
		{
			GameObject expl = Instantiate(bustedToasterExplosion, enemy.position, Quaternion.identity);
			expl.GetComponent<Explosion>().ActivateExplosion(damage * bustedToasterDM, true);
			ObjectPooler.instance.CreateExpandingExplosion(enemy.position, Color.white, 1f);
			SoundManager.instance.PlaySound(SoundManager.Sound.Explosion2);
		}
		if (MyRandom.RollProbability(sinisterCharmChance))
		{
			Vector2 pos = pc.GetRandomNearbyPosition(minDistance: 1.8f, maxDistance: 3f);
			GameObject charmStarter = Instantiate(sinisterCharmStarter, pos, Quaternion.identity);
			SoundManager.instance.PlaySound(SoundManager.Sound.PageTurn);

			Sequence seq = DOTween.Sequence();
			seq.AppendInterval(0.6f);
			seq.AppendCallback(() =>
			{
				Destroy(charmStarter);
				Vector2 target = SpawnManager.instance.GetRandomEnemy().position;
				if (target == null) return;

				float dAngle = HelperFunctions.GetDAngleTowards(pos, target);
				GameObject charmProj = Instantiate(sinisterCharmProjectile, pos, Quaternion.AngleAxis(dAngle, Vector3.forward));
				charmProj.GetComponent<BasicProjectile>().SetProjectile(19, dAngle, 
					damage * sinisterCharmDM, 1, 40f, true, true);
				SoundManager.instance.PlaySound(SoundManager.Sound.Whoosh, volumeDelta: -0.1f);
			});

			seq.Play();
		}
		if (MyRandom.RollProbability(bananaChance))
		{
			Vector2 direction = pc.transform.position - enemy.position;
			Vector2 randomDir = Quaternion.AngleAxis(Random.Range(-40f, 40f), Vector3.forward) * direction;
			ObjectPooler.instance.CreateHomingProjectile(pc.transform.position, Quaternion.identity, damage * bananaDM, true,
			randomDir, banana: true);
			// TODO: Adjust this sound
			SoundManager.instance.PlaySound(SoundManager.Sound.TestSound);
		}
		if (MyRandom.RollProbability(thunderWandChance))
		{
			HashSet<Transform> targets = SpawnManager.instance.GetRandomEnemies(15);

			CameraShake.instance.ShakeCamera(0.3f, 0.4f);
			SoundManager.instance.PlaySound(SoundManager.Sound.Thunder);
			Transform curr = enemy;
			foreach (Transform target in targets)
			{
				ObjectPooler.instance.CreateElectricity(curr.position, target.position);
				curr = target;
				target.GetComponent<EnemyHealth>().TakeDamage(damage * thunderWandDM, true);
			}
		}
		if (MyRandom.RollProbability(weirdEyeballChance))
		{
			Vector2 pos = pc.GetRandomNearbyPosition(minDistance: 1.6f, maxDistance: 3f);
			GameObject proj = Instantiate(weirdEyeballProjectile, pos, Quaternion.identity);
			proj.GetComponent<WeirdEyeball>().Setup(damage * weirdEyeballDM);
		}
		if (MyRandom.RollProbability(moonlightScytheChance))
		{
			GameObject scythe = Instantiate(moonlightScytheProjectile, enemy.position, Quaternion.identity);
			scythe.GetComponent<CollideWithEnemy>().damage = damage * moonlightScytheDM;
		}
		if (MyRandom.RollProbability(voltHammerChance))
		{
			Vector2 pos = pc.GetRandomNearbyPosition(minDistance: 1.6f, maxDistance: 3f);
			GameObject voltHammer = Instantiate(voltHammerProjectile, pos, Quaternion.identity);
			float dangle = HelperFunctions.GetDAngleTowards(voltHammer.transform.position, enemy.position);
			voltHammer.GetComponent<VoltHammerProjectile>().SetProjectile(damage * voltHammerDM, 10f, dangle, true, false);
		}
	}

	// NOTE: ShotCount means the number of shots for this upgrade to activate
	[NonSerialized] public int jumboGeorgeShotCount;
	[NonSerialized] public int jumboGeorgeShotCountCurr;
	[NonSerialized] public float jumboGeorgeDM;
	[SerializeField] private GameObject jumboGeorgeProjectile;

	// Call whenever you shoot. Angle is towards mouse. Source is position of DotShooter
	public void OnShoot(float dangle, Vector2 source)
	{
		jumboGeorgeShotCountCurr++;

		if (jumboGeorgeShotCountCurr >= jumboGeorgeShotCount && jumboGeorgeShotCount != 0)
		{
			SoundManager.instance.PlaySound(SoundManager.Sound.CannonFire);
			GameObject proj = Instantiate(jumboGeorgeProjectile, source, Quaternion.identity);
			proj.GetComponent<JumboGeorgeProjectile>().SetProjectile(8f, dangle, damage * jumboGeorgeDM, 12f, true, false);
			jumboGeorgeShotCountCurr = 0;
		}
	}


	[NonSerialized] public float vultureClawChance;
	[NonSerialized] public int vultureClawAmount;

	[NonSerialized] public int starFragmentCount = 0;
	[NonSerialized] public float starFragmentDM;

	// Call whenever an enemy dies to invoke on death effects
	public void OnProjectileKillEnemy(Transform enemy)
	{
		if (MyRandom.RollProbability(vultureClawChance))
		{
			AmmoSystem.instance.RegenerateBullet(vultureClawAmount);
		}
		if (starFragmentCount > 0)
		{
			float startDangle = Random.Range(0f, 90f);
			float angleDiff = 360f / starFragmentCount;
			for (int i = 0; i < starFragmentCount; i++)
			{
				float currDangle = startDangle + angleDiff * i;
				ObjectPooler.instance.CreatePlayerProjectile(enemy.position, currDangle, HelperFunctions.shotSpeed,
					damage * starFragmentDM, pierceCount, bulletDistance, true, false);
			}
		}
	}

	[NonSerialized] public float healthRestorePerCrit;
	// Called from the enemy that was critted
	public void OnCrit(Transform enemy, Collider2D collider)
	{
		hs.RestoreHealth(healthRestorePerCrit);

		// TODO: Crit fx
	}

	[NonSerialized] public int lastRegardsBulletCount;
	[NonSerialized] public float lastRegardsDM;

	// 2 arguments: player transform and the transform of the thing that hit you (can be an enemy or a projectile or whatever)
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
		float totalShootTime = 0.8f;
		int numberOfWaves = 8;
		int shotsPerWave = Mathf.RoundToInt((float)lastRegardsBulletCount / numberOfWaves);
		int shotsOnLastWave = lastRegardsBulletCount - (shotsPerWave * (numberOfWaves - 1));
		int counter = 0;
		for (int i = 0; i < numberOfWaves; i++)
		{
			int shotsOnThisWave;
			if (i == numberOfWaves - 1)
				shotsOnThisWave = shotsOnLastWave;
			else
				shotsOnThisWave = shotsPerWave;

			for (int j = 0; j < shotsOnThisWave; j++)
			{
				counter++;
				Vector2 randomDir = Quaternion.AngleAxis(Random.Range(-40f, 40f), Vector3.forward) * direction;
				ObjectPooler.instance.CreateHomingProjectile(pc.transform.position, Quaternion.identity, damage * lastRegardsDM, true,
				randomDir);
			}

			SoundManager.instance.PlaySound(SoundManager.Sound.TestSound);
			yield return new WaitForSeconds(totalShootTime / numberOfWaves);
		}
	}
}
