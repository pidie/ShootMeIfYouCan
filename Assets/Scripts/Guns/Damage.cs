using System;
using UnityEngine;

namespace Guns
{
	[Serializable]
	public class Damage
	{
		[SerializeField] private float min;
		[SerializeField] private float max;

		public float Min { get => min; private set => min = value; }
		public float Max { get => max; private set => max = value; }

		public Damage(float min, float max)
		{
			Min = min;
			Max = max;
		}
	}
}