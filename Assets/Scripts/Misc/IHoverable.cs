using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHoverable
{
	/// <summary>
	/// Called once when the mouse enters the collider
	/// </summary>
	void OnMouseEnter();

	/// <summary>
	/// Called every frame the mouse is hovering over this collider
	/// </summary>
	void OnMouseStay();

	/// <summary>
	/// Called once when the mouse exits this collider
	/// </summary>
	void OnMouseExit();
}
