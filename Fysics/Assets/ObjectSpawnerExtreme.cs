using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
//using Unity.Rendering;

public class ObjectSpawnerExtreme : MonoBehaviour
{
	[SerializeField] private GameObject objectPrefab;
	[SerializeField] private GameObject objectPrefabECS;
	[SerializeField] private int nrOfObjects = 100;
	[SerializeField] private bool cnst = false;
	[SerializeField] private bool ecs = false;

	EntityManager entityManager;

	private void Start()
	{
		entityManager = World.Active.EntityManager;
	}

	private void Update()
    {
		if (cnst)
		{
			if (Input.GetKey(KeyCode.Y))
			{
				if (ecs)
				{
					SpawnObjectsECS();
				}
				else
				{
					SpawnObjects();
				}
			}
		}
		else
		{
			if (Input.GetKeyDown(KeyCode.Y))
			{
				if (ecs)
				{
					SpawnObjectsECS();
				}
				else
				{
					SpawnObjects();
				}
			}
		}
    }

	public void SpawnObjects()
	{
		int n = Mathf.CeilToInt(Mathf.Sqrt(nrOfObjects));

		Vector3 oriSpawnPoint = transform.position;
		oriSpawnPoint += -transform.up + -transform.right * (n - 1) + -transform.forward * (n - 1);

		for (int i = 0; i < nrOfObjects; i++)
		{
			int r = i % n;
			int f = Mathf.FloorToInt(i / n);

			Vector3 spawnPoint = oriSpawnPoint;
			spawnPoint += transform.right * 2 * r;
			spawnPoint += transform.forward * 2 * f;

			Destroy(Instantiate(objectPrefab, spawnPoint, transform.rotation), 1f);
		}
	}

	public void SpawnObjectsECS()
	{
		int n = Mathf.CeilToInt(Mathf.Sqrt(nrOfObjects));

		Vector3 oriSpawnPoint = transform.position;
		oriSpawnPoint += -transform.up + -transform.right * (n - 1) + -transform.forward * (n - 1);

		//Entity ecsObj = entityManager.CreateEntity(typeof(Translation));
		EntityArchetype entityArchetype = entityManager.CreateArchetype(
			typeof(Translation),
			typeof(LocalToWorld)//,
			//typeof(RenderMesh)
			);
		NativeArray<Entity> ecsObjs = new NativeArray<Entity>(nrOfObjects, Allocator.Temp);
		entityManager.CreateEntity(entityArchetype, ecsObjs);

		for (int i = 0; i < nrOfObjects; i++)
		{
			int r = i % n;
			int f = Mathf.FloorToInt(i / n);

			Vector3 spawnPoint = oriSpawnPoint;
			spawnPoint += transform.right * 2 * r;
			spawnPoint += transform.forward * 2 * f;

			entityManager.SetComponentData(ecsObjs[i], new Translation { Value = spawnPoint });
			entityManager.Instantiate(ecsObjs[i]);
		}

		ecsObjs.Dispose();
	}

	private void SpawnObject(Vector3 position, Quaternion rotation)
	{
		if (ecs)
		{
			Entity ecsObj;

			//ecsObj = entityManager.Instantiate(objectPrefabECS);
			ecsObj = entityManager.CreateEntity(typeof(Translation));

			NativeArray<Entity> ecsObjs = new NativeArray<Entity>();

			entityManager.Instantiate(ecsObj);

			/*
			ecsObj = entityManager.CreateEntity();
			entityManager.SetComponentData(ecsObj, new Position { Value = position });
			entityManager.SetComponentData(ecsObj, new Rotation { Value = rotation });
			var instance = entityManager.Instantiate(ecsObj);
			*/
		}
		else
		{
			
		}
	}
}

public struct Position : IComponentData
{
	public Vector3 Value;
}

public struct Rotation : IComponentData
{
	public Quaternion Value;
}