using UnityEngine;

[AddComponentMenu(null)]
public class Projectile : MonoBehaviour
{
	private void Reset()
	{
		Debug.LogError("Projectile Script may only be added at runtime!");
		DestroyImmediate(this);
	}

	private const string COLOR = "_BaseColor";
	private const string BLUR = "_DistortionBlurRemapMin";

	private Rigidbody rigidbody;
	private Renderer renderer;

	private float drag;
	private float angularDrag;
	private Color color;
	private float blur;

	private PulseGun gun;
	private ProjectileSettings settings;
	private Quaternion fixedRotation;

	private Vector3 lastPosition;

	private void Awake()
	{
		// We can assume that Projectile will always have a rigidbody.
		rigidbody = GetComponent<Rigidbody>();
		renderer = GetComponent<Renderer>();

		drag = rigidbody.drag;
		angularDrag = rigidbody.angularDrag;

		color = renderer.material.color;
		blur = renderer.material.GetFloat(BLUR);
	}

	public void SetUp(PulseGun gun, ProjectileSettings settings)
	{
		this.gun = gun;
		this.settings = settings;

		fixedRotation = Quaternion.Inverse(gun.transform.rotation) * transform.rotation;

		rigidbody.drag = Mathf.Infinity;
		rigidbody.angularDrag = Mathf.Infinity;
		//rigidbody.useGravity = false;

		Material newMaterial = renderer.material;
		Color newColor = color;
		newColor.a = settings.transparency / 255f;
		newMaterial.SetColor(COLOR, newColor);
		newMaterial.SetFloat(BLUR, settings.blur);

		renderer.material.Lerp(renderer.material, newMaterial, 1f);
	}

	private void FixedUpdate()
	{
		Ray trackingRay = gun.TrackingRay;

		lastPosition = rigidbody.position;

		// Move
		Vector3 newPosition;
		bool hit = Physics.Raycast(trackingRay, out RaycastHit hitInfo, settings.distance, LayerMask.GetMask("Surface"), QueryTriggerInteraction.Ignore);

		if (hit)
		{
			newPosition = hitInfo.point + hitInfo.normal * Physics.defaultContactOffset;
		}
		else
		{
			newPosition = trackingRay.GetPoint(settings.distance);
		}

		//Vector3 newPosition = hit ? hitInfo.point : trackingRay.GetPoint(settings.distance);
		rigidbody.MovePosition(newPosition);

		// Rotate
		Vector3 lookDirection = transform.position - gun.transform.position;
		Quaternion projRot = Quaternion.LookRotation(lookDirection) * fixedRotation;
		rigidbody.MoveRotation(projRot);

		Debug.Log(rigidbody.velocity);
	}

	public void Shoot(Vector3 force, Vector3 torque)
	{
		rigidbody.AddForce(force, ForceMode.Impulse);
		rigidbody.AddTorque(torque, ForceMode.Impulse);
		Destroy(this);
	}

	private void OnDestroy()
	{
		rigidbody.drag = drag;
		rigidbody.angularDrag = angularDrag;

		Debug.Log("Destroyed: " + rigidbody.velocity);

		//rigidbody.useGravity = true;

		Debug.Log(rigidbody.position - lastPosition);

		rigidbody.AddForce((rigidbody.position - lastPosition) * 5f, ForceMode.VelocityChange);

		Material newMaterial = renderer.material;
		newMaterial.SetColor(COLOR, color);
		newMaterial.SetFloat(BLUR, blur);
		renderer.material.Lerp(renderer.material, newMaterial, 1f);
	}
}
