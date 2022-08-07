using UnityEngine;

namespace Enemies
{
	[CreateAssetMenu(menuName = "Enemy", fileName = "New Enemy Data")]
	public class EnemyData : ScriptableObject
	{
		[Header("Initialization")] 
		public GameObject projectile;
		public AudioClip projectileSound;
		public float scoreValue;
		[TextArea(3, 10)] public string notes;

		[Header("Primary Attack")] 
		public float primaryDamage;
		public float primaryWeaponRange;
		public float primaryWeaponCooldown;

		[Header("Secondary Attack")] 
		public float secondaryDamage;
		public float secondaryWeaponRange;
		public float secondaryWeaponCooldown;

		[Header("Defense")] 
		public float health;
		public float combatHealthRegeneration;
		public float outOfCombatHealthRegeneration;
		public float armor;
		public float armorRegeneration;
		public float shield;
		public float shieldRegeneration;

		[Header("Movement")] 
		public float movementSpeed;
	}
}