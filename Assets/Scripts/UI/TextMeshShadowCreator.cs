using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NaughtyAttributes;

// Place this component on a TextMesh object to give it a shadow
public class TextMeshShadowCreator : MonoBehaviour
{
	[Tooltip("Set this to true if this text object can and will change at some point (this includes changing text, changing font size, etc.)")]
	public bool dynamicText;

	public bool overrideSortingLayer;
	[EnableIf("overrideSortingLayer")]
	public int newSortingLayer;

	private TextMeshPro myTextMesh;
	private GameObject shadowObject;

	private void Awake()
	{
		myTextMesh = GetComponent<TextMeshPro>();
	}

	private void Start()
	{
		shadowObject = Instantiate(GameAssets.instance.textMeshShadow, transform, false);

		if (overrideSortingLayer)
		{
			shadowObject.GetComponent<MeshRenderer>().sortingOrder = newSortingLayer;
		}

		if (dynamicText)
			shadowObject.AddComponent<TextMeshShadowUpdater>();

		ForceRefreshShadow();
	}

	public void ForceRefreshShadow()
	{
		// If shadowObject is null, this object has never even been activated. So no need to refresh; it will automatically refresh when SetActive
		if (shadowObject == null)
			return;

		// Set shadow properties to be identical to this text
		TextMeshPro textComponent = shadowObject.GetComponent<TextMeshPro>();
		textComponent.text = myTextMesh.text;
		textComponent.fontSize = myTextMesh.fontSize;
		textComponent.fontStyle = myTextMesh.fontStyle;
		textComponent.alignment = myTextMesh.alignment;
		textComponent.color = new Color(0, 0, 0, myTextMesh.color.a);
		textComponent.rectTransform.sizeDelta = myTextMesh.rectTransform.sizeDelta;

		textComponent.ForceMeshUpdate();
	}
}
