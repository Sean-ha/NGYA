using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TimeManager : MonoBehaviour
{
	public static TimeManager instance;

	public static bool IsPaused;

	private void Awake()
	{
		instance = this;
	}

	public void PauseGame()
	{
		IsPaused = true;
		CameraShake.instance.CancelShake();
		CameraShake.instance.canShake = false;
	}

	public void UnpauseGame()
	{
		IsPaused = false;
		CameraShake.instance.canShake = true;
	}

	public void SlowToPause(TweenCallback onComplete, float time = 2.3f)
	{
		Tween tween = DOTween.To(() => Time.timeScale, (float val) => Time.timeScale = val, 0, time).SetUpdate(true).OnComplete(() => PauseGame());
		tween.onComplete += onComplete;
	}

	public void SlowToUnpause(float time = 2.3f)
	{
		UnpauseGame();
		DOTween.To(() => Time.timeScale, (float val) => Time.timeScale = val, 1, time).SetUpdate(true);
	}
}
