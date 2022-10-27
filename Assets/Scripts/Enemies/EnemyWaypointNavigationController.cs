using UnityEngine;

namespace Enemies
{
	/// <summary>
	/// This class controls the automated movement of enemies through the waypoint matrix.
	/// </summary>
	public class EnemyWaypointNavigationController : MonoBehaviour
	{
		[SerializeField] private Vector3 destination;
		[SerializeField] private float stopDistance;
		[SerializeField] private float rotationSpeed;
		[SerializeField] private Vector3 velocity;
		[SerializeField] private Vector3 lastPosition;

		private float movementSpeed;

		public bool ReachedDestination { get; private set; }

		private void Awake()
		{
			destination = Vector3.zero;
			movementSpeed = GetComponent<EnemyBehavior>().GetMovementSpeed();
		}

		private void Update()
		{
			if (transform.position != destination)
			{
				var destinationDirection = destination - transform.position;

				var destinationDistance = destinationDirection.magnitude;

				if (destinationDistance >= stopDistance)
				{
					ReachedDestination = false;
					var targetRotation = Quaternion.LookRotation(destinationDirection);
					transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation,
						rotationSpeed * Time.deltaTime);
				}
				else ReachedDestination = true;

				transform.position += destinationDirection.normalized * (movementSpeed * Time.deltaTime);
			}
		}

		public void SetDestination(Vector3 destination)
		{
			this.destination = destination;
			ReachedDestination = false;
		}
	}
}