using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class InspireManager : MonoBehaviour
{
	public GameObject inspireBannerPrefab;
	public float cooldown;

	private float currCooldown;

	private void Update()
	{
		currCooldown -= Time.deltaTime;

		if (currCooldown <= 0)
		{
			// Create banner
			Vector2 bannerPos = PoissonDiscSampling.GeneratePoints(2, 1)[0];

			// Set banner spawn aniamtion
			GameObject bannerObj = Instantiate(inspireBannerPrefab, bannerPos, Quaternion.identity);
			Transform circleField = bannerObj.transform.GetChild(1);
			circleField.localScale = new Vector3(0, 0, 1);
			circleField.DOScale(new Vector3(1, 1, 1), 0.25f).SetEase(Ease.OutQuad);

			bannerObj.transform.GetChild(2).GetComponent<ScaleByTween>().ActivateScale();

			ObjectPooler.instance.CreateHitParticles(Color.white, bannerPos);

			currCooldown = cooldown;
		}
	}
}
