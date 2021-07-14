using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Basic script for an enemy that just shoots a projectile in regular intervals
public class EnemyDotShooter : MonoBehaviour
{
	[Tooltip("Time between shots in seconds")]
	public float shotCooldown;
	[Tooltip("Speed of shots")]
	public float shotSpeed;
	[Tooltip("Maximum distance shot can travel")]
	public float bulletDistance;
	public float damage;

	private void Start()
	{
		StartCoroutine(ShootingBehavior());
	}

	private IEnumerator ShootingBehavior()
	{
		while (true)
		{
			yield return new WaitForSeconds(shotCooldown);
			Shoot();
		}
	}

	// Get angle in degrees that the current dot is at relative to its body
	private float GetAngle()
	{
		Vector2 diff = (Vector2)transform.position - (Vector2)transform.parent.position;

		return Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
	}

	public void Shoot()
	{
		// Get angle between player and mouse location
		float angle = GetAngle();

		LeanTween.scale(gameObject, new Vector3(0.6f, 0.6f, 1), 0.05f).setOnComplete(() =>
		{
			LeanTween.scale(gameObject, new Vector3(0.2f, 0.2f, 1), 0.05f);
		});
		GameObject proj = ObjectPooler.instance.Create(Tag.EnemyProjectile, transform.position, Quaternion.AngleAxis(angle, Vector3.forward));
		proj.transform.GetComponentInChildren<BasicEnemyProjectile>().SetProjectile(shotSpeed, angle, damage, bulletDistance);
	}
}
