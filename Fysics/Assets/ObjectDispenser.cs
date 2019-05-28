using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDispenser : MonoBehaviour
{
	[SerializeField] private List<GameObject> objects = new List<GameObject>();
	[SerializeField] private Vector2 timeRange = Vector2.zero;
	[SerializeField] private Transform spawnPoint = null;
	[SerializeField] private float spawnVelocity = 10f;

	private float timeWait = 0f;

	private void Start()
	{
		timeWait = GetTime();
	}

	private void Update()
	{
		if (timeWait > 0)
		{
			timeWait -= Time.deltaTime;
		}
		else
		{
			int r = Random.Range(0, objects.Count);
			GameObject obj = Instantiate(objects[r], spawnPoint.position, spawnPoint.rotation);
			obj.GetComponent<Rigidbody>().AddForce(spawnVelocity * spawnPoint.up, ForceMode.Impulse);
			timeWait = GetTime();
		}
	}

	private float GetTime()
	{
		return Random.Range(timeRange.x, timeRange.y);
	}
}
