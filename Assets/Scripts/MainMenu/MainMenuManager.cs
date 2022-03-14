using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using System;

public class MainMenuManager : MonoBehaviour
{
	public Transform titleTextTransform;
	[Tooltip("Items in order: Play, Stats, Options, Quit")]
	public Transform[] menuItems;

	private int uiLayer;
	private int previousHoverIndex = -1;

	private void Awake()
	{
		uiLayer = 1 << LayerMask.NameToLayer("UI");
	}

	private void Start()
	{
		StartCoroutine(PlayEnterMainMenu());
	}

	private void Update()
	{
		// Get position of mouse
		Vector2 pos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		Vector2 worldPos = Camera.main.ScreenToWorldPoint(pos);

		// Checks for colliders on the mouse area
		RaycastHit2D hitInfo = Physics2D.Raycast(worldPos, Vector2.zero, 10, uiLayer);

		if (hitInfo)
		{
			// Get index of current menu item being hovered
			int currIndex = Array.FindIndex(menuItems, (Transform val) => val == hitInfo.transform);

			// Shouldn't happen ever, but just in case...
			if (currIndex < 0) return;

			// Scale up card if it's a brand new button being hovered
			if (previousHoverIndex != currIndex)
			{
				CancelPreviousHoverTween();

				SoundManager.instance.PlaySound(SoundManager.Sound.Blip, false);
				previousHoverIndex = currIndex;
				hitInfo.transform.DOScale(new Vector3(1.5f, 1.5f, 1), 0.1f).SetUpdate(true).SetEase(Ease.InOutQuad);

				TextMeshPro textMesh = hitInfo.transform.GetComponent<TextMeshPro>();
				DOTween.To(() => textMesh.color, (Color to) => textMesh.color = to, Color.red, 0.1f);
			}

			if (Input.GetMouseButtonDown(0))
			{
				// TODO: Clicking the buttons functionality
			}
		}
		else
		{
			CancelPreviousHoverTween();
		}
	}

	private void CancelPreviousHoverTween()
	{
		if (previousHoverIndex != -1)
		{
			menuItems[previousHoverIndex].DOKill();
			menuItems[previousHoverIndex].DOScale(new Vector3(1f, 1f, 1), 0.1f).SetUpdate(true).SetEase(Ease.InOutQuad);

			TextMeshPro textMesh = menuItems[previousHoverIndex].transform.GetComponent<TextMeshPro>();
			DOTween.To(() => textMesh.color, (Color to) => textMesh.color = to, Color.white, 0.1f);

			previousHoverIndex = -1;
		}
	}

	private IEnumerator PlayEnterMainMenu()
	{
		titleTextTransform.gameObject.SetActive(false);
		foreach (Transform menuItem in menuItems)
			menuItem.gameObject.SetActive(false);

		PlayTextEntrance(titleTextTransform, 1.1f);
		yield return new WaitForSeconds(1.0f);
		foreach (Transform menuItem in menuItems)
		{
			PlayTextEntrance(menuItem, 0.65f);
			yield return new WaitForSeconds(0.25f);
		}
	}

	private void PlayTextEntrance(Transform textTransform, float duration)
	{
		textTransform.gameObject.SetActive(true);
		TextMeshPro text = textTransform.GetComponent<TextMeshPro>();
		Vector3 originalPos = textTransform.position;
		Color originalColor = text.color;
		Collider2D textCollider = textTransform.GetComponent<BoxCollider2D>();

		if (textCollider != null) 
			textCollider.enabled = false;

		text.color = Color.clear;

		textTransform.position = new Vector2(originalPos.x, originalPos.y - 1);

		DOTween.To(() => text.color, (Color to) => text.color = to, originalColor, duration);
		Tween transition = DOTween.To(() => textTransform.position, (Vector3 to) => textTransform.position = to, originalPos, duration);

		if (textCollider != null)
		{
			transition.OnComplete(() => textCollider.enabled = true);
		}
	}
}
