using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseGunController : MonoBehaviour
{
	// TODO : Rename some of these fields for clarity
	[SerializeField] private float tractorBeamDistance = 2f;
	[SerializeField] private float projectileDistanceFromPlayer = 2f; // TODO : find the distance from the player on track.
	[SerializeField] private float shootForce = 0;
	[SerializeField] private Vector3 shootTorque = Vector3.zero;

	[Header("ContinuesDynamic References Settings")]
	[SerializeField] [Range(1, 100)] private float maxVelocity = 30f;
	[SerializeField] [Range(0, 128)] private int maxCapacity = 24;
	//[SerializeField] [Range(0, 300f)] private float maxTime = 60f;

	private Ray forwardRay { get { return new Ray(transform.position, transform.forward); } }
	private float sqrMaxVelocity;
	private List<Rigidbody> continuesReferences = new List<Rigidbody>();
	private Projectile projectile = null;

	private bool wait = false; // Used for testing waiting between shooting and tracking
	private float waitDelay = 0.5f;

	private class Projectile
	{
		public readonly Rigidbody rigidbody;
		public readonly Quaternion rotation;
		public readonly float drag;
		public readonly float angularDrag;

		private readonly Transform gunTransform;

		public Vector3 direction { get { return (rigidbody.position - gunTransform.position); } }

		public Projectile(Transform gunTransform, Rigidbody rigidbody)
		{
			this.rigidbody = rigidbody;
			this.rotation = Quaternion.Inverse(gunTransform.rotation) * rigidbody.transform.rotation;
			this.drag = rigidbody.drag;
			this.angularDrag = rigidbody.angularDrag;

			this.gunTransform = gunTransform;
		}
	}

	private void Awake()
	{
		Cursor.lockState = CursorLockMode.Locked;
		sqrMaxVelocity = maxVelocity * maxVelocity;
	}

	private void Update()
	{
		CheckContinuesReferences();
		if (wait) return;

		if ((projectile == null) && (Input.GetButton("Fire1")))
		{
			Track();
		}
		else if ((Input.GetButtonDown("Fire1")))
		{
			Shoot();
		}
		else if ((Input.GetButtonDown("Fire2"))) 
		{
			Drop();
		}
	}

	// TODO : Find out which update MoveProjectiles should be in and what collision mode it should have.
	private void FixedUpdate()
	{
		MoveProjectile();
	}

	// TODO : Track projectiles across surfaces. Currently, if the player has a projectile, looks at a surface and turns, the moveposition is ignored because of the raycast.
	private void MoveProjectile()
	{
		if (projectile == null) return;

		if (!Physics.Raycast(forwardRay, projectileDistanceFromPlayer, LayerMask.GetMask("Surface"), QueryTriggerInteraction.Ignore))
		{
			Vector3 projPos = transform.position + transform.forward * projectileDistanceFromPlayer;
			projectile.rigidbody.MovePosition(projPos);
		}

		Quaternion projRot = Quaternion.LookRotation(projectile.direction) * projectile.rotation;
		projectile.rigidbody.MoveRotation(projRot);
	}

	private void Track()
	{
		if (projectile != null) return;

		if (Physics.Raycast(forwardRay, out RaycastHit hitInfo, tractorBeamDistance, LayerMask.GetMask("Object"), QueryTriggerInteraction.Ignore))
		{
			Rigidbody rigidbody = hitInfo.transform.GetComponent<Rigidbody>();

			projectile = new Projectile(transform, rigidbody);

			projectile.rigidbody.drag = Mathf.Infinity;
			projectile.rigidbody.angularDrag = Mathf.Infinity;

			StartCoroutine(Wait());
		}
	}

	private void Shoot()
	{
		if (projectile == null) return;

		projectile.rigidbody.drag = projectile.drag;
		projectile.rigidbody.angularDrag = projectile.angularDrag;
		projectile.rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

		projectile.rigidbody.AddForce(shootForce * transform.forward, ForceMode.Impulse);
		projectile.rigidbody.AddTorque(shootTorque, ForceMode.Impulse);

		continuesReferences.Add(projectile.rigidbody);
		projectile = null;

		StartCoroutine(Wait());
	}

	// TODO : Make Objects throwable by dropping them when moving the pulseGun.
	private void Drop()
	{
		if (projectile == null) return;

		projectile.rigidbody.drag = projectile.drag;
		projectile.rigidbody.angularDrag = projectile.angularDrag;

		projectile = null;

		StartCoroutine(Wait());
	}

	// TODO : Add check on how much time has passed since the element was added
	private void CheckContinuesReferences()
	{
		int overflow = continuesReferences.Count - maxCapacity;
		for (int i = 0; i < overflow; i++)
		{
			Rigidbody reference = continuesReferences[0];
			reference.collisionDetectionMode = CollisionDetectionMode.Discrete;
			continuesReferences.RemoveAt(0);
		}

		for (int i = continuesReferences.Count - 1; i >= 0; i--)
		{
			Rigidbody reference = continuesReferences[i];
			if (reference.velocity.sqrMagnitude <= sqrMaxVelocity)
			{
				reference.collisionDetectionMode = CollisionDetectionMode.Discrete;
				continuesReferences.RemoveAt(i);
			}
		}
	}

	// TODO : Rename the "Wait" function to something more appropriate, or switch to a state-driven-machine with delays in between statechanges!
	IEnumerator Wait()
	{
		wait = true;
		yield return new WaitForSeconds(waitDelay);
		wait = false;
	}
}
