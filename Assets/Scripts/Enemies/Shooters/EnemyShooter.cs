using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public abstract class EnemyShooter : MonoBehaviour
{
	[Tooltip("Speed of shots")]
	public float shotSpeed;
	[Tooltip("Maximum distance shot can travel")]
	public float bulletDistance;
	public float damage;

	public SoundManager.Sound shootSound;

	public UnityEvent onShoot;

	private Vector2 originalSize;

	protected void Awake()
	{
		originalSize = transform.localScale;
		OnAwake();

		onShoot.AddListener(() => SoundManager.instance.PlaySound(shootSound));
	}

	protected virtual void OnAwake()
	{
	}

	/// <summary>
	/// Get angle in degrees that the current dot is at relative to its body
	/// </summary>
	protected float GetAngle()
	{
		Vector2 diff = (Vector2)transform.position - (Vector2)transform.parent.position;

		return Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
	}

	public void Scale()
	{
		transform.localScale = new Vector3(originalSize.x + 0.4f, originalSize.y + 0.4f, 1);
		transform.DOScale(new Vector3(originalSize.x, originalSize.y, 1), 0.1f);
	}

	public abstract void Shoot();
}
