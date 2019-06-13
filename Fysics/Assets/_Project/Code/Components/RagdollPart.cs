using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollPart : MonoBehaviour
{
	private Rigidbody rigidbody;
	private Collider collider;

	private RagdollBehaviour rb;

	private void Start()
	{
		rigidbody = GetComponent<Rigidbody>();
		collider = GetComponent<Collider>();
		rb = GetComponentInParent<RagdollBehaviour>();

		SetActive(false);
	}

	public void SetActive(bool value)
	{
		rigidbody.useGravity = value;
		rigidbody.isKinematic = !value;
		collider.enabled = value;
	}

	private void OnCollisionEnter(Collision collision)
	{
		StartCoroutine(Wait(2f));
	}

	IEnumerator Wait(float seconds)
	{
		rb.RagdollSetActive(true);
		SetActive(true);
		yield return new WaitForSeconds(seconds);
		rb.RagdollSetActive(false);
		SetActive(false);
	}
}
