using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NOTE: UNNECESSARY... UNITY HAS BUILT-IN METHODS FOR MOUSE-OVER ON COLLIDERS

// Manages hovering over hoverable objects. Ideally, no two hoverables should ever overlap. Only one hoverable can be active at a time.
public class MouseHoverManager : MonoBehaviour
{
	private int uiLayer;
	private IHoverable previousHoverable;

	private void Awake()
	{
		uiLayer = 1 << LayerMask.NameToLayer("UI");
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
			// Get current object being hovered
			IHoverable currHoverable = hitInfo.transform.gameObject.GetComponent<IHoverable>();

			// Object is hoverable
			if (currHoverable != null)
			{
				// Different hoverable; initiate the new hoverable action (and cancel the previous one)
				if (currHoverable != previousHoverable)
				{
					print("loc1");
					CancelPreviousHover();
					previousHoverable = currHoverable;
					currHoverable.OnMouseEnter();
				}
				// Same hoverable. Initiate "stay" action
				else 
				{
					print("loc2");
					currHoverable.OnMouseStay();
				}
			}
			// Hovering over non-hoverable UI. Cancel previous hover.
			else
			{
				print("loc3");
				CancelPreviousHover();
			}
		}
		// Nothing being hovered; cancel previous hover.
		else
		{
			print("loc4");
			CancelPreviousHover();
		}
	}

	private void CancelPreviousHover()
	{
		if (previousHoverable != null)
		{
			previousHoverable.OnMouseExit();
			previousHoverable = null;
		}
	}
}
