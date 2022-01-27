using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script for making sprite flash white. Sprite requires DropShadow... material
public class FlashWhite : MonoBehaviour
{
	private Renderer rend;
	private MaterialPropertyBlock block;
	private SpriteRenderer sr;

	private Coroutine stopFlashCR;
	private Coroutine spriteRendererFlashCR;

	private void Awake()
	{
		rend = GetComponent<Renderer>();
		block = new MaterialPropertyBlock();
		sr = GetComponent<SpriteRenderer>();
	}

	private void OnEnable()
	{
		rend.GetPropertyBlock(block);
		block.SetFloat("_FlashAmount", 0f);
		rend.SetPropertyBlock(block);
	}

	// Default time
	public void InitiateWhiteFlash()
	{
		InitiateWhiteFlash(0.1f);
	}

	/// <summary>
	/// Sprite turns white for given amount of time before turning back to normal
	/// </summary>
	/// <param name="whiteTime"></param>
	public void InitiateWhiteFlash(float whiteTime = 0.1f)
	{
		if (stopFlashCR != null)
			StopCoroutine(stopFlashCR);

		rend.GetPropertyBlock(block);
		block.SetFloat("_FlashAmount", 1f);
		rend.SetPropertyBlock(block);

		if (gameObject.activeInHierarchy)
			stopFlashCR = StartCoroutine(StopFlash(whiteTime));
	}

	private IEnumerator StopFlash(float whiteTime)
	{
		yield return new WaitForSeconds(whiteTime);

		rend.GetPropertyBlock(block);
		block.SetFloat("_FlashAmount", 0f);
		rend.SetPropertyBlock(block);
	}



	// Good for making sprites flash white if the object does not have the DropShadow shader AND is colored differently using SpriteRenderer.color
	public void ChangeSpriteColorToWhite()
	{
		if (spriteRendererFlashCR != null)
			StopCoroutine(spriteRendererFlashCR);

		Color originalColor = sr.color;
		sr.color = Color.white;
		if (gameObject.activeInHierarchy)
			spriteRendererFlashCR = StartCoroutine(ChangeSpriteColorToWhiteCR(0.1f, originalColor));
	}

	private IEnumerator ChangeSpriteColorToWhiteCR(float whiteTime, Color originalColor)
	{
		yield return new WaitForSeconds(whiteTime);

		sr.color = originalColor;
	}
}