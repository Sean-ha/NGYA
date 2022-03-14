using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperFunctions
{
	readonly static Vector2 topRight = new Vector2(19.8f, 11f);
	readonly static Vector2 bottomLeft = new Vector2(-19.8f, -11f);

	// Given a Vector2, return true if it is within the game bounds, or false if not
	public static bool IsWithinGameBounds(Vector2 position)
	{
		if (bottomLeft.x < position.x && position.x < topRight.x)
		{
			if (bottomLeft.y < position.y && position.y < topRight.y)
			{
				return true;
			}
		}
		return false;
	}

	// withinGameBounds: True if the random point must be in game bounds, false if not
	public static Vector2 RandomPointInAnnulus(Vector2 origin, float minRadius, float maxRadius)
	{
		Vector2 randomDirection = (Random.insideUnitCircle * origin).normalized;
		float randomDistance = Random.Range(minRadius, maxRadius);
		Vector2 point = origin + randomDirection * randomDistance;

		if (!IsWithinGameBounds(point))
		{
			
		}

		return point;
	}

	// Randomly rerolls until the random point is within game bounds. Max possible rerolls can be specified
	public static Vector2 RandomPointInAnnulusWithinGameBounds(Vector2 origin, float minRadius, float maxRadius, int maxRerolls = 15)
	{
		Vector2 testPoint = RandomPointInAnnulus(origin, minRadius, maxRadius);
		for (int i = 1; i < maxRerolls; i++)
		{
			if (IsWithinGameBounds(testPoint))
				break;
			testPoint = RandomPointInAnnulus(origin, minRadius, maxRadius);
		}

		return testPoint;
	}

	// Returns the angle in degrees from startPos to endPos
	public static float GetDAngleTowards(Vector2 startPos, Vector2 endPos)
	{
		Vector2 diff = endPos - startPos;

		return Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
	}

	public static void BlinkSpriteRenderer(SpriteRenderer sr, int blinkCount, float timeOff, float timeOn)
	{
		Sequence s = DOTween.Sequence();
		
		for (int i = 0; i < blinkCount; i++)
		{
			s.AppendCallback(() => sr.enabled = false);
			s.AppendInterval(timeOff);
			s.AppendCallback(() => sr.enabled = true);
			s.AppendInterval(timeOn);
		}
		s.Play();
	}

	// Returns true if the given position is outside of the game bounds (i.e. outside of the edges of the arena)
	public static bool IsOutOfBounds(Vector2 pos)
	{
		if (pos.x > 20.1f || pos.x < -20.1f || pos.y > 11.3f || pos.y < -11.3f)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
}
