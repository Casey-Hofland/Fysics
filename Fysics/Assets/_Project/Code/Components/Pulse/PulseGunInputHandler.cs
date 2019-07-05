using UnityEngine;
using System.Collections;

public class PulseGunInputHandler : MonoBehaviour
{
	private bool waitForReset = false;
	private bool prevLoaded = false;

	private PulseGun pulseGun;
	private PulsePack pulsePack;

	private void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
		pulseGun = GetComponent<PulseGun>();
		pulsePack = GetComponent<PulsePack>();
	}

	private void Update()
	{
		if (waitForReset)
		{
			waitForReset = (Input.GetButton("Fire1") || Input.GetButton("Fire2"));
			return;
		}

		if (pulseGun != null && pulseGun.Loaded)
		{
			if (Input.GetButtonUp("Fire1"))
			{
				pulseGun.Shoot();
			}
			else if (Input.GetButton("Fire1"))
			{
				pulseGun.ChargeTorque();
			}

			if (Input.GetButtonUp("Fire2"))
			{
				pulseGun.Drop();
			}
			if (Input.GetButton("Fire2"))
			{
				pulseGun.ChargeForce();
			}
		}
		else
		{
			if (pulsePack != null)
			{
				if (Input.GetButtonUp("Fire1"))
				{
					pulsePack.Shoot();
				}
				else if (Input.GetButton("Fire1"))
				{
					pulsePack.Charge();
				}
			}
			
			if (pulseGun != null)
			{
				if (Input.GetButton("Fire2"))
				{
					pulseGun.Track();
				}
			}
		}

		waitForReset = prevLoaded != pulseGun.Loaded;
		prevLoaded = pulseGun.Loaded;
	}
}
