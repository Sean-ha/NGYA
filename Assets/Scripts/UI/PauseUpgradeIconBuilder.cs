using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PauseUpgradeIconBuilder : MonoBehaviour
{
	public GameObject iconTemplate;
	public Transform iconHolder;

	private Vector3 currentPosition;

	private float originalPosX;

	private Dictionary<Upgrade_OLD2, PauseUpgradeObject> upgradeCounts = new Dictionary<Upgrade_OLD2, PauseUpgradeObject>();

	private void Awake()
	{
		currentPosition = iconTemplate.transform.position;
		originalPosX = currentPosition.x;

		// Move the template away from scene
		iconTemplate.transform.position = new Vector3(100, 100, iconTemplate.transform.position.z);
	}

	public void AddUpgradeIcon(Upgrade_OLD2 upgrade)
	{
		// Add 1 to existing upgrade
		if (upgradeCounts.ContainsKey(upgrade))
		{
			PauseUpgradeObject obj = upgradeCounts[upgrade];

			obj.quantity++;
			obj.quantityText.text = obj.quantity.ToString();
			obj.quantityText.GetComponent<TextMeshShadowCreator>().ForceRefreshShadow();
		}
		// New upgrade
		else
		{
			GameObject icon = Instantiate(iconTemplate, iconHolder, false);
			TextMeshPro quantityText = icon.transform.GetChild(0).GetComponent<TextMeshPro>();

			icon.GetComponent<PauseUpgradeIcon>().upgrade = upgrade;

			icon.transform.position = currentPosition;
			icon.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.upgradeDict[upgrade].upgradeSprite;
			// Update currentPosition
			currentPosition += new Vector3(2.25f, 0f, 0f);

			PauseUpgradeObject obj = new PauseUpgradeObject();

			obj.pauseObject = icon;
			obj.quantityText = quantityText;
			obj.quantity = 1;

			quantityText.text = "1";
			upgradeCounts[upgrade] = obj;
			quantityText.GetComponent<TextMeshShadowCreator>().ForceRefreshShadow();
		}		

		if (currentPosition.x >= 18)
		{
			currentPosition = new Vector2(originalPosX, currentPosition.y + 2.25f);
		}
	}

	private class PauseUpgradeObject
	{
		public GameObject pauseObject;
		public int quantity;
		public TextMeshPro quantityText;
	}
}
