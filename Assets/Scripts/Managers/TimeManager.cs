using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TimeManager : MonoBehaviour
{
	public static TimeManager instance;

	public static bool IsPaused;

	public GameObject pauseBlackScreen;
	public GameObject pauseText;
	public GameObject pauseUpgradeIconHolder;
	public PauseUpgradeDescriptionBox pauseDescriptionBox;

	// Whether or not pausing is allowed
	public bool canPause { get; set; } = true;

	// Whether in upgrade choose window or not
	private bool inUpgradeWindow;
	// Whether in a slow to pause or slow to unpause transition
	private bool inTransition;
	// Whether in pause menu or not
	private bool inPauseMenu;

	private Tween currentSlowToPauseTween;


	private void Awake()
	{
		instance = this;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			// Unpause
			if (inPauseMenu)
			{
				if (inUpgradeWindow)
				{
					ExitPauseMenu();

				}
				else if (inTransition)
				{
					ExitPauseMenu();
					UnpauseGame();
					currentSlowToPauseTween.Play();
				}
				else
				{
					Time.timeScale = 1;
					UnpauseGame();
					ExitPauseMenu();
				}
			}
			// Pause
			else if (canPause)
			{
				if (inUpgradeWindow)
				{
					ShowPauseMenu();
				}
				else if (inTransition)
				{
					currentSlowToPauseTween.Pause();
					PauseGame();
					ShowPauseMenu();
				}
				else
				{
					PauseGame();
					ShowPauseMenu();
				}
			}
		}
	}

	private void ShowPauseMenu()
	{
		inPauseMenu = true;
		SoundManager.instance.PlaySoundPitch(SoundManager.Sound.PauseMenuClick, 1.06f);
		// Show pause menu stuff here
		pauseBlackScreen.SetActive(true);
		pauseText.SetActive(true);
		pauseUpgradeIconHolder.SetActive(true);
	}

	private void ExitPauseMenu()
	{
		inPauseMenu = false;
		SoundManager.instance.PlaySoundPitch(SoundManager.Sound.PauseMenuClick, 0.94f);
		// Hide pause menu stuff here
		pauseBlackScreen.SetActive(false);
		pauseText.SetActive(false);
		pauseUpgradeIconHolder.SetActive(false);
		pauseDescriptionBox.DisableBox();
	}

	public void PauseGame()
	{
		Time.timeScale = 0;
		IsPaused = true;
	}

	public void UnpauseGame()
	{
		IsPaused = false;
	}

	public void SlowToPause(TweenCallback onComplete, float time = 1f)
	{
		inTransition = true;
		currentSlowToPauseTween = DOTween.To(() => Time.timeScale, (float val) => Time.timeScale = val, 0, time).SetUpdate(true).OnComplete(() => 
		{
			PauseGame();
			inUpgradeWindow = true;
			inTransition = false;
		});
		currentSlowToPauseTween.onComplete += onComplete;
	}

	public void SlowToUnpause(float time = 1.3f)
	{
		inUpgradeWindow = false;
		inTransition = true;
		UnpauseGame();
		currentSlowToPauseTween = DOTween.To(() => Time.timeScale, (float val) => Time.timeScale = val, 1, time).SetUpdate(true).OnComplete(() =>
		{
			inTransition = false;
		});
	}
}
