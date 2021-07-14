using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script for making sprite flash white. Sprite requires DropShadow... material
public class FlashWhite : MonoBehaviour
{
	// Time for sprite to be white
	private const float WHITE_TIME = 0.08f;
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

	public void InitiateWhiteFlash()
	{
		sr.GetPropertyBlock(block);
		block.SetFloat("_FlashAmount", 1f);
		sr.SetPropertyBlock(block);

		if (gameObject.activeInHierarchy)
			StartCoroutine(StopFlash());
	}

	private IEnumerator StopFlash()
	{
		yield return new WaitForSeconds(WHITE_TIME);

		sr.GetPropertyBlock(block);
		block.SetFloat("_FlashAmount", 0f);
		sr.SetPropertyBlock(block);
	}
}