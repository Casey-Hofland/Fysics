using UnityEngine;
using System.Collections;

public abstract class PulsePack : MonoBehaviour
{
	[SerializeField] protected Rigidbody controller;

	private void Reset()
	{
		controller = GetComponentInParent<Rigidbody>();
	}

	public abstract void Shoot();
	public abstract void Charge();
}
