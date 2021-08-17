using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script for making sprite flash white. Sprite requires DropShadow... material
public class FlashWhite : MonoBehaviour
{
	private Renderer rend;
	private MaterialPropertyBlock block;

	private Coroutine stopFlashCR;

	private void Awake()
	{
		rend = GetComponent<Renderer>();
		block = new MaterialPropertyBlock();
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
}