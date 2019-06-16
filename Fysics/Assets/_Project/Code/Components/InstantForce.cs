using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
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
		rigidbody.AddRelativeForce(force, ForceMode.Impulse);
		rigidbody.AddRelativeTorque(torque, ForceMode.Impulse);

		/*
		rigidbody.velocity = force / rigidbody.mass;
		rigidbody.angularVelocity = torque / rigidbody.mass;
		*/
	}
}
