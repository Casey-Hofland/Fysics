using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseGunController : MonoBehaviour
{
	// TODO : Rename some of these fields for clarity
	[SerializeField] private float tractorBeamDistance = 2f;
	[SerializeField] private float shootForce = 1000f;
	[SerializeField] private Vector3 shootTorque = Vector3.zero;
	[SerializeField] private Rigidbody controller = null;

	[SerializeField] private ProjectileSettings projectileSettings;

	private Ray forwardRay { get { return new Ray(transform.position, transform.forward); } }
	private Projectile projectile = null;

	private bool wait = false; // Used for testing waiting between shooting and tracking
	private float waitDelay = 0.5f;

	[Serializable]
	private class ProjectileSettings
	{
		[SerializeField] public float distFromPlayer = 2f; // TODO : find the distance from the player on track.
		[Range(0f, 255f)] public float transparency = 200f;
		[Range(0f, 1f)] public float blur = 0.3f;

		[HideInInspector] public Transform transform;
	}

	private class Projectile
	{
		public readonly Rigidbody rigidbody;
		private readonly Quaternion rotation;
		private readonly float distFromPlayer;
		private readonly float drag;
		private readonly float angularDrag;
		private readonly RigidbodyInterpolation interpolation;

		private readonly Transform gunTransform;
		private readonly Renderer renderer;
		private Color materialColor;

		private const string BASECOLORNAME = "_BaseColor";
		private const string DISTORTIONNAME = "_DistortionEnable";
		private const string DISTORTIONTESTNAME = "_DistortionDepthTest";
		private const string DISTORTIONBLURNAME = "_DistortionBlurRemapMin";

		public Vector3 direction { get { return (rigidbody.position - gunTransform.position); } }

		public Projectile(Rigidbody rigidbody, ProjectileSettings projectileSettings)
		{
			this.rigidbody = rigidbody;
			gunTransform = projectileSettings.transform;
			distFromPlayer = projectileSettings.distFromPlayer;

			rotation = Quaternion.Inverse(gunTransform.rotation) * rigidbody.transform.rotation;
			drag = rigidbody.drag;
			angularDrag = rigidbody.angularDrag;
			interpolation = rigidbody.interpolation;

			renderer = rigidbody.gameObject.GetComponent<Renderer>();
			materialColor = renderer.material.color;

			rigidbody.drag = Mathf.Infinity;
			rigidbody.angularDrag = Mathf.Infinity;
			//rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
			materialColor.a = projectileSettings.transparency / 255f;

			renderer.material.SetFloat(DISTORTIONNAME, 1f);
			renderer.material.SetFloat(DISTORTIONTESTNAME, 1f);
			renderer.material.SetFloat(DISTORTIONBLURNAME, projectileSettings.blur);
			renderer.material.SetColor(BASECOLORNAME, materialColor);
		}

		// TODO : Track projectiles across surfaces. Currently, if the player has a projectile, looks at a surface and turns, the moveposition is ignored because of the raycast.
		// TODO : Correct projectile collision detection. Currently, the object phases through walls if held close enough.
			// ?Solution: Raycast to desired position / rotation before moving
		public void Move()
		{
			//rigidbody.GetComponent<DynamicCollisionDetection>().OverrideFrame(CollisionDetectionMode.Continuous);

			Ray forwardDir = new Ray(gunTransform.position, gunTransform.forward);

			if (!Physics.Raycast(forwardDir, distFromPlayer, LayerMask.GetMask("Surface"), QueryTriggerInteraction.Ignore))
			{
				Vector3 newPosition = gunTransform.position + gunTransform.forward * distFromPlayer;
				rigidbody.MovePosition(newPosition);
			}

			Quaternion projRot = Quaternion.LookRotation(direction) * rotation;
			rigidbody.MoveRotation(projRot);
		}

		public void Reset()
		{
			rigidbody.drag = drag;
			rigidbody.angularDrag = angularDrag;
			rigidbody.interpolation = interpolation;
			materialColor.a = 1f;
			renderer.material.SetColor(BASECOLORNAME, materialColor);
			renderer.material.SetFloat(DISTORTIONBLURNAME, 0f);

			//rigidbody.GetComponent<DynamicCollisionDetection>().OverrideFrame(CollisionDetectionMode.ContinuousDynamic);
		}
	}

	private void Awake()
	{
		Cursor.lockState = CursorLockMode.Locked;
		projectileSettings.transform = transform;
	}

	private void Update()
	{
		if (wait) return;

		if (projectile == null)
		{
			if ((Input.GetButtonDown("Fire1")))
			{
				ShootSelf();
			}
			else if (Input.GetButton("Fire2"))
			{
				Track();
			}
		}
		else
		{
			if ((Input.GetButtonDown("Fire1")))
			{
				ShootProjectile();
			}
			else if (Input.GetButtonDown("Fire2"))
			{
				Drop();
			}
		}
	}

	// TODO : Find out which update MoveProjectiles should be in and what collision mode it should have.
	private void FixedUpdate()
	{
		if (projectile == null) return;
		projectile.Move();
	}

	private void Track()
	{
		if (projectile != null) return;

		if (Physics.Raycast(forwardRay, out RaycastHit hitInfo, tractorBeamDistance, LayerMask.GetMask("Object"), QueryTriggerInteraction.Ignore))
		{
			Rigidbody rigidbody = hitInfo.transform.GetComponent<Rigidbody>();

			projectile = new Projectile(rigidbody, projectileSettings);

			StartCoroutine(Wait());
		}
	}

	private void ShootSelf()
	{
		if (controller == null) return;

		controller.AddForce(shootForce * transform.forward, ForceMode.Impulse);

		StartCoroutine(Wait());
	}

	private void ShootProjectile()
	{
		if (projectile == null) return;

		projectile.rigidbody.AddForce(shootForce * transform.forward, ForceMode.Impulse);
		projectile.rigidbody.AddTorque(shootTorque, ForceMode.Impulse);
		projectile.Reset();

		projectile = null;

		StartCoroutine(Wait());
	}

	// TODO : Make Objects throwable by dropping them when moving the pulseGun.
	private void Drop()
	{
		if (projectile == null) return;

		projectile.Reset();
		projectile = null;

		StartCoroutine(Wait());
	}

	// TODO : Rename the "Wait" function to something more appropriate, or switch to a state-driven-machine with delays in between statechanges!
	IEnumerator Wait()
	{
		wait = true;
		yield return new WaitForSeconds(waitDelay);
		wait = false;
	}
}
