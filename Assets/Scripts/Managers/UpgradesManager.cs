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

	public GameObject defenderShield;
	public InspireManager inspireManager;
	public GameObject loveBalloon;
	public GameObject buddyObject;

	private PauseUpgradeIconBuilder pauseIconBuilder;

	private GameObject chooseUpgradeText;
	private int uiLayer;
	private bool readyToPick;
	private int previousHoverCardIndex = -1;

	private Transform[] upgradeCards = new Transform[3];

	private HashSet<Upgrade> availableUpgrades;

	// Contains only the currently available upgrades. When max count is reached, the upgrade will be deleted from this set!!
	private HashSet<Upgrade> commonUpgrades = new HashSet<Upgrade>();
	private HashSet<Upgrade> rareUpgrades = new HashSet<Upgrade>();

	// Key: upgrade; value: number of that upgrade currently held
	public Dictionary<Upgrade, int> obtainedUpgrades { get; set; } = new Dictionary<Upgrade, int>();

	private System.Random rand;

	private void Awake()
	{
		instance = this;
		uiLayer = 1 << LayerMask.NameToLayer("UI");
		rand = new System.Random();

		pauseIconBuilder = FindObjectOfType<PauseUpgradeIconBuilder>();

		// Initialize available upgrades set
		availableUpgrades = new HashSet<Upgrade>(Enum.GetValues(typeof(Upgrade)).Cast<Upgrade>());

		StartCoroutine(LoadUpgrades());
	}

	private IEnumerator LoadUpgrades()
	{
		while (!GameAssets.LoadingDone)
			yield return null;

		// Setup dictionary and rarity hashsets
		foreach (Upgrade upgrade in availableUpgrades)
		{
			if (GameAssets.instance.upgradeDict[upgrade].rarity == UpgradeRarity.Common)
				commonUpgrades.Add(upgrade);
			else if (GameAssets.instance.upgradeDict[upgrade].rarity == UpgradeRarity.Rare)
				rareUpgrades.Add(upgrade);
			obtainedUpgrades.Add(upgrade, 0);
		}
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

					// Move up card if it's a brand new card being hovered
					if (previousHoverCardIndex != currIndex)
					{
						CancelPreviousHoverCardTween();

						SoundManager.instance.PlaySound(SoundManager.Sound.Blip, false);
						previousHoverCardIndex = currIndex;
						hitInfo.transform.DOLocalMoveY(-0.25f, 0.1f).SetUpdate(true).SetEase(Ease.OutQuad);
					}

					if (Input.GetMouseButtonDown(0))
					{
						TimeManager.instance.canPause = false;
						SoundManager.instance.PlaySound(SoundManager.Sound.Pickup, false);
						readyToPick = false;
						previousHoverCardIndex = -1;

						// Tween the non-selected upgrades downwards
						for (int i = 0; i < upgradeCards.Length; i++)
						{
							Transform currentCard = upgradeCards[i];

							if (i == currIndex)
								currentCard.DOLocalMoveY(-0.25f, 0.75f).SetEase(Ease.OutQuart).SetUpdate(true);
							else
								currentCard.DOLocalMoveY(-18f, 1f).SetEase(Ease.OutQuart).SetUpdate(true).OnComplete(() =>
								{
									Destroy(currentCard.gameObject);
								});
						}
						StartCoroutine(FinishChoosingUpgrades(currIndex));

						Upgrade selected = upgradeCards[currIndex].GetComponent<UpgradeCard>().upgrade;
						GainUpgradeEffect(selected);
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
			upgradeCards[previousHoverCardIndex].DOLocalMoveY(-1f, 0.1f).SetUpdate(true).SetEase(Ease.OutQuad);
			previousHoverCardIndex = -1;
		}
	}

	public void DisplayUpgradesWindow(int currentLevel)
	{
		StartCoroutine(DisplayUpgradesWindowCR(currentLevel));
	}

	private IEnumerator DisplayUpgradesWindowCR(int currentLevel)
	{
		List<Upgrade> chosen;
		string chooseUpgradeStr = "";

		// If current level is a multiple of 4, choose rare upgrade. Otherwise, choose common upgrade
		if (currentLevel % 4 == 0)
		{
			chosen = rareUpgrades.OrderBy(x => rand.Next()).Take(3).ToList();
			chooseUpgradeStr = "choose a <color=#" + GameAssets.instance.blueColorHex + ">rare</color> upgrade";
		}
		else
		{
			chosen = commonUpgrades.OrderBy(x => rand.Next()).Take(3).ToList();
			chooseUpgradeStr = "choose an upgrade";
		}		

		yield return new WaitForSecondsRealtime(0.5f);

		// Darken screen
		float alphaVal = 0.85f;
		Color darkenedColor = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, alphaVal);
		blackScreen.DOColor(darkenedColor, 0.5f).SetEase(Ease.OutQuad).SetUpdate(true);

		yield return new WaitForSecondsRealtime(0.75f);

		// Create "choose an upgrade" text
		chooseUpgradeText = ObjectPooler.instance.CreateTextObject(new Vector3(0, 7.5f, 0), Quaternion.identity, Color.white, 8, chooseUpgradeStr);
		TextMeshPro upgradeText = chooseUpgradeText.GetComponent<TextMeshPro>();
		upgradeText.color = new Color(1, 1, 1, 0);

		chooseUpgradeText.transform.DOLocalMoveY(8f, 0.75f).SetEase(Ease.OutQuad).SetUpdate(true);

		DOTween.To(() => upgradeText.color, (Color to) => upgradeText.color = to, new Color(1, 1, 1, 1), 0.75f).SetEase(Ease.OutQuad)
			.SetTarget(chooseUpgradeText.transform).SetUpdate(true);

		chooseUpgradeText.GetComponent<MeshRenderer>().sortingOrder = 11;

		RotateText(chooseUpgradeText);

		yield return new WaitForSecondsRealtime(0.5f);

		// Create upgrade cards
		Transform left = Instantiate(upgradeCardTemplate, new Vector3(-10f, -18f, 0), Quaternion.identity).transform;
		Transform mid = Instantiate(upgradeCardTemplate, new Vector3(0, -18f, 0), Quaternion.identity).transform;
		Transform right = Instantiate(upgradeCardTemplate, new Vector3(10f, -18f, 0), Quaternion.identity).transform;
		left.transform.DOLocalMoveY(-1f, 1f).SetEase(Ease.OutQuart).SetUpdate(true);
		mid.transform.DOLocalMoveY(-1f, 1f).SetEase(Ease.OutQuart).SetUpdate(true);
		right.transform.DOLocalMoveY(-1f, 1f).SetEase(Ease.OutQuart).SetUpdate(true).OnComplete(() => readyToPick = true);
		SoundManager.instance.PlaySound(SoundManager.Sound.Whoosh);

		upgradeCards[0] = left;
		upgradeCards[1] = mid;
		upgradeCards[2] = right;

		// Set card values
		for (int i = 0; i < upgradeCards.Length; i++)
		{
			upgradeCards[i].GetComponent<UpgradeCard>().SetCard(chosen[i], obtainedUpgrades[chosen[i]]);
		}
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
		currCard.DOLocalMoveY(-18f, 0.75f).SetEase(Ease.OutQuart).SetUpdate(true).OnComplete(() =>
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

		TimeManager.instance.canPause = true;
		// Unpause game
		TimeManager.instance.SlowToUnpause();
	}

	private void GainUpgradeEffect(Upgrade upgrade)
	{
		UpgradeObject uo = GameAssets.instance.upgradeDict[upgrade];

		obtainedUpgrades[upgrade]++;
		// upgradeCount = 1 for first time upgrade obtained
		int upgradeCount = obtainedUpgrades[upgrade];

		// Max number of this upgrade has been reached; remove it from the available pool
		if (obtainedUpgrades[upgrade] >= uo.maxCount)
		{
			if (uo.rarity == UpgradeRarity.Common)
				commonUpgrades.Remove(upgrade);
			else if (uo.rarity == UpgradeRarity.Rare)
				rareUpgrades.Remove(upgrade);

			availableUpgrades.Remove(upgrade);
		}
		pauseIconBuilder.AddUpgradeIcon(upgrade);

		switch (upgrade)
		{
			case Upgrade.MagicBullet:
				DotBuilder.instance.ChangeBulletAmmoConsumptionChance(0.15f);
				break;

			case Upgrade.FingerlessGloves:
				ShootManager.instance.ShootCooldown -= 0.03f;
				break;

			case Upgrade.Lipstick:
				HealthSystem.instance.healthUpgradeCount += 5;
				HealthSystem.instance.UpdateMaxBar();
				HealthSystem.instance.RestoreToMaxHealth();
				break;

			case Upgrade.FlimsyString:
				AmmoSystem.instance.AmmoRegenPerSecond += 2;
				break;

			case Upgrade.FannyPack:
				AmmoSystem.instance.AmmoUpgrades += 1;
				break;

			case Upgrade.Goggles:
				ShootManager.instance.BulletDistance += 1.6f;
				break;

			case Upgrade.LoveJar:
				HealthSystem.instance.healthRegenPerMinute += 6f;
				break;

			case Upgrade.VultureClaw:
				if (upgradeCount == 1)
					ShootManager.instance.vultureClawAmount += 1;
				if (upgradeCount % 3 == 0)
					ShootManager.instance.vultureClawAmount += 1;
				ShootManager.instance.vultureClawChance += 0.15f;
				break;

			case Upgrade.BustedToaster:
				if (upgradeCount == 1)
				{
					ShootManager.instance.bustedToasterChance = 0.25f;
					ShootManager.instance.bustedToasterDM = 0.9f;
				}
				else
				{
					ShootManager.instance.bustedToasterChance += 0.05f;
					ShootManager.instance.bustedToasterDM += 0.1f;
				}
				break;

			case Upgrade.LastRegards:

				break;

				/*
			case Upgrade.CannedSoup:
				PlayerController.instance.moveSpeed += 1;
				break;

			case Upgrade.SquigglyHead:
				if (upgradeCount == 1)
				{
					ShootManager.instance.tendrilDM = 0.6f;
					ShootManager.instance.tendrilChance = 0.3f;
				}
				else
				{
					ShootManager.instance.tendrilDM += 0.15f;
					ShootManager.instance.tendrilChance += 0.05f;
				}
				break;

			case Upgrade.TatteredCharm:
				break;

			case Upgrade.DumbBigAxe:
				break;

			case Upgrade.CloakedDagger:
				break;

			case Upgrade.D20:
				break;

			case Upgrade.Thumbtack:
				break;

			case Upgrade.SwirlyStraw:
				break;

			case Upgrade.PilferedFence:
				break;

			case Upgrade.PotLid:
				break;
				*/
		}
	}

	/*  Old stuff
	private void GainUpgradeEffect(Upgrade upgrade)
	{
		availableUpgrades.Remove(upgrade);
		obtainedUpgrades.Add(upgrade);
		pauseIconBuilder.AddUpgradeIcon(upgrade);

		switch (upgrade)
		{
			case Upgrade.Adrenaline:
				PlayerController.instance.moveSpeed += 2f;
				PlayerController.instance.onTakeDamage.AddListener(() =>
				{
					// TODO: Create some sort of effect to show adrenaline is taking effect, after taking damage ??

					PlayerController.instance.moveSpeed += 6f;
					DOTween.To(() => PlayerController.instance.moveSpeed, (float val) => PlayerController.instance.moveSpeed = val, PlayerController.instance.moveSpeed - 6, 0.95f);
				});
				break;

			case Upgrade.Backbone:
				DotBuilder.instance.BuildBackboneDot();
				break;

			case Upgrade.Blowback:
				PlayerController.instance.onTakeDamage.AddListener(() =>
				{
					Instantiate(GameAssets.instance.blowbackExplosion, PlayerController.instance.transform.position, Quaternion.identity);
					SoundManager.instance.PlaySound(SoundManager.Sound.Explosion1);
				});
				break;

			case Upgrade.Brawl:
				ShootManager.instance.BulletDistance = 3.5f;
				ShootManager.instance.damage += 2f;
				break;

			case Upgrade.DeepBreaths:
				PlayerController.instance.onStandStill.AddListener(() => AmmoSystem.instance.AmmoRegenPerSecond += 10);
				PlayerController.instance.onCancelStandStill.AddListener(() => AmmoSystem.instance.AmmoRegenPerSecond -= 10);
				break;

			case Upgrade.Defender:
				defenderShield.SetActive(true);
				break;

			case Upgrade.HeavyBullets:
				ShootManager.instance.damage += 2;
				DotBuilder.instance.ChangeBulletAmmoCost(1);
				break;

			case Upgrade.Inspire:
				inspireManager.enabled = true;
				break;

			case Upgrade.Love:
				loveBalloon.SetActive(true);
				break;

			case Upgrade.MagicBullets:
				DotBuilder.instance.ChangeBulletAmmoConsumptionChance(0.25f);
				break;

			case Upgrade.Resistance:
				PlayerController.instance.invincibilityDuration += 2.0f;
				HealthSystem.instance.damageReduction += 0.1f;
				break;

			case Upgrade.Sight:
				ShootManager.instance.BulletDistance += 6f;
				break;

			case Upgrade.Snipe:
				// TODO
				break;

			case Upgrade.TriggerFinger:
				ShootManager.instance.ShootCooldown -= 0.075f;
				break;

			case Upgrade.Unwavering:
				PlayerController.instance.knockBackForce = 0;
				PlayerController.instance.onStandStill.AddListener(() =>
				{
					HealthSystem.instance.damageReduction += 0.25f;
				});
				PlayerController.instance.onCancelStandStill.AddListener(() =>
				{
					HealthSystem.instance.damageReduction -= 0.25f;
				});
				break;

			case Upgrade.Shrapnel:
				foreach (EnemyHealth enemy in FindObjectsOfType<EnemyHealth>())
				{
					enemy.onDeath.AddListener(() =>
					{
						enemy.CreateShrapnel();
					});
				}
				break;

			case Upgrade.Buddy:
				buddyObject.SetActive(true);
				ShootManager.instance.onShoot.AddListener(buddyObject.GetComponentInChildren<BuddyShooter>().Shoot);
				break;

			case Upgrade.Sharpness:
				ShootManager.instance.pierceCount += 3;
				break;

			case Upgrade.Supplies:
				AmmoSystem.instance.AmmoUpgrades += 1;
				break;
		}
	}
	*/
}
