using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Waypoints
{
	public class Waypoint : MonoBehaviour
	{
		[SerializeField] private Waypoint previousWaypoint;
		[SerializeField] private Waypoint nextWaypoint;

		[Range (0f, 5f)]
		[SerializeField] private float width = 1f;

		public float Width => width;

		[Range(0f, 1f)] 
		[SerializeField] private float branchRatio = 0.5f;

		public float BranchRatio => branchRatio;
		
		public Waypoint PreviousWaypoint
		{
			get => previousWaypoint;
			set => previousWaypoint = value;
		}

		public Waypoint NextWaypoint
		{
			get => nextWaypoint;
			set => nextWaypoint = value;
		}

		[SerializeField] private List<Waypoint> branches = new ();

		public List<Waypoint> Branches => branches;

		private void Awake()
		{
			branches ??= new List<Waypoint>();
		}

		public Vector3 GetPosition()
		{
			var minBound = transform.position + transform.right * width / 2f;
			var maxBound = transform.position - transform.right * width / 2f;

			return Vector3.Lerp(minBound, maxBound, Random.Range(0f, 1f));
		}
	}
}