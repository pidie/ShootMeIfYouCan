using Enemies;
using UnityEngine;

namespace Waypoints
{
	public class WaypointNavigator : MonoBehaviour
	{
		private EnemyWaypointNavigationController controller;
		[SerializeField] private Waypoint currentWaypoint;

		private int direction;

		private void Awake() => controller = GetComponent<EnemyWaypointNavigationController>();

		private void Start()
		{
			direction = Mathf.RoundToInt(Random.Range(0f, 1f));
			controller.SetDestination(currentWaypoint.GetPosition());
		}

		private void Update()
		{
			if (controller.ReachedDestination)
			{
				var shouldBranch = false;

				if (currentWaypoint.Branches.Count > 0)
					shouldBranch = Random.Range(0f, 1f) <= currentWaypoint.BranchRatio;

				if (shouldBranch)
				{
					var rnd = Mathf.RoundToInt(Random.Range(0f, currentWaypoint.Branches.Count - 1f));
					currentWaypoint = currentWaypoint.Branches[rnd];
				}
				else
				{
					switch (direction)
					{
						case 0 when currentWaypoint.NextWaypoint != null:
							currentWaypoint = currentWaypoint.NextWaypoint;
							break;
						case 0:
							currentWaypoint = currentWaypoint.PreviousWaypoint;
							direction = 1;
							break;
						case 1 when currentWaypoint.PreviousWaypoint != null:
							currentWaypoint = currentWaypoint.PreviousWaypoint;
							break;
						case 1:
							currentWaypoint = currentWaypoint.NextWaypoint;
							direction = 0;
							break;
					}
				}
				
				controller.SetDestination(currentWaypoint.GetPosition());
			}
		}

		public void SetCurrentWaypoint(Waypoint waypoint) => currentWaypoint = waypoint;
	}
}