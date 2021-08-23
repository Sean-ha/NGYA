using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoveBalloon : MonoBehaviour
{
	public bool popped { get; set; }

	private Transform loveBalloonObject;

	private void Awake()
	{
		loveBalloonObject = transform.GetChild(3);
	}

	/// <summary>
	/// Try to pop the balloon. Return true if successfully popped, or false if not.
	/// </summary>
	public bool TryPop()
	{
		if (popped)
			return false;

		popped = true;
		Vector2 balloonPos = loveBalloonObject.position + new Vector3(0, 0.12f, 0);
		ObjectPooler.instance.CreateCircleHitEffect(Color.red, balloonPos, 1f);
		ObjectPooler.instance.CreateHitParticles(Color.red, balloonPos);

		SoundManager.instance.PlaySound(SoundManager.Sound.BalloonPop);

		gameObject.SetActive(false);

		return true;
	}
}
