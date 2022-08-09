using Enemies.Behaviors;
using Guns;
using Managers;
using UnityEngine;

namespace Enemies
{
	public class Enemy : MonoBehaviour
	{
		[Header("Initialization")] 
		[SerializeField] protected GameObject projectilePrefab;
		[SerializeField] protected AudioClip weaponFireSound;
		[SerializeField] protected EnemyDetection detector;
		[SerializeField] protected int scoreValue;
		[SerializeField] protected Damageable damageable;
		[SerializeField] protected GameObject killCountEffect;
		[Header("Offense")] 
		[SerializeField] protected float damage = 1;
		[SerializeField] protected float range = 10;
		[SerializeField] protected float weaponCooldown = 1;
		[Header("Defense")] 
		[SerializeField] protected float health = 10;
		[SerializeField] protected float armor;
		[SerializeField] protected float shield;
		[SerializeField] protected float passiveHealthRegen;
		[SerializeField] protected float passiveHealthBonus;
		[Header("Movement")] 
		[SerializeField] protected float movementSpeed = 1;
		[SerializeField] protected Waypoint targetWaypoint;
		[SerializeField] protected Waypoint[] waypointsInRange;
		[Tooltip("The last 20 waypoints visited")]
		[SerializeField] protected Waypoint[] waypointsVisited = new Waypoint[20];

		private bool _ascendedThisMove;

		public Color Color { get; protected set; }

		protected virtual void Awake()
		{
			for (var i = 0; i < waypointsVisited.Length; i++)
				waypointsVisited[i] = null;

			health *= GameManager.EnemyHealthBonus;
			damage *= GameManager.EnemyDamageBonus;
			
			Color = gameObject.GetComponent<Renderer>().material.color;
			damageable.Set(health, passiveHealthRegen, passiveHealthBonus);
			transform.LookAt(targetWaypoint.transform);
		}

		protected virtual void Update()
		{
			
			if (Vector3.Distance(transform.position, targetWaypoint.transform.position) < 0.2f)
			{
				AddVisitedWaypoint(targetWaypoint);
				targetWaypoint.Depart();
				var newWaypoint = SetTargetWaypoint(targetWaypoint.GetRandomNeighbor());
				newWaypoint.RequestIncoming(waypointsVisited[0]);
				transform.LookAt(targetWaypoint.transform);
			}
			transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.transform.position, movementSpeed * Time.deltaTime);
		}

		public int GetScoreValue() => scoreValue;

		public GameObject GetKillCountEffect() => killCountEffect;

		public Waypoint SetTargetWaypoint(Waypoint waypoint)
		{
			if (waypoint == null) return targetWaypoint;
			
			if (waypoint.GetType() == typeof(AscendingWaypoint) && !_ascendedThisMove)
			{
				_ascendedThisMove = true;
				waypoint.SetIsStartingAscent(true);
			}
			else
			{
				_ascendedThisMove = false;
				waypoint.SetIsStartingAscent(false);
			}
			
			return targetWaypoint = waypoint;
		}

		public void AddVisitedWaypoint(Waypoint waypoint)
		{
			for (var i = waypointsVisited.Length - 1; i >= 0; i--)
			{
				if (i == 0)
					waypointsVisited[i] = waypoint;
				else if (waypointsVisited[i - 1] == null) { }
				else
					waypointsVisited[i] = waypointsVisited[i - 1];

			}
		}
	}
}