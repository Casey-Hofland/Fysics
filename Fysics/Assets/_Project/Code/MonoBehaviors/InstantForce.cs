using UnityEngine;

public class InstantForce : MonoBehaviour
{
	[SerializeField] private Vector3 force = Vector3.zero;
	[SerializeField] private Vector3 torque = Vector3.zero;

	private new Rigidbody rigidbody;

	private void Awake()
	{
		rigidbody = GetComponent<Rigidbody>();
	}

	private void Start()
	{
		rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		rigidbody.AddRelativeForce(force, ForceMode.Impulse);
		rigidbody.AddRelativeTorque(torque, ForceMode.Impulse);
	}
}
