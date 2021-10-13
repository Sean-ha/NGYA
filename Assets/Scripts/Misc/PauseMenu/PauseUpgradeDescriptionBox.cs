using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PauseUpgradeDescriptionBox : MonoBehaviour
{
	public TextMeshPro upgradeName;
	public TextMeshPro upgradeDescription;

	private SpriteRenderer sr;

	private void Awake()
	{
		sr = GetComponent<SpriteRenderer>();
		gameObject.SetActive(false);
	}

	public void SetDescriptionBox(string name, string description, Vector2 rootPosition)
	{
		upgradeName.text = name;
		upgradeDescription.text = description;
		upgradeName.ForceMeshUpdate();
		upgradeDescription.ForceMeshUpdate();

		// Sets box size to be the right size based on incoming text
		float boxHeight = upgradeDescription.preferredHeight + 3.6f;
		sr.size = new Vector2(sr.size.x, boxHeight);

		upgradeName.transform.localPosition = new Vector2(0, boxHeight - 1.2f);
		upgradeDescription.transform.localPosition = new Vector2(0, boxHeight - 4.2f);

		// Sets box position to be correct
		transform.position = rootPosition + new Vector2(0, 1.25f);

		// Box cannot leave edges of screen (horizontally, at least)
		if (transform.position.x < -15f)
			transform.position = new Vector2(-15f, transform.position.y);
		if (transform.position.x > 15f)
			transform.position = new Vector2(15f, transform.position.y);

		gameObject.SetActive(true);
	}

	public void DisableBox()
	{
		gameObject.SetActive(false);
	}
}
