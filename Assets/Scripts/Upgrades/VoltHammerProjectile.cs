using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class VoltHammerProjectile : MonoBehaviour
{
	private float damage;
	private bool canCrit;
	private bool enableOnHitEffects;

	// If true, zap cycles are checked
	private bool projectileEnabled;

	private Rigidbody2D rb;
	private Collider2D col;
	private ShootManager sm;

	private HashSet<GameObject> hitSet = new HashSet<GameObject>();
	private Queue<Collider2D> hitQueue = new Queue<Collider2D>();

	private void Awake()
	{
		col = GetComponent<Collider2D>();
	}

	private void Start()
	{
		sm = ShootManager.instance;
		StartCoroutine(HitQueueChecker());
		StartCoroutine(HitSetClearer());
	}

	private IEnumerator HitQueueChecker()
	{
		while(true)
		{
			if (projectileEnabled && hitQueue.Count > 0)
			{
				// Max number of enemies to hit per "zap cycle"
				int hitCount = 7;
				for (int i = 0; i < hitCount; i++)
				{
					if (hitQueue.Count > 0)
						ZapEnemy(hitQueue.Dequeue());
					else
						break;
				}
				ObjectPooler.instance.CreateCircleHitEffect(Color.white, transform.position, 1.5f);
				CameraShake.instance.ShakeCamera(0.1f, 0.1f);

				yield return new WaitForSeconds(0.08f);
			}
			else
				yield return null;
		}
	}

	private IEnumerator HitSetClearer()
	{
		while(true)
		{
			yield return new WaitForSeconds(0.6f);
			hitSet.Clear();
			col.enabled = false;
			col.enabled = true;
		}
	}

	private void ZapEnemy(Collider2D collision)
	{
		if (collision == null)
			return;

		if (enableOnHitEffects)
			sm.OnMainProjectileHitEnemy(transform, collision.transform, collision);

		ObjectPooler.instance.CreateElectricity(transform.position, collision.transform.position);
		collision.GetComponent<EnemyHealth>().TakeDamage(damage, canCrit: canCrit);
		ObjectPooler.instance.CreateHitParticles(Color.white, collision.transform.position);
		ObjectPooler.instance.CreateCircleHitEffect(Color.white, collision.transform.position, 1f);
	}

	public void SetProjectile(float damage, float speed, float dangle, bool canCrit, bool enableOnHitEffects)
	{
		rb = GetComponent<Rigidbody2D>();
		this.damage = damage;
		
		float angle = Mathf.Deg2Rad * dangle;
		float distance = 50f;
		this.canCrit = canCrit;
		this.enableOnHitEffects = enableOnHitEffects;

		Vector2 destination = (Vector2)transform.position + (new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance);
		float timeToReachFinal = distance / speed;

		// Play summon animation
		ObjectPooler.instance.CreateCircleHitEffect(Color.white, transform.position, 2f);
		ObjectPooler.instance.CreateElectricity(transform.position, transform.position + new Vector3(0, 2f, 0f));

		DOVirtual.DelayedCall(0.3f, () =>
		{
			projectileEnabled = true;
			if (dangle <= 90 && dangle >= -90)
			{
				GetComponent<Animator>().Play("VoltHammerSpinCW");
			}
			else
			{
				GetComponent<Animator>().Play("VoltHammerSpinCCW");
			}
			rb.DOMove(destination, timeToReachFinal).SetUpdate(UpdateType.Fixed);
		});
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		// Enemy layer
		if (collision.gameObject.layer == 7)
		{
			// Do not collide with enemy projectiles
			if (collision.gameObject.CompareTag("Projectile"))
				return;

			if (!hitSet.Contains(collision.gameObject))
			{
				hitQueue.Enqueue(collision);
				hitSet.Add(collision.gameObject);
			}
		}
	}
}
