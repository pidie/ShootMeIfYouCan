using Enemies.Behaviors;
using UnityEngine;

namespace Managers
{
	public class WaypointManager : MonoBehaviour
	{
		public static Waypoint[] allWaypoints;

		private void Awake() => allWaypoints = GetAllWaypoints();

		private static Waypoint[] GetAllWaypoints() => FindObjectsOfType<Waypoint>();
	}
}
