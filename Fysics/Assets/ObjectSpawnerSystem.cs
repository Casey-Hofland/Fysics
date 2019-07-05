using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ObjectSpawnerSystem : ComponentSystem
{
	struct Components
	{
		public GameObject gameObject;
		public Vector3 position;
		public Vector3 rotation;
	}

	protected override void OnUpdate()
	{
		//Entities.ForEach<Components>(EntityQueryBuilder.F_B<Components>);

		/*
		foreach (var e in GetEntities<Components>())
		{

		}
		*/
	}
}
