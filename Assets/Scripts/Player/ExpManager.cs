using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ExpManager : MonoBehaviour
{
	public static ExpManager instance;

	public Transform expBar;

	private float expBarXScale;

	private int level = 1;
	private int expToLevelUp;
	private int currentExp;
	// Amount of exp currently in the UI bar
	private int displayExp;

	private bool isLeveling;

	private Tween currentTween;

	private void Awake()
	{
		instance = this;
		expBarXScale = expBar.localScale.x;
		expToLevelUp = GetEXPToLevelUp();
	}

	public void GainExp(int amount)
	{
		currentExp += amount;

		UpdateExpBar();
	}

	private void UpdateExpBar()
	{
		if (isLeveling)
			return;

		currentTween.Kill();

		if (currentExp >= expToLevelUp)
		{
			currentTween = expBar.DOScaleY(1, 0.3f).SetEase(Ease.OutQuad);
			currentTween.onComplete += () => LevelUp();
			isLeveling = true;
		}
		else
		{
			currentTween = DOTween.To(() => displayExp, (int val) =>
			{
				expBar.localScale = new Vector3(expBarXScale, (float)val / expToLevelUp, 1);
				displayExp = val;
			}, currentExp, 0.3f).SetEase(Ease.OutQuad);

			// expBar.DOScaleY(currentExp / expToLevelUp, 0.3f).SetEase(Ease.OutQuad);
		}
	}

	private void LevelUp()
	{
		PlayerController.instance.LevelUp();

		isLeveling = false;
		currentExp -= expToLevelUp;
		level++;
		expToLevelUp = GetEXPToLevelUp();

		UpdateExpBar();
	}

	private IEnumerator SlowDown()
	{
		yield return null;
	}

	private int GetEXPToLevelUp()
	{
		return 40 * level;
	}
}
