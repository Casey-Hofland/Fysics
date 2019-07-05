using UnityEngine;

public abstract class PulseGun : MonoBehaviour
{
	[SerializeField] protected float trackingDistance = 2f;
	[SerializeField] protected float shootForce = 1000f;
	[SerializeField] [Range(1f, 10f)] protected float maxForceCharge = 6f;
	[SerializeField] [Range(1f, 100f)] protected float forceChargeSpeed = 4f;
	[SerializeField] [Range(0f, 1f)] protected float forceChargeGrounding = 1f;
	[SerializeField] protected Vector3 shootTorque = Vector3.one * 36f;
	[SerializeField] [Range(0f, 10f)] protected float maxTorqueCharge = 3f;
	[SerializeField] [Range(1f, 100f)] protected float torqueChargeSpeed = 2f;
	[SerializeField] [Range(0f, 1f)] protected float torqueChargeGrounding = 0f;
	[SerializeField] protected ProjectileSettings projectileSettings;

	protected Projectile projectile = null;
	private float forceCharge = 1f;
	private float forceChargeChange { get { return forceChargeSpeed * Time.deltaTime; } }
	private float torqueCharge = 0f;

	public bool Loaded { get { return projectile != null; } }

	public abstract Ray TrackingRay { get; }

	public virtual void Track()
	{
		if (Physics.Raycast(TrackingRay, out RaycastHit hitInfo, trackingDistance) && hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Object"))
		{
			projectile = hitInfo.transform.gameObject.AddComponent<Projectile>();
			projectile.SetUp(this, projectileSettings);
		}
	}

	public virtual void Shoot()
	{
		float forceCharge = this.forceCharge - this.forceCharge % forceChargeGrounding;

		projectile.Shoot(shootForce * forceCharge * transform.forward, shootTorque * torqueCharge);
		projectile = null;
	}

	public virtual void ChargeForce()
	{
		forceCharge += forceChargeChange * 2;
	}

	private void Update()
	{
		forceCharge -= Mathf.Max(forceCharge - forceChargeChange, 1f);
	}

	public abstract void ChargeTorque();

	public virtual void Drop()
	{
		Destroy(projectile);
		projectile = null;
	}
}
