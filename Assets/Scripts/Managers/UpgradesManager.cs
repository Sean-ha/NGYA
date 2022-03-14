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

	public UpgradeWindow upgradeWindow;

	public GameObject upgradeCardTemplate;

	public GameObject defenderShield;
	public ShockCollar shockCollar;

	// Unused, Old upgrades
	public InspireManager inspireManager;
	public GameObject loveBalloon;
	public GameObject buddyObject;

	public GameObject chooseUpgradeText;

	private PauseUpgradeIconBuilder pauseIconBuilder;
	
	private int uiLayer;
	private bool readyToPick;
	private int previousHoverCardIndex = -1;

	private UpgradeCard[] upgradeCards = new UpgradeCard[3];

	private HashSet<Upgrade> availableUpgrades;

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
		// TODO: initialize availableUpgrades based on which character is being played
		// availableUpgrades = new HashSet<Upgrade>(Enum.GetValues(typeof(Upgrade)).Cast<Upgrade>());
		availableUpgrades = new HashSet<Upgrade>();
		// TOOD: Don't hardcode this value, probably!!!
		for (int i = 0; i <= 12; i++)
		{
			availableUpgrades.Add((Upgrade)i);
		}

		StartCoroutine(LoadUpgrades());
	}

	private IEnumerator LoadUpgrades()
	{
		while (!GameAssets.LoadingDone)
			yield return null;

		// Setup dictionary
		foreach (Upgrade upgrade in availableUpgrades)
		{
			if (!GameAssets.instance.upgradeDict.ContainsKey(upgrade))
			{
				print(upgrade);
			}
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
				// Hovering over upgrade card
				if (hitInfo.transform.CompareTag("UpgradeCard"))
				{
					// Get index of current upgrade card being hovered
					int currIndex = Array.FindIndex(upgradeCards, (UpgradeCard val) => val.transform == hitInfo.transform);

					// Shouldn't happen ever, but just in case...
					if (currIndex < 0) return;

					// Behavior for a new card being hovered
					if (previousHoverCardIndex != currIndex)
					{
						CancelPreviousHoverCardTween();

						SoundManager.instance.PlaySound(SoundManager.Sound.Blip, false);
						previousHoverCardIndex = currIndex;
						// hitInfo.transform.GetChild(0).DOLocalMoveY(0.5f, 0.1f).SetUpdate(true).SetEase(Ease.OutQuad);
						hitInfo.transform.GetComponent<UpgradeCard>().DisplayText();
					}

					// On upgrade card click behavior
					if (Input.GetMouseButtonDown(0))
					{
						TimeManager.instance.canPause = false;
						
						CameraShake.instance.ShakeCameraRealtime(0.3f, 0.5f);
						readyToPick = false;
						previousHoverCardIndex = -1;

						// Cards do something based on whether or not they were picked
						for (int i = 0; i < upgradeCards.Length; i++)
						{
							UpgradeCard currentCard = upgradeCards[i];

							if (i != currIndex)
								currentCard.UnpickedCard();
							else
								currentCard.PickedCard();
						}

						StartCoroutine(HideUpgradeWindow(currIndex));

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
			upgradeCards[previousHoverCardIndex].HideText();
			// upgradeCards[previousHoverCardIndex].transform.DOKill();
			// upgradeCards[previousHoverCardIndex].transform.GetChild(0).DOLocalMoveY(0f, 0.1f).SetUpdate(true).SetEase(Ease.OutQuad);
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
		bool rareUpgrade = false;

		// If current level is a multiple of 4, choose rare upgrade. Otherwise, choose common upgrade
		chosen = availableUpgrades.OrderBy(x => rand.Next()).Take(3).ToList();
		chooseUpgradeStr = "choose an upgrade";

		chooseUpgradeText.SetActive(true);
		chooseUpgradeText.GetComponent<TextMeshPro>().text = chooseUpgradeStr;

		// Create upgrade cards
		Transform left = Instantiate(upgradeCardTemplate, new Vector3(-10f, 0, 0), Quaternion.identity).transform;
		Transform mid = Instantiate(upgradeCardTemplate, new Vector3(0, 0, 0), Quaternion.identity).transform;
		Transform right = Instantiate(upgradeCardTemplate, new Vector3(10f, 0, 0), Quaternion.identity).transform;

		upgradeCards[0] = left.GetComponent<UpgradeCard>();
		upgradeCards[1] = mid.GetComponent<UpgradeCard>();
		upgradeCards[2] = right.GetComponent<UpgradeCard>();

		// Set card values
		for (int i = 0; i < upgradeCards.Length; i++)
		{
			if (rareUpgrade)
				upgradeCards[i].SetRareUpgrade();

			upgradeCards[i].GetComponent<UpgradeCard>().SetCard(chosen[i], obtainedUpgrades[chosen[i]]);
		}

		yield return new WaitForSecondsRealtime(0.4f);

		upgradeWindow.DisplayUpgradeWindow(() => readyToPick = true);
	}

	// Deprecated
	private IEnumerator DisplayUpgradesWindowCR_Dep(int currentLevel)
	{
		List<Upgrade> chosen;
		string chooseUpgradeStr = "";

		// If current level is a multiple of 4, choose rare upgrade. Otherwise, choose common upgrade
		chosen = availableUpgrades.OrderBy(x => rand.Next()).Take(3).ToList();
		chooseUpgradeStr = "choose an upgrade";

		yield return new WaitForSecondsRealtime(0.5f);

		// Darken screen
		float alphaVal = 0.85f;
		// Color darkenedColor = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, alphaVal);
		// blackScreen.DOColor(darkenedColor, 0.5f).SetEase(Ease.OutQuad).SetUpdate(true);

		yield return new WaitForSecondsRealtime(0.75f);

		// Create "choose an upgrade" text
		chooseUpgradeText = ObjectPooler.instance.CreateUITextObject(new Vector3(0, 7.5f, 0), Quaternion.identity, Color.white, 8, chooseUpgradeStr);
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
		// SoundManager.instance.PlaySound(SoundManager.Sound.Whoosh);

		upgradeCards[0] = left.GetComponent<UpgradeCard>();
		upgradeCards[1] = mid.GetComponent<UpgradeCard>();
		upgradeCards[2] = right.GetComponent<UpgradeCard>();

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

	private IEnumerator HideUpgradeWindow(int chosenUpgrade)
	{
		// TODO: THIS WHOLE METHOD

		yield return new WaitForSecondsRealtime(2f);

		/*
		// Close remaining card
		Transform currCard = upgradeCards[chosenUpgrade].transform;
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
		// Color darkenedColor = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, 0);
		// blackScreen.DOColor(darkenedColor, 0.5f).SetEase(Ease.OutQuad).SetUpdate(true);

		yield return new WaitForSecondsRealtime(0.75f);
		
		// Reset text
		ResetText();
		*/

		upgradeWindow.HideUpgradeWindow(() =>
		{
			TimeManager.instance.canPause = true;
			// Unpause game
			TimeManager.instance.SlowToUnpause();
		});		
	}

	private void GainUpgradeEffect(Upgrade upgrade)
	{
		UpgradeObject uo = GameAssets.instance.upgradeDict[upgrade];

		obtainedUpgrades[upgrade]++;
		// upgradeCount = 1 for first time upgrade obtained
		int upgradeCount = obtainedUpgrades[upgrade];

		// Max number of this upgrade has been reached; remove it from the available pool
		if (obtainedUpgrades[upgrade] >= uo.upgradeDescriptions.Length)
		{
			availableUpgrades.Remove(upgrade);
		}
		pauseIconBuilder.AddUpgradeIcon(upgrade);

		switch (upgrade)
		{
			case Upgrade.Vitality:
				HealthSystem.instance.healthUpgradeCount += 5;
				HealthSystem.instance.UpdateMaxBar();
				HealthSystem.instance.RestoreToMaxHealth();
				break;

			case Upgrade.Wisdom:
				// TODO
				break;

			case Upgrade.Serenity:
				AmmoSystem.instance.AmmoRegenPerSecond += 2f;
				break;

			case Upgrade.Haste:
				PlayerController.instance.moveSpeed += 1.5f;
				break;

			case Upgrade.Ironskin:
				HealthSystem.instance.damageReduction += 0.1f;
				break;

			case Upgrade.ManaMastery:
				// TODO
				break;

			case Upgrade.FocusMind:
				// TODO
				break;

			case Upgrade.Barrier:
				// TODO
				break;

			case Upgrade.PowerRune:
				// TODO
				break;

			case Upgrade.MagicDice:
				ShootManager.instance.critChance += 0.1f;
				break;

			case Upgrade.ArcaneMight:
				ShootManager.instance.critDamage += 0.1f;
				break;

			case Upgrade.LastRegards:
				ShootManager.instance.lastRegardsDM = 1f;
				ShootManager.instance.lastRegardsBulletCount += 9;
				break;

			case Upgrade.Vampirism:
				// TODO
				break;

			case Upgrade.QuickBolts:

				break;

			case Upgrade.MagicBolt:

				break;

			case Upgrade.CondensedMagic:

				break;

			case Upgrade.MoreBolts:

				break;
		}
	}

	/*
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
				{
					ShootManager.instance.vultureClawAmount = 1;
					ShootManager.instance.vultureClawChance = 0.15f;
				}
				else
				{
					ShootManager.instance.vultureClawChance += 0.09f;
				}
				if (upgradeCount == 3 || upgradeCount == 6)
				{ 
					ShootManager.instance.vultureClawAmount += 1;
				}
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
				if (upgradeCount == 1)
				{
					ShootManager.instance.lastRegardsBulletCount = 10;
					ShootManager.instance.lastRegardsDM = 1f;
				}
				else
				{
					ShootManager.instance.lastRegardsBulletCount += 8;
					ShootManager.instance.lastRegardsDM += 0.04f;
				}
				break;

			case Upgrade.CannedSoup:
				PlayerController.instance.moveSpeed += 1.5f;
				break;

			case Upgrade.AbyssalHead:
				if (upgradeCount == 1)
				{
					ShootManager.instance.abyssalHeadDM = 0.7f;
					ShootManager.instance.abyssalHeadChance = 0.3f;
				}
				else
				{
					ShootManager.instance.abyssalHeadDM += 0.2f;
					ShootManager.instance.abyssalHeadChance += 0.12f;
				}
				break;

			case Upgrade.SinisterCharm:
				ShootManager.instance.sinisterCharmDM = 0.5f;
				ShootManager.instance.sinisterCharmChance += 0.12f;
				break;

			case Upgrade.DeadlyBananas:
				if (upgradeCount == 1)
				{
					ShootManager.instance.bananaChance = 0.3f;
					ShootManager.instance.bananaDM = 1f;
				}
				else
				{
					ShootManager.instance.bananaChance += 0.15f;
					ShootManager.instance.bananaDM += 0.2f;
				}
				break;

			
			case Upgrade.Scissors:
				if (upgradeCount == 1)
				{
					ShootManager.instance.scissorsHealthPercent = 0.15f;
					ShootManager.instance.scissorsCritChanceAddition = 0.15f;
				}
				else
				{
					ShootManager.instance.scissorsHealthPercent += 0.06f;
					ShootManager.instance.scissorsCritChanceAddition += 0.15f;
				}
				break;

			case Upgrade.CloakedDagger:
				ShootManager.instance.cloakedDaggerDM += 0.5f;
				break;

			case Upgrade.D6:
				ShootManager.instance.critChance += 0.1f;
				break;

			case Upgrade.Thumbtack:
				ShootManager.instance.critDamage += 0.12f;
				break;

			case Upgrade.SwirlyStraw:
				ShootManager.instance.healthRestorePerCrit += 0.5f;
				break;

			case Upgrade.RoyalShield:
				DefenderShield s = defenderShield.GetComponent<DefenderShield>();
				if (upgradeCount == 1)
				{
					defenderShield.SetActive(true);
					s.ObtainShield();
					s.cooldown = 30f;
				}
				else
				{
					s.cooldown -= 5f;
					s.DecrementRemainingShieldCooldown(5f);
				}
				break;

			case Upgrade.PotLid:
				if (upgradeCount == 1)
					HealthSystem.instance.damageReduction += 0.1f;
				else
					HealthSystem.instance.damageReduction += 0.08f;
				break;

			case Upgrade.DopeSunglasses:
				ShootManager.instance.damageMultiplier += 0.06f;
				break;

			case Upgrade.RadCape:
				HealthSystem.instance.dodgeChance += 0.15f;
				break;


			// Rare upgrades:
			case Upgrade.StarFragment:
				ShootManager.instance.starFragmentCount += 1;
				ShootManager.instance.starFragmentDM = 0.65f;
				break;

			case Upgrade.GentleQuill:
				ShootManager.instance.pierceCount += 1;
				break;

			case Upgrade.VoltHammer:
				if (upgradeCount == 1)
				{
					ShootManager.instance.voltHammerChance = 0.11f;
					ShootManager.instance.voltHammerDM = 0.65f;
				}
				else
				{
					ShootManager.instance.voltHammerChance += 0.05f;
					ShootManager.instance.voltHammerDM += 0.3f;
				}
				break;

			case Upgrade.JumboGeorge:
				if (upgradeCount == 1)
				{
					ShootManager.instance.jumboGeorgeShotCount = 18;
					ShootManager.instance.jumboGeorgeDM = 0.8f;
				}
				else
				{
					ShootManager.instance.jumboGeorgeShotCount--;
					ShootManager.instance.jumboGeorgeDM += 0.25f;
				}
				break;

			case Upgrade.WeirdEyeball:
				if (upgradeCount == 1)
				{
					ShootManager.instance.weirdEyeballChance = 0.11f;
					ShootManager.instance.weirdEyeballDM = 1.5f;
				}
				else
				{
					ShootManager.instance.weirdEyeballChance += 0.08f;
					ShootManager.instance.weirdEyeballDM += 0.2f;
				}
				break;

			case Upgrade.MoonlightScythe:
				if (upgradeCount == 1)
				{
					ShootManager.instance.moonlightScytheChance = 0.1f;
					ShootManager.instance.moonlightScytheDM = 0.3f;
				}
				else
				{
					ShootManager.instance.moonlightScytheChance += 0.05f;
					ShootManager.instance.moonlightScytheDM += 0.17f;
				}
				break;

			case Upgrade.ThunderWand:
				if (upgradeCount == 1)
				{
					ShootManager.instance.thunderWandChance = 0.1f;
					ShootManager.instance.thunderWandDM = 1.1f;
				}
				else
				{
					ShootManager.instance.thunderWandChance += 0.06f;
					ShootManager.instance.thunderWandDM += 0.2f;
				}
				break;

			case Upgrade.ShockCollar:
				if (upgradeCount == 1)
				{
					shockCollar.gameObject.SetActive(true);
					shockCollar.EnableShockCollar(true);
					shockCollar.damageMultiplier = 0.25f;
					shockCollar.timePerShock = 0.8f;
					shockCollar.SetRadius(3.5f);
				}
				else
				{
					shockCollar.damageMultiplier += 0.1f;
					shockCollar.SetRadius(shockCollar.radius + 0.75f);
				}
				break;
			*/



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
