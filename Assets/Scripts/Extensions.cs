using Guns;
using UnityEngine;

public static class Extensions
{
	public static float Damage(Damage damageRange, float modifier) => Random.Range(damageRange.Min, damageRange.Max) * modifier;

	public static float Damage(float min, float max, float modifier) => Random.Range(min, max) * modifier;
}