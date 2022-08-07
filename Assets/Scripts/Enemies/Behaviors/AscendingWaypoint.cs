using UnityEngine;

namespace Enemies.Behaviors
{
	public class AscendingWaypoint : Waypoint
	{
		[SerializeField] private bool isLower;
		
		private Vector2 _position;
		private bool _isStartingAscent;

		public override Waypoint GetRandomNeighbor(Waypoint[] compareArray = null)
		{
			if (_isStartingAscent)
			{
				_isStartingAscent = false;
				
				var direction = isLower ? Vector3.down : Vector3.up;
				var ray = new Ray(transform.position, direction);
				Physics.Raycast(ray, out var hit, LayerMask.NameToLayer("Waypoint"));
				
				var ascending = hit.transform.GetComponent<AscendingWaypoint>();
				return ascending;
			}

			return base.GetRandomNeighbor(compareArray);
		}

		public override void SetIsStartingAscent(bool value) => _isStartingAscent = value;
	}
}