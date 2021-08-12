using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpgradeCard : MonoBehaviour
{
	public Upgrade upgrade { get; set; }

	private TextMeshPro upgradeName;
	private SpriteRenderer upgradeSprite;
	private TextMeshPro upgradeDescription;

	private void Awake()
	{
		upgradeName = transform.GetChild(0).GetComponent<TextMeshPro>();
		upgradeSprite = transform.GetChild(1).GetComponent<SpriteRenderer>();
		upgradeDescription = transform.GetChild(2).GetComponent<TextMeshPro>();
	}

	public void SetCard(Upgrade upgrade)
	{
		// Get upgrade object from upgrade
		UpgradeObject obj = GameAssets.instance.upgradeDict[upgrade];

		this.upgrade = obj.upgradeType;

		upgradeName.text = obj.upgradeName;
		upgradeSprite.sprite = obj.upgradeSprite;
		upgradeDescription.text = obj.upgradeDescription;
		upgradeDescription.ForceMeshUpdate();

		if (upgradeDescription.bounds.size.y > 4.15f)
		{
			upgradeDescription.enableAutoSizing = true;
		}
	}
}
