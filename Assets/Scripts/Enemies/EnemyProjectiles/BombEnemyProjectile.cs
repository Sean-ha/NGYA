using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BombEnemyProjectile : MonoBehaviour
{
	public GameObject projectileIndicator;

	private GameObject colliderObject;

	private void Awake()
	{
		colliderObject = transform.GetChild(1).gameObject;
	}

	public void SetProjectile(Vector2 destination, float timeToReach, float damage)
	{
		// Get jump height between this and destination
		float dist = Vector2.Distance(transform.position, destination);
		float height = Random.Range(3f, 3.6f);

		float rotation = dist * 180;
		// Spin clockwise
		if (destination.x > transform.position.x)
			rotation *= -1;

		transform.DOLocalRotate(new Vector3(0, 0, rotation), timeToReach, RotateMode.FastBeyond360);

		colliderObject.GetComponent<Damager>().damage = damage;

		transform.DOLocalJump(destination, height, 1, timeToReach).OnComplete(CreateExplosion);

		GameObject indicator = Instantiate(projectileIndicator, destination, Quaternion.identity);
		indicator.transform.localScale = new Vector3(0, 0, 1);
		indicator.transform.DOScale(new Vector3(0.6f, 0.6f, 1), timeToReach).OnComplete(() => Destroy(indicator));
	}

	private void CreateExplosion()
	{
		CameraShake.instance.ShakeCamera(0.2f, 0.3f);

		// Creates circle effect that starts white and turns red
		SpriteRenderer circleEffect = ObjectPooler.instance.CreateCircleHitEffect(Color.white, transform.position, 3.5f).GetComponent<SpriteRenderer>();
		Sequence seq = DOTween.Sequence();
		seq.AppendInterval(0.05f);
		seq.AppendCallback(() => circleEffect.color = Color.red);

		Transform explosionParticles = transform.GetChild(0);
		explosionParticles.gameObject.SetActive(true);
		explosionParticles.parent = null;

		colliderObject.SetActive(true);
		colliderObject.transform.parent = null;
		Destroy(colliderObject, 0.1f);

		Destroy(gameObject);
	}
}
