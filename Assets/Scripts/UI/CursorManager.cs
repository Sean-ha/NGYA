using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

// Custom cursor script
public class CursorManager : MonoBehaviour
{
	public static CursorManager instance;

	public Sprite cursorDefault;
	public float spinsPerSecond;
	public float cursorExpandAmount;

	private float degreesPerFrame;
	private float currAngle;

	// Order of array: Top, Bot, Left, Right
	private Transform[] crosshairPieces = new Transform[4];

	// Hardcoded vector positions of the cursor piece original locations
	private Vector2[] defaultPositions = { new Vector2(0, 0.2f), new Vector2(0, -0.2f), new Vector2(-0.2f, 0), new Vector2(0.2f, 0) };

	private void Awake()
	{
		instance = this;

		degreesPerFrame = spinsPerSecond * 360 / Application.targetFrameRate;

		for (int i = 0; i < crosshairPieces.Length; i++)
			crosshairPieces[i] = transform.GetChild(i);
	}

	private void Update()
	{
#if !UNITY_EDITOR
		Cursor.visible = !IsInGameBounds();
#endif

		// Spin regardless of delta time
		if (currAngle > 360 || currAngle < -360)
			currAngle %= 360;
		currAngle += degreesPerFrame;
		transform.localRotation = Quaternion.Euler(0, 0, currAngle);

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

	public void AnimateCursorShoot()
	{
		float outTime = 0.03f;
		float inTime = 0.08f;

		foreach (Transform piece in crosshairPieces)
			piece.DOKill();

		crosshairPieces[0].DOLocalMoveY(defaultPositions[0].y + cursorExpandAmount, outTime).OnComplete(() => crosshairPieces[0].DOLocalMove(defaultPositions[0], inTime).SetUpdate(true)).SetUpdate(true);

		crosshairPieces[1].DOLocalMoveY(defaultPositions[1].y - cursorExpandAmount, outTime).OnComplete(() => crosshairPieces[1].DOLocalMove(defaultPositions[1], inTime).SetUpdate(true)).SetUpdate(true);

		crosshairPieces[2].DOLocalMoveX(defaultPositions[2].x - cursorExpandAmount, outTime).OnComplete(() => crosshairPieces[2].DOLocalMove(defaultPositions[2], inTime).SetUpdate(true)).SetUpdate(true);

		crosshairPieces[3].DOLocalMoveX(defaultPositions[3].x + cursorExpandAmount, outTime).OnComplete(() => crosshairPieces[3].DOLocalMove(defaultPositions[3], inTime).SetUpdate(true)).SetUpdate(true);
	}
}