using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HomingProjectile : MonoBehaviour
{
   private Rigidbody2D rb;
   private Transform target;

   private float speed;
   private float rotateSpeed = 300;

   private float horizontal;
   private float vertical;
   private float time;
   private int random;

   // Whether or not the bullet should home in on target or not
   private bool active;
   // Bullet starts off unable to hit walls, but after about 2secs, wall collisions are enabled again
   private bool canHitWalls;

   private Coroutine behaviorCR;

   private Tween t1, t2;

   private void Awake()
   {
      rb = GetComponent<Rigidbody2D>();
      // target = GameObject.FindGameObjectWithTag("Player").transform;
   }

	private void OnEnable()
   {
      speed = 14;
      time = 0;
      canHitWalls = false;

      target = SpawnManager.instance.GetRandomEnemy();

      behaviorCR = StartCoroutine(BulletBehavior());
   }

	private void OnDisable()
	{
      if (behaviorCR != null)
         StopCoroutine(behaviorCR);
	}

	/*
   private void FixedUpdate()
   {
      time += Time.fixedDeltaTime;
      if (active)
      {
         // speed += .8f;
         FollowTarget();
      }
      else
      {
         rb.velocity = new Vector2(horizontal, vertical);

         float angle = Mathf.Atan2(vertical, horizontal) * Mathf.Rad2Deg;

         if (random == 0)
         {
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
         }
         else
         {
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.back);
         }

         if (time >= 0.15f)
         {
            active = true;
         }
      }
   }
   */

   // Call this to disable the bullet and have it travel in a given direction for 'time' seconds, and then reactivate.
   public void ActivateBullet(Vector2 direction, float timeToEnable)
	{
      active = false;
      horizontal = direction.x;
      vertical = direction.y;
      CheckRotation();

      t1 = DOVirtual.DelayedCall(timeToEnable, () => active = true, false);
      t2 = DOVirtual.DelayedCall(2f, () => canHitWalls = true, false);
	}

	// Acts like a "FixedUpdate" for this bullet but in Coroutine form
	private IEnumerator BulletBehavior()
	{
      while (true)
		{
         time += Time.fixedDeltaTime;
         if (active)
         {
            if (target == null)
				{
               target = SpawnManager.instance.GetRandomEnemy();
               rb.angularVelocity = 0;
               yield return new WaitForSeconds(0.1f);
				}
            else
				{
               // speed += .8f;
               FollowTarget();
            }
         }
         else
         {
            rb.velocity = new Vector2(horizontal, vertical).normalized * speed;

            CheckRotation();
         }
         yield return new WaitForFixedUpdate();
      }
   }

   private void FollowTarget()
   {
      Vector2 direction = (Vector2)target.position - rb.position;

      // Decreases speed if velocity is near perpendicular to direction to turn to avoid orbital behavior
      /*
      if ((ang > 85 && ang < 95) || (ang > -95 && ang < -85))
      {
         speed = 6;
      }
      */

      direction.Normalize();

      float rotateAmount = Vector3.Cross(direction, transform.right).z;
      rb.angularVelocity = -rotateAmount * rotateSpeed;
      rb.velocity = transform.right * speed;
   }

   private void CheckRotation()
	{
      float angle = Mathf.Atan2(vertical, horizontal) * Mathf.Rad2Deg;

      if (random == 0)
      {
         transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
      }
      else
      {
         transform.rotation = Quaternion.AngleAxis(angle, Vector3.back);
      }
   }

	private void OnTriggerEnter2D(Collider2D collision)
	{
      if (collision.gameObject.layer == 9 && canHitWalls)
      {
         Vector2 closestPoint = collision.ClosestPoint(transform.position);

         ObjectPooler.instance.CreateHitParticles(Color.white, closestPoint);
         ObjectPooler.instance.CreateCircleHitEffect(Color.white, closestPoint, 1f);
         SoundManager.instance.PlaySound(SoundManager.Sound.EnemyHit);

         t1.Kill();
         t2.Kill();
         transform.gameObject.SetActive(false);
      }
   }
}