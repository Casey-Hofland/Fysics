using UnityEngine;
using System.Collections;

public class PulseGunFirstPerson : PulseGun
{
	public override Ray TrackingRay => new Ray(transform.position, transform.forward);

	public override void Track()
	{
		if (Physics.Raycast(TrackingRay, out RaycastHit hitInfo, trackingDistance) && hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Object"))
		{
			projectile = hitInfo.transform.gameObject.AddComponent<Projectile>();
			projectile.SetUp(this, projectileSettings);

			//projectile = new Projectile(rigidbody, projectileSettings);

			//StartCoroutine(Wait());
		}
	}

	public override void Shoot()
	{
		projectile.Shoot(shootForce * transform.forward, shootTorque);
		projectile = null;
	}

	public override void ChargeForce()
	{
		//throw new System.NotImplementedException();
	}

	public override void ChargeTorque()
	{
		//throw new System.NotImplementedException();
	}

	public override void Drop()
	{
		Destroy(projectile);
		projectile = null;
	}
}
