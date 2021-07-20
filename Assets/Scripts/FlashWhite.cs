using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script for making sprite flash white. Sprite requires DropShadow... material
public class FlashWhite : MonoBehaviour
{
	private SpriteRenderer sr;
	private MaterialPropertyBlock block;

	private void Awake()
	{
		sr = GetComponent<SpriteRenderer>();
		block = new MaterialPropertyBlock();
	}

	private void OnEnable()
	{
		sr.GetPropertyBlock(block);
		block.SetFloat("_FlashAmount", 0f);
		sr.SetPropertyBlock(block);
	}

	// Default time
	public void InitiateWhiteFlash()
	{
		sr.GetPropertyBlock(block);
		block.SetFloat("_FlashAmount", 1f);
		sr.SetPropertyBlock(block);

		if (gameObject.activeInHierarchy)
			StartCoroutine(StopFlash(0.1f));
	}

	/// <summary>
	/// Sprite turns white for given amount of time before turning back to normal
	/// </summary>
	/// <param name="whiteTime"></param>
	public void InitiateWhiteFlash(float whiteTime = 0.1f)
	{
		sr.GetPropertyBlock(block);
		block.SetFloat("_FlashAmount", 1f);
		sr.SetPropertyBlock(block);

		if (gameObject.activeInHierarchy)
			StartCoroutine(StopFlash(whiteTime));
	}

	private IEnumerator StopFlash(float whiteTime)
	{
		yield return new WaitForSeconds(whiteTime);

		sr.GetPropertyBlock(block);
		block.SetFloat("_FlashAmount", 0f);
		sr.SetPropertyBlock(block);
	}
}