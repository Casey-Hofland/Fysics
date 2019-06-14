using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace BzKovSoft.RagdollTemplate.Scripts.Charachter
{
	public class LimbCollision : MonoBehaviour
	{
		[SerializeField] private float minMagnitude = 30f;

		[SerializeField] private BzHealth _bzHealth;
		[SerializeField] private Rigidbody rigidbody;

		private Rigidbody rootRigidbody;

		private float sqrMinMagnitude { get { return minMagnitude * minMagnitude;} }

		private void Start()
		{
			rootRigidbody = transform.root.GetChild(1).GetComponent<Rigidbody>();
			Debug.Log(rootRigidbody.name);
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (collision.transform.root != transform.root && 
				collision.impulse.sqrMagnitude >= sqrMinMagnitude)
			{
				_bzHealth.Health -= 101f;
				rootRigidbody.AddForce(-collision.impulse * 2.5f + Vector3.up * 25f, ForceMode.Impulse);
				rigidbody.AddForce(-collision.impulse * 2f, ForceMode.Impulse);
			}
		}

		private void Reset()
		{
			rigidbody = GetComponent<Rigidbody>();
		}
	}
}
