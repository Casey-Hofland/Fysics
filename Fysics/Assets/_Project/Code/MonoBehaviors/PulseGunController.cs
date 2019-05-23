using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseGunController : MonoBehaviour
{
	[SerializeField] private float tractorBeamDistance = 2f;
	[SerializeField] private Vector3 shootForce = Vector3.zero;

	[Header("ContinuesDynamic References Settings")]
	[SerializeField] [Range(1, 100)] private float maxVelocity = 30f;
	[SerializeField] [Range(0, 128)] private int maxCapacity = 24;
	//[SerializeField] [Range(0, 300f)] private float maxTime = 60f;

	private float sqrMaxVelocity;

	// TODO : store references of each shot projectile, and reset their collisionDetectionMode to Discrete when their velocity reaches below a certain speed.
	private List<Rigidbody> continuesReferences = new List<Rigidbody>();

	[Header("! For Testing Only !")]
	[SerializeField] private Vector3 shootTorque = Vector3.zero;

	private Rigidbody projectile;
	private Transform projectileParent;

	private bool wait = false; // Used for testing waiting between shooting and tracking
	private float waitDelay = 0.5f;

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
	}

	private void Track()
	{
		Debug.Log("Tracking");

		Ray ray = new Ray(transform.position, transform.forward);
		if (Physics.Raycast(ray, out RaycastHit hitInfo, tractorBeamDistance, LayerMask.GetMask("Object"), QueryTriggerInteraction.Ignore))
		{
			projectile = hitInfo.transform.GetComponent<Rigidbody>();

			if (projectile)
			{
				projectileParent = projectile.transform.parent;

				projectile.transform.SetParent(transform, true);
				projectile.isKinematic = true;

				projectile.transform.localPosition = new Vector3(0, 0, 1.5f);
				projectile.transform.localRotation = Quaternion.identity;

				StartCoroutine(Wait());
			}
		}
	}

	private void Shoot()
	{
		Debug.Log("Shot!!!");

		projectile.transform.SetParent(projectileParent, true);
		projectile.isKinematic = false;
		projectile.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		projectile.AddRelativeForce(shootForce, ForceMode.Impulse);

#if UNITY_EDITOR
		projectile.AddRelativeTorque(shootTorque, ForceMode.Impulse);
#endif

		continuesReferences.Add(projectile);
		projectile = null;
		projectileParent = null;

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

	IEnumerator Wait()
	{
		wait = true;
		yield return new WaitForSeconds(waitDelay);
		wait = false;
	}
}
