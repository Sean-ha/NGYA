using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseUpgradeIcon : MonoBehaviour
{
	public PauseUpgradeDescriptionBox descriptionBox;
	public Upgrade upgrade { get; set; }

	public void OnMouseEnter()
	{
		UpgradeObject currUpgrade = GameAssets.instance.upgradeDict[upgrade];
		descriptionBox.SetDescriptionBox(currUpgrade.upgradeName, currUpgrade.upgradeDescriptionFirst, transform.position);
	}

	public void OnMouseExit()
	{
		descriptionBox.DisableBox();
	}
}
