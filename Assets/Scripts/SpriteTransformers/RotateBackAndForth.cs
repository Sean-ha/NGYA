using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RotateBackAndForth : MonoBehaviour
{
	[Tooltip("Time it takes to rotate towards one side")]
	public float time;
	[Tooltip("Angle in degrees which is the max in either direction to rotate")]
	public float angle;
	[Tooltip("Should this tween animate when paused?")]
	public bool ignoreTimeScale;
	[Tooltip("Is this a physics object that needs to be animated in FixedUpdate?")]
	public bool useFixedUpdate;

	private void Start()
	{
		RotateThis();
	}

	public void RotateThis()
	{
		Tween t1 = transform.DORotate(new Vector3(0, 0, angle), 2 * time).SetEase(Ease.InOutQuad).OnComplete(() =>
		{
			Tween t2 = transform.DORotate(new Vector3(0, 0, -angle), 2 * time).SetEase(Ease.InOutQuad).OnComplete(() => RotateThis());
			if (useFixedUpdate)
				t2.SetUpdate(UpdateType.Fixed, ignoreTimeScale);
			else
				t2.SetUpdate(ignoreTimeScale);
		});
		if (useFixedUpdate)
			t1.SetUpdate(UpdateType.Fixed, ignoreTimeScale);
		else
			t1.SetUpdate(ignoreTimeScale);
	}
}
