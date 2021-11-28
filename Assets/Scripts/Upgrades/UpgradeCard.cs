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

		if (upgradeDescription.bounds.size.y > 4.15f)
		{
			upgradeDescription.enableAutoSizing = true;
		}
	}
}
