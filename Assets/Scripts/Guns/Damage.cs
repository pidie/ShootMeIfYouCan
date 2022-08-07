using System;
using UnityEngine;

[Serializable]
public class Damage
{
	[SerializeField] private float min;
	[SerializeField] private float max;

	public float Min => min;
	public float Max => max;
}