using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;

// Doesn't actually do any shooting by itself. Only purpose is to look like a dot shooter.
public class EnemyDotShooterFake : MonoBehaviour
{
	public float damage;

	private Vector2 originalSize;

	private void Awake()
	{
		originalSize = transform.localScale;
	}
}
