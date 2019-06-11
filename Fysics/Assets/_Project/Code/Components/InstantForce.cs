using UnityEngine;
using UnityEditor;

[DisallowMultipleComponent]
[CanEditMultipleObjects]
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
		rigidbody.velocity = force / rigidbody.mass;
		rigidbody.angularVelocity = torque / rigidbody.mass;
	}
}
