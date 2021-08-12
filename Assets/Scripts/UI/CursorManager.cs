using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

// Custom cursor script
public class CursorManager : MonoBehaviour
{
	public Sprite cursorDefault;

	private void Update()
	{
#if !UNITY_EDITOR
		Cursor.visible = !IsInGameBounds();
#endif

		Vector3 mousePos = Input.mousePosition;
		Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
		worldPos.z = 0;

		transform.position = worldPos;
		// customCursor.anchoredPosition = mousePos;
	}

	// Returns false if the mouse is not within the bounds of the game window
	private bool IsInGameBounds()
	{
		// TEMP: Remove editor code
#if UNITY_EDITOR
		if (Input.mousePosition.x <= 0 || Input.mousePosition.y <= 0 || Input.mousePosition.x >= Handles.GetMainGameViewSize().x - 1 || Input.mousePosition.y >= Handles.GetMainGameViewSize().y - 1)
		{
			return false;
		}
#else
		if (Input.mousePosition.x <= 0 || Input.mousePosition.y <= 0 || Input.mousePosition.x >= Screen.width - 1 || Input.mousePosition.y >= Screen.height - 1)
		{
			return false;
		}
#endif
		else
		{
			return true;
		}
	}
}