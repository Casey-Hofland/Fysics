using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class DynamicCollisionDetection : MonoBehaviour
{
	private float continuousDynamicSqrMagnitude = Mathf.Pow(30f, 2);
	private float continuousSqrMagnitude = Mathf.Pow(5f, 2);

	private new Rigidbody rigidbody;
	private bool overridden;

	private bool valid { get { return (rigidbody && !rigidbody.isKinematic && !overridden); } }

	private void Awake()
	{
		rigidbody = GetComponent<Rigidbody>();
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!valid) return;

		rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
	}

	private void FixedUpdate()
	{
		if (!valid) return;

		switch (rigidbody.velocity.sqrMagnitude)
		{
			case float f when f > continuousDynamicSqrMagnitude:
				rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
				break;
			case float f when f > continuousSqrMagnitude:
				rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
				break;
			default:
				rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
				break;
		}
	}

	private void LateUpdate()
	{
		overridden = false;
	}

	// Overrides the collision detection mode for a single frame.
	public void OverrideFrame(CollisionDetectionMode collisionDetectionMode)
	{
		if (!valid) return;

		rigidbody.collisionDetectionMode = collisionDetectionMode;
		overridden = true;
	}
}
