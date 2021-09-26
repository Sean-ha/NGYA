using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseUpgradeIconBuilder : MonoBehaviour
{
	public GameObject iconTemplate;
	public Transform iconHolder;

	private Vector2 currentPosition;

	private float originalPosX;

	private void Awake()
	{
		currentPosition = iconTemplate.transform.position;
		originalPosX = currentPosition.x;

		// Move the template away from scene
		iconTemplate.transform.position = new Vector2(100, 100);
	}

	public void AddUpgradeIcon(Upgrade upgrade)
	{
		GameObject icon = Instantiate(iconTemplate, iconHolder, false);
		icon.transform.position = currentPosition;
		print(GameAssets.instance.upgradeDict.Count);
		icon.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.upgradeDict[upgrade].upgradeSprite;

		// Update currentPosition
		currentPosition += new Vector2(2.25f, 0f);

		if (currentPosition.x >= 18)
		{
			currentPosition = new Vector2(originalPosX, currentPosition.y + 2.25f);
		}
	}
}
