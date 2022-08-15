using System.Collections;
using System.Collections.Generic;
using Enemies.Behaviors;
using Guns;
using Managers;
using UnityEngine;

namespace Enemies
{
	public enum EnemyState
	{
		Idle,
		Patrolling,
		Searching,
		Hunting,
		Zooming,
		Attack,
		BossPhaseOne,
		BossPhaseTwo,
	}
	public abstract class Enemy : MonoBehaviour
	{
		[Header("Initialization")] 
		[SerializeField] protected EnemyDetection detector;
		[SerializeField] protected int scoreValue;
		[SerializeField] protected Damageable damageable;
		[SerializeField] protected GameObject killCountEffect;
		[Header("Global Modifications")] 
		[SerializeField] protected float globalDamageBonus = 1f;
		[SerializeField] protected float globalHealthBonus = 1f;
		[Header("Offense - Primary")] 
		[SerializeField] protected Damage primaryDamage = new (3f, 3f);
		[SerializeField] protected float primaryDamageBonus = 1f;
		[SerializeField] protected float range = 10f;
		[SerializeField] protected float primaryCooldown = 1f;
		[SerializeField] protected GameObject primaryProjectile;
		[SerializeField] protected AudioClip primaryWeaponFireSound;
		[Header("Offense - Secondary")] 
		[SerializeField] protected Damage secondaryDamage = new (10f, 12f);
		[SerializeField] protected float secondaryDamageBonus = 1f;
		[SerializeField] protected float secondaryRange = 5f;
		[SerializeField] protected float secondaryCooldown = 2f;
		[SerializeField] protected GameObject secondaryProjectile;
		[SerializeField] protected AudioClip secondaryWeaponFireSound;
		[Header("Defense")] 
		[SerializeField] protected float health = 10f;
		[SerializeField] protected float armor;
		[SerializeField] protected float shield;
		[SerializeField] protected float passiveHealthRegen;
		[SerializeField] protected float passiveHealthBonus;
		[Header("Movement")] 
		[SerializeField] protected float movementSpeed = 1f;
		[SerializeField] protected Waypoint targetWaypoint;
		[SerializeField] protected Waypoint[] waypointsInRange;
		[Tooltip("The last 20 waypoints visited")]
		[SerializeField] protected Waypoint[] waypointsVisited = new Waypoint[20];

		private bool _ascendedThisMove;
		protected bool canPrimaryAttack;
		protected bool canSecondaryAttack;
		[SerializeField]
		protected EnemyState state = EnemyState.Idle;
		protected List<Vector3> knownPlayerCoordinates = new ();
		protected Vector3 lastKnownPlayerCoordinates = Vector3.zero;

		public Color Color { get; protected set; }
		
		protected virtual void Awake()
		{
			for (var i = 0; i < waypointsVisited.Length; i++)
				waypointsVisited[i] = null;

			globalHealthBonus *= GameManager.EnemyHealthBonus;
			primaryDamageBonus *= GameManager.EnemyDamageBonus;
			
			Color = gameObject.GetComponent<Renderer>().material.color;
			damageable.Set(health * globalHealthBonus, passiveHealthRegen, passiveHealthBonus);
			transform.LookAt(targetWaypoint.transform);

			canPrimaryAttack = true;
		}

		protected virtual void Update() { }

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

		protected virtual void PrimaryAttack() 
		{ 
			StartCoroutine(PrimaryCooldown());
			AudioManager.instance.PlayOneShot(primaryWeaponFireSound);
			state = EnemyState.Hunting;
		}

		protected virtual void SecondaryAttack() { }
		
		protected IEnumerator PrimaryCooldown()
		{
			yield return new WaitForSeconds(primaryCooldown);
			canPrimaryAttack = true;
		}
		
		protected IEnumerator SecondaryCooldown()
		{
			yield return new WaitForSeconds(secondaryCooldown);
			canSecondaryAttack = true;
		}
	}
}