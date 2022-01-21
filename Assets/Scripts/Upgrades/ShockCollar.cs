using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockCollar : MonoBehaviour
{
   public float lineWidth;

   public float radius { get; private set; }
   [System.NonSerialized] public bool shockingEnabled = true;
   [System.NonSerialized] public float damageMultiplier;
   [System.NonSerialized] public float timePerShock;

   private LineRenderer lr;
   private ShootManager sm;

   private Dictionary<Collider2D, Coroutine> shockDict = new Dictionary<Collider2D, Coroutine>();

   private bool canCrit;

	private void Awake()
	{
      lr = GetComponent<LineRenderer>();
   }

	private void Start()
	{
      sm = ShootManager.instance;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		// Enemy layer
		if (collision.gameObject.layer == 7)
		{
			// Do not collide with enemy projectiles
			if (collision.gameObject.CompareTag("Projectile"))
				return;

         // Begin shocking the enemy repeatedly
         if (!shockDict.ContainsKey(collision))
			{
            Coroutine c = StartCoroutine(ShockEnemy(collision));
            shockDict.Add(collision, c);
			}
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
      // Enemy layer
      if (collision.gameObject.layer == 7)
      {
         // Do not collide with enemy projectiles
         if (collision.gameObject.CompareTag("Projectile"))
            return;

         // Stop shocking the enemy
         if (shockDict.ContainsKey(collision))
         {
            StopCoroutine(shockDict[collision]);
            shockDict.Remove(collision);
         }
      }
   }

   private IEnumerator ShockEnemy(Collider2D collision)
	{
      while(true)
		{
         if (collision != null)
			{
            ObjectPooler.instance.CreateCircleHitEffect(Color.white, collision.transform.position, 1.5f);
            ObjectPooler.instance.CreateElectricity(transform.position, collision.transform.position);
            collision.GetComponent<EnemyHealth>().TakeDamage(sm.damage * damageMultiplier, canCrit: canCrit);
            SoundManager.instance.PlaySound(SoundManager.Sound.Zap);
         }

         yield return new WaitForSeconds(timePerShock);
		}
	}

	public void EnableShockCollar(bool canCrit)
	{
      this.canCrit = canCrit;
      GetComponent<Collider2D>().enabled = true;
      StartCoroutine(DrawField());
      shockingEnabled = true;
   }

   public void SetRadius(float radius)
	{
      this.radius = radius;
      GetComponent<CircleCollider2D>().radius = radius;
	}

   private IEnumerator DrawField()
	{
      while (true)
		{
         DrawCircle(radius, lineWidth);
         yield return new WaitForSeconds(0.1f);
      }
	}

	public void DrawCircle(float radius, float lineWidth)
   {
      int segments = 60;

      lr.startWidth = lineWidth;
      lr.endWidth = lineWidth;
      lr.positionCount = segments + 1;

      int pointCount = segments + 1; // add extra point to make startpoint and endpoint the same to close the circle
      Vector3[] points = new Vector3[pointCount];

      for (int i = 0; i < pointCount; i++)
      {
         var rad = Mathf.Deg2Rad * (i * 360f / segments);
         points[i] = new Vector3(Mathf.Sin(rad) * radius, Mathf.Cos(rad) * radius, 0);
         points[i] += (Vector3)Random.insideUnitCircle * 0.1f;
      }
      points[pointCount - 1] = points[0];

      lr.SetPositions(points);
   }
}
