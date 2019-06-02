using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

[Obsolete]
public class NetworkSetup : NetworkBehaviour
{
	[SerializeField] private Behaviour[] networkSingletons;

	private void Start()
	{
		if (!isLocalPlayer)
		{
			for (int i = 0; i < networkSingletons.Length; i++)
			{
				networkSingletons[i].enabled = false;
			}
		}
	}
}
