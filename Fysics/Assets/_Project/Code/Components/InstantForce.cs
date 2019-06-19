using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class InstantForce : MonoBehaviour
{
	[SerializeField] private Vector3 force = Vector3.zero;
	[SerializeField] private Vector3 torque = Vector3.zero;

	private Rigidbody rigidbody;

	private void Start()
	{
		rigidbody = GetComponent<Rigidbody>();

		rigidbody.AddRelativeForce(force, ForceMode.Impulse);
		rigidbody.AddRelativeTorque(torque, ForceMode.Impulse);
	}
}
