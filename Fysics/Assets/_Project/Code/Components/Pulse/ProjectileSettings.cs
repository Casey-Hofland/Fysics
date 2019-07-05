using System;
using UnityEngine;

[Serializable]
public class ProjectileSettings
{
	public float distance = 2f; // TODO : find the distance from the player on tracking.
	[Range(0f, 255f)] public float transparency = 128f;
	[Range(0f, 1f)] public float blur = 0.2f;
}
