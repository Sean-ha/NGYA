using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class ExploderEnemy : MonoBehaviour
{
	public float explosionRadius;
	public Sprite hurtSprite;

	public GameObject mySprite;
	public GameObject colliderObject;
	public GameObject explosionParticles;

	public UnityEvent onExplosionComplete;

	public void Explode()
	{
		// Set hurt sprite
		mySprite.GetComponent<SpriteRenderer>().sprite = hurtSprite;
		// Disable movement
		GetComponent<MoveTowardsPlayer>().enabled = false;
		// Disable spinning
		GetComponent<Spin>().enabled = false;

		StartCoroutine(ExplodeCR());
	}

	private IEnumerator ExplodeCR()
	{
		ParticleSystem.ShapeModule shape = explosionParticles.GetComponent<ParticleSystem>().shape;
		shape.radius = explosionRadius;
		colliderObject.transform.localScale = new Vector3(explosionRadius, explosionRadius, 1f);
		colliderObject.GetComponent<Damager>().damage = GetComponent<Damager>().damage;

		mySprite.transform.DOPunchScale(new Vector3(0.3f, 0.3f, 0f), 1.4f, vibrato: 7);

		yield return new WaitForSeconds(1.4f);

		colliderObject.SetActive(true);
		explosionParticles.SetActive(true);
		explosionParticles.transform.parent = null;
		mySprite.SetActive(false);
		ObjectPooler.instance.CreateExpandingExplosion(transform.position, Color.red, explosionRadius);
		SoundManager.instance.PlaySound(SoundManager.Sound.EnemyExplosion1);

		yield return new WaitForSeconds(0.1f);

		Destroy(gameObject);
	}
}
