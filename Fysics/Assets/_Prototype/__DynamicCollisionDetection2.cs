using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class __DynamicCollisionDetection2 : MonoBehaviour
{
	private readonly float continuousDynamicSqrMagnitude = Mathf.Pow(30f, 2);
	private readonly float continuousSqrMagnitude = Mathf.Pow(5f, 2);

	private Rigidbody rigidbody;
	private bool lastIsSleeping;
	private Vector3 lastVelocity = Vector3.zero;

	private void Awake()
	{
		rigidbody = GetComponent<Rigidbody>();
	}

	private void OnEnable()
	{
		rigidbody.Sleep();
		lastIsSleeping = false;
	}

	private void FixedUpdate()
	{
		bool isSleeping = rigidbody.IsSleeping();

		if (!isSleeping)
		{
			rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		}
		else if (!lastIsSleeping)
		{
			rigidbody.WakeUp();
			rigidbody.AddForce(lastVelocity, ForceMode.VelocityChange);
			StartCoroutine(UpdateCollisionDetectionMode());
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		rigidbody.WakeUp();
	}

	IEnumerator UpdateCollisionDetectionMode()
	{
		yield return new WaitForFixedUpdate();

		rigidbody.collisionDetectionMode = DynamicCollisionDetectionMode();
		lastIsSleeping = rigidbody.IsSleeping();
		lastVelocity = rigidbody.velocity;
		rigidbody.Sleep();
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
}
