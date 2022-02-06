using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpgradeCard : MonoBehaviour
{
	public Upgrade upgrade { get; set; }

	[SerializeField] private TextMeshPro upgradeName;
	[SerializeField] private SpriteRenderer upgradeSprite;
	[SerializeField] private TextMeshPro upgradeDescription;

	[SerializeField] private GameObject selectedUiHolder;
	// Selected UI parts
	[SerializeField] private Transform topLeft;
	[SerializeField] private Transform topRight;
	[SerializeField] private Transform botLeft;
	[SerializeField] private Transform botRight;

	// Together these two parts act as a single unit allowing for top half to have different scaling from bottom half
	// Center piece that will scale upwards
	[SerializeField] private Transform centerTop;
	// Center piece that will scale downwards
	[SerializeField] private Transform centerBot;

	[SerializeField] private ParticleSystem[] disappearParticles;

	[SerializeField] private LineRenderer leftLR;
	[SerializeField] private LineRenderer botLR;
	[SerializeField] private LineRenderer rightLR;
	[SerializeField] private LineRenderer topLR;

	[SerializeField] private ParticleSystem[] chosenParticles;

	[SerializeField] private Transform outlineParticles;
	private Transform fillParticles;

	// Tweens for when hovering over something (i.e. selection UI is opening)
	private List<Tween> openTweenList = new List<Tween>();
	// Tweens for when unhovering over something (i.e. selection UI is closing)
	private List<Tween> closeTweenList = new List<Tween>();

	// True if player is currently hovering over this card, false otherwise
	private bool isHovered;

	private void Start()
	{
		StartCoroutine(LateStart());
	}

	private IEnumerator LateStart()
	{
		yield return null;
		fillParticles = outlineParticles.GetChild(0);
	}

	// Params: amount is the number of this upgrade the player currently has
	public void SetCard(Upgrade upgrade, int amount)
	{
		// Get upgrade object from upgrade
		UpgradeObject obj = GameAssets.instance.upgradeDict[upgrade];

		this.upgrade = obj.upgradeType;

		string upgradeTxt = "";
		if (obj.rarity == UpgradeRarity.Rare)
			upgradeTxt += "<color=#" + GameAssets.instance.blueColorHex + ">";

		upgradeTxt += obj.upgradeName + " " + (amount + 1);

		if (obj.rarity == UpgradeRarity.Rare)
			upgradeTxt += "</color>";

		upgradeName.text = upgradeTxt;
		upgradeSprite.sprite = obj.upgradeSprite;

		if (amount == 0)
			upgradeDescription.text = obj.upgradeDescriptionFirst;
		else
			upgradeDescription.text = obj.upgradeDescriptionSecond;

		upgradeDescription.ForceMeshUpdate();
	}

	private void KillAllTweens()
	{
		foreach (Tween t in openTweenList)
		{
			t.Kill();
		}
		foreach (Tween t in closeTweenList)
		{
			t.Kill();
		}
	}

	// Called when card is hovered over.
	public void DisplayText()
	{
		if (isHovered)
			return;
		
		isHovered = true;

		// Animate "selected" ui
		KillAllTweens();
		// Distance from 0 that a corner should be in the x direction (static value, same for all upgrades)
		float localWidth = 3f;
		float animationLength = 0.1f;

		selectedUiHolder.SetActive(true);
		// Starting positions
		topLeft.transform.localPosition = new Vector3(-localWidth, 0, 0);
		topRight.transform.localPosition = new Vector3(localWidth, 0, 0);
		botLeft.transform.localPosition = new Vector3(-localWidth, 0, 0);
		botRight.transform.localPosition = new Vector3(localWidth, 0, 0);
		centerTop.localScale = new Vector3(centerTop.localScale.x, 0.5f, 1);
		centerBot.localScale = new Vector3(centerBot.localScale.x, 0.5f, 1);

		float topPos = upgradeName.preferredHeight + 2.75f;
		float botPos = upgradeDescription.preferredHeight + 2.75f;

		// Tween corners to proper positions
		// Top stuff
		openTweenList.Add(DOTween.To(() => 0f, (float val) =>
		{
			topLeft.localPosition = new Vector3(topLeft.localPosition.x, val, 0);
			topRight.localPosition = new Vector3(topRight.localPosition.x, val, 0);
			centerTop.localScale = new Vector3(centerTop.localScale.x, val / 1.8125f + 0.5f, 1f);
		}, topPos, animationLength).SetEase(Ease.InOutQuad).SetUpdate(true));
		// Bottom stuff
		openTweenList.Add(DOTween.To(() => 0f, (float val) =>
		{
			botLeft.localPosition = new Vector3(botLeft.localPosition.x, -val, 0);
			botRight.localPosition = new Vector3(botRight.localPosition.x, -val, 0);
			centerBot.localScale = new Vector3(centerBot.localScale.x, val / 1.8125f + 0.5f, 1f);
		}, botPos, animationLength).SetEase(Ease.InOutQuad).SetUpdate(true));
	}

	// Called when the card stops being hovered over. Should be efficient even when called every frame!
	public void HideText()
	{
		if (!isHovered)
			return;

		isHovered = false;

		// Animate "unselected" ui
		KillAllTweens();
		float animationLength = 0.1f;

		// Starting positions
		/*topLeft.transform.localPosition = new Vector3(-localWidth, 0, 0);
		topRight.transform.localPosition = new Vector3(localWidth, 0, 0);
		botLeft.transform.localPosition = new Vector3(-localWidth, 0, 0);
		botRight.transform.localPosition = new Vector3(localWidth, 0, 0);
		centerTop.localScale = new Vector3(centerTop.localScale.x, 0.5f, 1);
		centerBot.localScale = new Vector3(centerBot.localScale.x, 0.5f, 1);*/

		float topPos = 0f;
		float botPos = 0f;

		// Tween corners to proper positions
		// Top stuff
		openTweenList.Add(DOTween.To(() => topLeft.localPosition.y, (float val) =>
		{
			topLeft.localPosition = new Vector3(topLeft.localPosition.x, val, 0);
			topRight.localPosition = new Vector3(topRight.localPosition.x, val, 0);
			centerTop.localScale = new Vector3(centerTop.localScale.x, val / 1.8125f + 0.5f, 1f);
		}, topPos, animationLength).SetEase(Ease.InOutQuad).SetUpdate(true));
		// Bottom stuff
		openTweenList.Add(DOTween.To(() => Mathf.Abs(botLeft.localPosition.y), (float val) =>
		{
			botLeft.localPosition = new Vector3(botLeft.localPosition.x, -val, 0);
			botRight.localPosition = new Vector3(botRight.localPosition.x, -val, 0);
			centerBot.localScale = new Vector3(centerBot.localScale.x, val / 1.8125f + 0.5f, 1f);
		}, botPos, animationLength).SetEase(Ease.InOutQuad).SetUpdate(true).OnComplete(() =>
		{
			selectedUiHolder.SetActive(false);
		}));
	}

	// Called on the UpgradeCards that were not chosen. Scaled down to zero and then destroyed.
	public void UnpickedCard()
	{
		float duration = 0.1f;
		transform.DOScale(0f, duration).SetEase(Ease.InOutQuad).SetUpdate(true);
		fillParticles.DOScale(0f, duration).SetEase(Ease.InOutQuad).SetUpdate(true).OnComplete(() =>
		{
			Destroy(leftLR.gameObject);
			Destroy(botLR.gameObject);
			Destroy(rightLR.gameObject);
			Destroy(topLR.gameObject);
			foreach (ParticleSystem ps in disappearParticles)
			{
				// Make sure these particle systems have stop behavior set to Destroy!!!
				ps.transform.parent = null;
				ps.transform.localScale = Vector3.one;
				ps.Play();
			}
		});

		SoundManager.instance.PlaySound(SoundManager.Sound.Squish);

		DOVirtual.DelayedCall(2.0f, () => Destroy(gameObject), ignoreTimeScale: true);
	}

	// Called on the UpgradeCard that was chosen
	public void PickedCard()
	{
		foreach (Tween t in openTweenList)
		{
			t.Complete();
		}

		SoundManager.instance.PlaySound(SoundManager.Sound.UpgradeChoose1, false);

		float duration = 0.35f;
		// Left side
		float endpoint = Mathf.Abs(leftLR.transform.position.y - botLR.transform.position.y);
		DOTween.To(() => 0f, (float val) => leftLR.SetPosition(1, new Vector3(0, -val, 0)), endpoint, 
			duration).SetEase(Ease.OutQuad).SetUpdate(true);
		// Bot side
		endpoint = Mathf.Abs(botLR.transform.position.x - rightLR.transform.position.x);
		DOTween.To(() => 0f, (float val) => botLR.SetPosition(1, new Vector3(val, 0, 0)), endpoint,
			duration).SetEase(Ease.OutQuad).SetUpdate(true);
		// Right side
		endpoint = Mathf.Abs(topLR.transform.position.y - rightLR.transform.position.y);
		DOTween.To(() => 0f, (float val) => rightLR.SetPosition(1, new Vector3(0f, val, 0)), endpoint,
			duration).SetEase(Ease.OutQuad).SetUpdate(true);
		// Top side
		endpoint = Mathf.Abs(topLR.transform.position.x - leftLR.transform.position.x);
		DOTween.To(() => 0f, (float val) => topLR.SetPosition(1, new Vector3(-val, 0, 0)), endpoint,
			duration).SetEase(Ease.OutQuad).SetUpdate(true).OnComplete(() =>
			{
				SoundManager.instance.PlaySound(SoundManager.Sound.UpgradeChoose2, false);

				float ySize = Mathf.Abs(leftLR.transform.position.y - rightLR.transform.position.y);
				float yCenter = leftLR.transform.position.y - (ySize / 2f);
				foreach (ParticleSystem ps in chosenParticles)
				{
					ps.transform.parent = null;
					ParticleSystem.ShapeModule sm = ps.shape;
					ps.transform.position = new Vector3(ps.transform.position.x, yCenter, 0);
					sm.scale = new Vector3(sm.scale.x, ySize, 1);
					ps.Play();
				}
			});

		DOVirtual.DelayedCall(1.5f, UnpickedCard);
	}

	// This is a rare upgrade! Perform effects and stuff to show it
	public void SetRareUpgrade()
	{
		ParticleSystem.MainModule main = outlineParticles.GetComponent<ParticleSystem>().main;
		main.startColor = GameAssets.instance.blueColor;
	}
}
