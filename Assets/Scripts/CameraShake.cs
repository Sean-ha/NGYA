using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
	public static CameraShake instance;

	private Transform cam;

	private Vector3 defaultPos = new Vector3(0, 0, -10);
	private Quaternion defaultRot = Quaternion.identity;

	public bool canShake { get; set; } = true;
	private bool isShaking;
	private float currentDuration;
	private float currentMagnitude;

	private Coroutine shakeCR;

	private void Awake()
	{
		instance = this;
		cam = Camera.main.transform;
	}

	/// <summary>
	/// Default values (0.1f, 0.1f)
	/// </summary>
	public void ShakeCamera()
	{
		if (canShake)
			ShakeCamera(0.1f, 0.1f);
	}

	public void ShakeCamera(float duration, float magnitude)
	{
		if (!canShake)
			return;

		if (isShaking && magnitude < currentMagnitude)
			return;

		currentMagnitude = magnitude;
		currentDuration = duration;

		StopAllCoroutines();
		cam.position = defaultPos; //Reset to original postion
		cam.rotation = defaultRot; //Reset to original rotation

		shakeCR = StartCoroutine(Shake());
	}

	public void CancelShake()
	{
		if (shakeCR != null)
			StopCoroutine(shakeCR);
	}

	/// <summary>
	/// Shake camera at the given angle (in degrees)
	/// </summary>
	/// <param name="angle">Angle in degrees</param>
	public void ShakeCameraDirectional(float duration, float angle, float magnitude)
	{
		if (!canShake)
			return;

		if (isShaking && magnitude < currentMagnitude)
			return;

		currentMagnitude = magnitude;
		currentDuration = duration;

		StopAllCoroutines();
		cam.position = defaultPos; //Reset to original postion
		cam.rotation = defaultRot; //Reset to original rotation

		shakeCR = StartCoroutine(ShakeDirectional(duration, -angle, magnitude));
	}

	private IEnumerator Shake()
	{
		float counter = 0f;

		//Angle Rotation
		const float angleRot = 0.05f;

		float decreasePoint = currentDuration / 2;

		isShaking = true;
		//Do the actual shaking
		while (counter < currentDuration)
		{
			counter += Time.deltaTime;
			float decreaseSpeed = currentMagnitude;
			float decreaseAngle = angleRot;

			//Shake camera
			Vector3 tempPos = defaultPos + Random.insideUnitSphere * decreaseSpeed * 0.5f;
			tempPos.z = defaultPos.z;
			cam.position = tempPos;

			cam.rotation = defaultRot * Quaternion.AngleAxis(Random.Range(-angleRot, angleRot), new Vector3(0f, 0f, 1f));
			yield return null;

			//Check if we have reached the decreasePoint then start decreasing  decreaseSpeed value
			if (counter >= decreasePoint)
			{
				//Reset counter to 0 
				counter = 0f;
				while (counter <= decreasePoint)
				{
					counter += Time.deltaTime;
					decreaseSpeed = Mathf.Lerp(currentMagnitude, 0, counter / decreasePoint);
					decreaseAngle = Mathf.Lerp(angleRot, 0, counter / decreasePoint);

					// Shake camera
					tempPos = defaultPos + Random.insideUnitSphere * decreaseSpeed;
					tempPos.z = defaultPos.z;
					cam.position = tempPos;

					cam.rotation = defaultRot * Quaternion.AngleAxis(Random.Range(-decreaseAngle, decreaseAngle), new Vector3(0f, 0f, 1f));

					yield return null;
				}

				//Break from the outer loop
				break;
			}
		}
		cam.position = defaultPos; //Reset to original postion
		cam.rotation = defaultRot; //Reset to original rotation
		isShaking = false;
	}

	private IEnumerator ShakeDirectional(float duration, float angle, float magnitude)
	{
		float counter = 0f;

		float decreasePoint = duration / 2.5f;

		float xVal = Mathf.Sin(angle * Mathf.Deg2Rad);
		float yVal = Mathf.Cos(angle * Mathf.Deg2Rad);
		Vector3 dir = new Vector3(xVal, yVal, 0);

		//Do the actual shaking
		while (counter < duration)
		{
			counter += Time.deltaTime;
			float decreaseSpeed = magnitude;

			//Shake camera
			Vector3 tempPos = defaultPos + dir * -0.6f * decreaseSpeed;
			tempPos.z = defaultPos.z;
			cam.position = tempPos;

			yield return null;


			//Check if we have reached the decreasePoint then start decreasing decreaseSpeed value
			if (counter >= decreasePoint)
			{
				//Reset counter to 0 
				counter = 0f;
				while (counter <= decreasePoint)
				{
					counter += Time.deltaTime;
					decreaseSpeed = Mathf.Lerp(magnitude, 0, counter / decreasePoint);

					// Shake camera
					tempPos = defaultPos + dir * Random.Range(-1f, 1f) * decreaseSpeed;
					tempPos.z = defaultPos.z;
					cam.position = tempPos;

					yield return null;
				}

				//Break from the outer loop
				break;
			}
		}
		cam.position = defaultPos; //Reset to original postion
		cam.rotation = defaultRot; //Reset to original rotation
	}
}