using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System;
using System.Linq;

public class UpgradesManager : MonoBehaviour
{
	public static UpgradesManager instance;

	public SpriteRenderer blackScreen;
	public GameObject upgradeCardTemplate;

	private GameObject chooseUpgradeText;
	private int uiLayer;
	private bool readyToPick;
	private int previousHoverCardIndex;

	private Transform[] upgradeCards = new Transform[3];

	private HashSet<Upgrade> availableUpgrades;
	private System.Random rand;

	private void Awake()
	{
		instance = this;
		uiLayer = 1 << LayerMask.NameToLayer("UI");
		rand = new System.Random();

		// Initialize available upgrades set
		availableUpgrades = new HashSet<Upgrade>(Enum.GetValues(typeof(Upgrade)).Cast<Upgrade>());
	}

	private void Update()
	{
		// If ready to choose an upgrade, raycast to check upgrade being hovered
		if (readyToPick)
		{
			// Get position of mouse
			Vector2 pos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			Vector2 worldPos = Camera.main.ScreenToWorldPoint(pos);

			// Checks for colliders on the mouse area
			RaycastHit2D hitInfo = Physics2D.Raycast(worldPos, Vector2.zero, 10, uiLayer);

			if (hitInfo)
			{
				// Hovering over upgrade card for first time
				if (hitInfo.transform.CompareTag("UpgradeCard"))
				{
					// Get index of current upgrade card being hovered
					int currIndex = Array.FindIndex(upgradeCards, (Transform val) => val == hitInfo.transform);

					// Shouldn't happen ever, but just in case...
					if (currIndex < 0) return;

					// Scale up card if it's a brand new card being hovered
					if (previousHoverCardIndex != currIndex)
					{
						CancelPreviousHoverCardTween();
						
						previousHoverCardIndex = currIndex;
						hitInfo.transform.DOScale(new Vector3(1.2f, 1.2f, 1), 0.1f).SetUpdate(true).SetEase(Ease.OutQuad);
					}

					if (Input.GetMouseButtonDown(0))
					{
						readyToPick = false;

						// Tween the non-selected upgrades downwards
						for (int i = 0; i < upgradeCards.Length; i++)
						{
							Transform currentCard = upgradeCards[i];

							if (i == currIndex)
								currentCard.DOLocalMoveY(-0.25f, 0.75f).SetEase(Ease.OutQuart).SetUpdate(true);
							else
								currentCard.DOLocalMoveY(-10f, 0.75f).SetEase(Ease.OutQuart).SetUpdate(true).OnComplete(() =>
								{
									Destroy(currentCard.gameObject);
								});
						}

						// TODO: Actually add the upgrade to your player
						StartCoroutine(FinishChoosingUpgrades(currIndex));

						availableUpgrades.Remove(upgradeCards[currIndex].GetComponent<UpgradeCard>().upgrade);
					}
				}
			}
			else
			{
				CancelPreviousHoverCardTween();
			}
		}
	}

	private void CancelPreviousHoverCardTween()
	{
		if (previousHoverCardIndex != -1)
		{
			upgradeCards[previousHoverCardIndex].DOKill();
			upgradeCards[previousHoverCardIndex].DOScale(new Vector3(1f, 1f, 1), 0.1f).SetUpdate(true).SetEase(Ease.OutQuad);
			previousHoverCardIndex = -1;
		}
	}

	public void DisplayUpgradesWindow()
	{
		StartCoroutine(DisplayUpgradesWindowCR());
	}

	private IEnumerator DisplayUpgradesWindowCR()
	{
		// Get 3 random available upgrades
		List<Upgrade> chosen = availableUpgrades.OrderBy(x => rand.Next()).Take(3).ToList();

		yield return new WaitForSecondsRealtime(0.5f);

		// Darken screen
		float alphaVal = 0.85f;
		Color darkenedColor = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, alphaVal);
		blackScreen.DOColor(darkenedColor, 0.5f).SetEase(Ease.OutQuad).SetUpdate(true);

		yield return new WaitForSecondsRealtime(0.75f);

		// Create "choose an upgrade" text
		chooseUpgradeText = ObjectPooler.instance.CreateTextObject(new Vector3(0, 4.4f, 0), Quaternion.identity, Color.white, 8, "choose an upgrade");
		TextMeshPro upgradeText = chooseUpgradeText.GetComponent<TextMeshPro>();
		upgradeText.color = new Color(1, 1, 1, 0);

		chooseUpgradeText.transform.DOLocalMoveY(4.75f, 0.75f).SetEase(Ease.OutQuad).SetUpdate(true);

		DOTween.To(() => upgradeText.color, (Color to) => upgradeText.color = to, new Color(1, 1, 1, 1), 0.75f).SetEase(Ease.OutQuad)
			.SetTarget(chooseUpgradeText.transform).SetUpdate(true);

		chooseUpgradeText.GetComponent<MeshRenderer>().sortingOrder = 11;

		RotateText(chooseUpgradeText);

		yield return new WaitForSecondsRealtime(0.5f);

		// Create upgrade cards
		Transform left = Instantiate(upgradeCardTemplate, new Vector3(-6.25f, -10f, 0), Quaternion.identity).transform;
		Transform mid = Instantiate(upgradeCardTemplate, new Vector3(0, -10f, 0), Quaternion.identity).transform;
		Transform right = Instantiate(upgradeCardTemplate, new Vector3(6.25f, -10f, 0), Quaternion.identity).transform;
		left.transform.DOLocalMoveY(-1f, 0.75f).SetEase(Ease.OutQuart).SetUpdate(true);
		mid.transform.DOLocalMoveY(-1f, 0.75f).SetEase(Ease.OutQuart).SetUpdate(true);
		right.transform.DOLocalMoveY(-1f, 0.75f).SetEase(Ease.OutQuart).SetUpdate(true).OnComplete(() => readyToPick = true);

		upgradeCards[0] = left;
		upgradeCards[1] = mid;
		upgradeCards[2] = right;

		// Set card values
		for (int i = 0; i < upgradeCards.Length; i++)
			upgradeCards[i].GetComponent<UpgradeCard>().SetCard(chosen[i]);
	}

	private void RotateText(GameObject textObj)
	{
		float time = 0.75f;

		textObj.transform.DORotate(new Vector3(0, 0, 1.5f), 2 * time).SetEase(Ease.InOutQuad).SetUpdate(true).onComplete += () =>
			textObj.transform.DORotate(new Vector3(0, 0, -1.5f), 2 * time).SetUpdate(true).SetEase(Ease.InOutQuad).onComplete += () => RotateText(textObj);
	}

	// Resets text object back to normal for next pooled instance
	private void ResetText()
	{
		if (chooseUpgradeText == null) return;

		chooseUpgradeText.transform.DOKill();

		chooseUpgradeText.transform.rotation = Quaternion.Euler(0, 0, 0);
		chooseUpgradeText.GetComponent<MeshRenderer>().sortingOrder = 0;
	}

	private IEnumerator FinishChoosingUpgrades(int chosenUpgrade)
	{
		yield return new WaitForSecondsRealtime(1f);

		// Close remaining card
		Transform currCard = upgradeCards[chosenUpgrade];
		currCard.DOLocalMoveY(-12f, 0.75f).SetEase(Ease.OutQuart).SetUpdate(true).OnComplete(() =>
		{
			Destroy(currCard.gameObject);
		});

		yield return new WaitForSecondsRealtime(0.5f);

		// Close "choose an upgrade" text
		TextMeshPro upgradeText = chooseUpgradeText.GetComponent<TextMeshPro>();
		chooseUpgradeText.transform.DOLocalMoveY(4.4f, 0.75f).SetEase(Ease.OutQuad).SetUpdate(true);

		DOTween.To(() => upgradeText.color, (Color to) => upgradeText.color = to, new Color(1, 1, 1, 0), 0.75f).SetEase(Ease.OutQuad)
			.SetTarget(chooseUpgradeText.transform).SetUpdate(true).OnComplete(() => chooseUpgradeText.SetActive(false));

		yield return new WaitForSecondsRealtime(1f);

		// Undarken the screen
		Color darkenedColor = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, 0);
		blackScreen.DOColor(darkenedColor, 0.5f).SetEase(Ease.OutQuad).SetUpdate(true);

		yield return new WaitForSecondsRealtime(0.75f);

		// Reset text
		ResetText();

		// Unpause game
		TimeManager.instance.SlowToUnpause();
	}
}
