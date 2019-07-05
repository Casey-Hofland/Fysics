using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class __DynamicCollisionDetection : MonoBehaviour
{
	private readonly float continuousDynamicSqrMagnitude = Mathf.Pow(30f, 2);
	private readonly float continuousSqrMagnitude = Mathf.Pow(5f, 2);

	private Rigidbody rigidbody;
	private bool firstFrame;
	private bool lastIsSleeping;

	private void Awake()
	{
		rigidbody = GetComponent<Rigidbody>();
	}

	private void OnEnable()
	{
		rigidbody.Sleep();
		firstFrame = true;
		lastIsSleeping = true;
	}

	private void OnCollisionEnter(Collision collision)
	{
		rigidbody.WakeUp();
	}

	private void FixedUpdate()
	{
		bool isSleeping = rigidbody.IsSleeping();
		if (firstFrame && isSleeping)
		{
			firstFrame = false;
			rigidbody.WakeUp();
			isSleeping = false;
			lastIsSleeping = isSleeping;
		}

		if (!isSleeping)
		{
			if (lastIsSleeping != isSleeping)
			{
				rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
			}
			else
			{
				StartCoroutine(UpdateCollisionDetectionMode());
			}
		}

		lastIsSleeping = isSleeping;
	}

	private CollisionDetectionMode DynamicCollisionDetectionMode()
	{
		switch (rigidbody.velocity.sqrMagnitude)
		{
			case float f when f >= continuousDynamicSqrMagnitude:
				return CollisionDetectionMode.ContinuousDynamic;
			case float f when f >= continuousSqrMagnitude:
				return CollisionDetectionMode.Continuous;
			default:
				return CollisionDetectionMode.Discrete;
		}
	}

	IEnumerator UpdateCollisionDetectionMode()
	{
		yield return new WaitForFixedUpdate();

		rigidbody.collisionDetectionMode = DynamicCollisionDetectionMode();
	}
}
