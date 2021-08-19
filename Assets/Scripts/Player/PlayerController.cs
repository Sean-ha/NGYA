using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
	public static PlayerController instance;

	public GameObject levelUpText;
	public float moveSpeed;


	private HealthSystem healthSystem;
	private Rigidbody2D rb;
	private SpriteRenderer sr;

	private float horizontal;
	private float vertical;
	private bool canMove = true;
	// If set to true, will zero out velocity on the next FixedUpdate.
	private bool zeroVelocity;
	private Coroutine disableMovementCR;

	public float knockBackForce { get; set; } = 500f;

	// Duration of invincibility in seconds
	public float invincibilityDuration { get; set; } = 1f;
	// Duration of current invincibility. <=0 means player can be hit.
	private float currentInvincibility;
	public UnityEvent onTakeDamage { get; set; } = new UnityEvent();

	private Collider2D[] expShells = new Collider2D[10];
	private int expShellLayer;

	private GameObject levelUpSquare;
	private GameObject levelUpParticles;

	// Time it takes for stand still effects to take effect
	private float standStillDuration = 1.5f;
	private float currentStandStillDuration;
	public UnityEvent onStandStill { get; set; } = new UnityEvent();
	// Invoked when player moves after standing still effect activates
	public UnityEvent onCancelStandStill { get; set; } = new UnityEvent();

	private void Awake()
	{
		instance = this;

		// TODO: Move this stuff somewhere else probably
		Application.targetFrameRate = 60;
		DOTween.Init(useSafeMode:false).SetCapacity(500, 50);
		DOTween.defaultEaseType = Ease.Linear;
		DOTween.useSafeMode = true;

		rb = GetComponent<Rigidbody2D>();
		sr = GetComponent<SpriteRenderer>();

		expShellLayer = LayerMask.GetMask("Exp");

		levelUpSquare = transform.GetChild(0).gameObject;
		levelUpParticles = transform.GetChild(1).gameObject;

		onStandStill.AddListener(() => print("STILL!!"));
		onCancelStandStill.AddListener(() => print("moving again"));
	}

	private void Start()
	{
		healthSystem = HealthSystem.instance;
	}

	private void Update()
	{
		horizontal = Input.GetAxis("Horizontal");
		vertical = Input.GetAxis("Vertical");

		if (currentInvincibility > 0)
		{
			currentInvincibility -= Time.deltaTime;
		}

		if (horizontal == 0 && vertical == 0 && currentStandStillDuration < standStillDuration)
		{
			currentStandStillDuration += Time.deltaTime;
			if (currentStandStillDuration >= standStillDuration)
			{
				onStandStill.Invoke();
			}
		}
		else if (horizontal != 0 || vertical != 0)
		{
			CancelStandStill();
		}

#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.LeftShift))
		{
			Time.timeScale = 5;
		}
		else if (Input.GetKeyUp(KeyCode.LeftShift))
		{
			Time.timeScale = 1;
		}
		if (Input.GetKeyDown(KeyCode.RightShift))
		{
			TimeManager.instance.SlowToPause(() =>
			{
			});
		}
		else if (Input.GetKeyUp(KeyCode.RightShift))
		{
			TimeManager.instance.SlowToUnpause();
		}
#endif

		CheckForExp();
	}

	private void FixedUpdate()
	{
		if (zeroVelocity)
		{
			rb.velocity = Vector2.zero;
			zeroVelocity = false;
		}
		if (!canMove)
			return;

		Vector2 velVector = new Vector2(horizontal, vertical);
		if (velVector.magnitude > 1)
		{
			velVector /= velVector.magnitude;
		}
		velVector *= moveSpeed;

		rb.velocity = velVector;
	}

	private void CancelStandStill()
	{
		if (currentStandStillDuration >= standStillDuration)
			onCancelStandStill.Invoke();

		currentStandStillDuration = 0;
	}

	private void CheckForExp()
	{
		Physics2D.OverlapCircleNonAlloc(transform.position, 0.2f, expShells, expShellLayer);

		for (int i = 0; i < expShells.Length; i++)
		{
			if (expShells[i] != null)
			{
				ExpManager.instance.GainExp(expShells[i].GetComponent<ExpShell>().expAmount);
				expShells[i].gameObject.SetActive(false);
				expShells[i] = null;
				// SoundManager.instance.PlayExpPickupSound();
				SoundManager.instance.PlaySound(SoundManager.Sound.PickupExp);
			}
		}
	}

	private void OnCollisionStay2D(Collision2D collision)
	{
		OnTriggerStay2D(collision.collider);
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.gameObject.layer == 7)
		{
			if (currentInvincibility <= 0)
			{
				float toTake = collision.GetComponent<Damager>().damage;
				healthSystem.TakeDamage(toTake);
				TakeDamageEffects();
				CameraShake.instance.ShakeCamera(0.2f, 0.3f);
				SoundManager.instance.PlaySound(SoundManager.Sound.PlayerHit);

				if (!Mathf.Approximately(0, knockBackForce))
				{
					// Get knocked back (and cancel standstill)
					Vector2 diff = (transform.position - collision.transform.position).normalized;
					Vector2 force = diff * knockBackForce;

					DisableMovement(0.15f);
					rb.AddForce(force);
					CancelStandStill();
				}

				if (UpgradesManager.instance.obtainedUpgrades.Contains(Upgrade.Blowback))
				{
					Instantiate(GameAssets.instance.blowbackExplosion, collision.ClosestPoint(transform.position), Quaternion.identity);
				}

				onTakeDamage.Invoke();

				currentInvincibility = invincibilityDuration;
			}
		}
	}

	// Sprite blinking effect after taking damage
	public void TakeDamageEffects()
	{
		StartCoroutine(BlinkSprite());
		// StartCoroutine(ChromaticAberration());
	}

	private IEnumerator BlinkSprite()
	{
		int count = Mathf.RoundToInt(invincibilityDuration / 0.2f);
		for (int i = 0; i < count; i++)
		{
			sr.enabled = false;
			yield return new WaitForSeconds(0.1f);
			sr.enabled = true;
			yield return new WaitForSeconds(0.1f);
		}
	}

	/*
	private IEnumerator ChromaticAberration()
	{
		sr.sharedMaterial.SetFloat("_OffsetBlueX", -.1f);
		sr.sharedMaterial.SetFloat("_OffsetRedX", .1f);
		yield return new WaitForSeconds(0.2f);

		LeanTween.value(gameObject, -.1f, 0, 0.2f).setOnUpdate((float val) =>
		{
			sr.sharedMaterial.SetFloat("_OffsetBlueX", val);
		});
		LeanTween.value(gameObject, .1f, 0, 0.2f).setOnUpdate((float val) =>
		{
			sr.sharedMaterial.SetFloat("_OffsetRedX", val);
		});
	}
	*/

	// Disables player movement for time seconds. Note that as soon as movement is disabled, the player abruptly stops moving.
	private void DisableMovement(float time)
	{
		if (disableMovementCR != null)
			StopCoroutine(disableMovementCR);
		disableMovementCR = StartCoroutine(DisableMovementCR(time));
	}

	private IEnumerator DisableMovementCR(float time)
	{
		canMove = false;
		zeroVelocity = true;
		yield return new WaitForSeconds(time);
		canMove = true;
	}

	/// <summary>
	/// Get the position the player will be in 'time' seconds given their current movement
	/// </summary>
	/// <param name="time">Number of seconds</param>
	/// <returns></returns>
	public Vector2 GetForwardPosition(float time)
	{
		Vector2 vel = rb.velocity * time;
		return (Vector2)transform.position + vel;
	}

	public Vector2 GetBackwardPosition(float time)
	{
		Vector2 vel = rb.velocity * time;
		return (Vector2)transform.position - vel;
	}

	public void LevelUp()
	{
		levelUpSquare.SetActive(true);
		levelUpParticles.SetActive(true);
		Instantiate(levelUpText, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
		SoundManager.instance.PlaySound(SoundManager.Sound.LevelUp);

		TimeManager.instance.SlowToPause(() =>
		{
			UpgradesManager.instance.DisplayUpgradesWindow();
		});
	}
}
