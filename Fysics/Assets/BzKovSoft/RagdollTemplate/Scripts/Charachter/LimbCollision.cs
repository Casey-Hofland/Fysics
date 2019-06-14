using UnityEngine;

namespace BzKovSoft.RagdollTemplate.Scripts.Charachter
{
	public class LimbCollision : MonoBehaviour
	{
		[SerializeField] private float minMagnitude = 30f;

		[SerializeField] private BzHealth _bzHealth;
		[SerializeField] private Rigidbody rigidbody;

		private float sqrMinMagnitude { get { return minMagnitude * minMagnitude;} }

		private void OnCollisionEnter(Collision collision)
		{
			if (collision.impulse.sqrMagnitude >= sqrMinMagnitude)
			{
				_bzHealth.Health -= 101f;
				rigidbody.AddForce(-collision.impulse, ForceMode.Impulse);
			}
		}

		private void Reset()
		{
			rigidbody = GetComponent<Rigidbody>();
		}
	}
}
