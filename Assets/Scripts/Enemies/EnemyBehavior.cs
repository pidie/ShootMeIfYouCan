using System.Collections;
using System.Collections.Generic;
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
	public abstract class EnemyBehavior : MonoBehaviour
	{
		[Header("Initialization")] 
		[SerializeField] protected PlayerDetector detector;
		[SerializeField] protected int scoreValue;
		[SerializeField] protected Damageable damageable;
		[SerializeField] protected GameObject killCountEffect;
		[SerializeField] protected LayerMask waypointLayer;
		[SerializeField] protected float physicalWidth;
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
		[SerializeField] protected float bobbingHeight = 0.4f;
		[SerializeField] protected float bobbingSpeed = 1f;
		
		private bool _ascendedThisMove;
		protected bool canPrimaryAttack;
		protected bool canSecondaryAttack;
		protected bool canZoom;
		protected List<Vector3> knownPlayerCoordinates = new ();
		protected Vector3 lastKnownPlayerCoordinates = Vector3.zero;
		protected EnemyStateController stateController = new ();

		public GameObject GetKillCountEffect => killCountEffect;
		public int GetScoreValue => scoreValue;
		public Color Color { get; protected set; }

		protected virtual void Awake()
		{
			globalHealthBonus *= GameManager.EnemyHealthBonus;
			primaryDamageBonus *= GameManager.EnemyDamageBonus;
			
			Color = gameObject.GetComponent<Renderer>().material.color;
			damageable.Set(health * globalHealthBonus, passiveHealthRegen, passiveHealthBonus);
			stateController.State = EnemyState.Patrolling;

			canPrimaryAttack = true;
		}

		protected virtual void Update() { }
		
		protected virtual void PrimaryAttack() 
		{ 
			StartCoroutine(PrimaryCooldown());
			AudioManager.instance.PlayOneShot(primaryWeaponFireSound);
			stateController.State = EnemyState.Hunting;
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

		public float GetMovementSpeed() => movementSpeed;
	}
}