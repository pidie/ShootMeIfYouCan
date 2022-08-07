using Enemies;
using UnityEngine;

namespace Guns
{
	public class Projectile : MonoBehaviour
	{
		private float _damage;
		private Vector3 _point;
		private Vector3 _normal;
		private float _range;
		private Color _color;

		private Vector3 _initialPosition;

		public bool FiredByPlayer { get; private set; }			// true if the shot was fired by the player
		public GameObject Shooter { get; private set; }		// used to prevent enemies from shooting themselves

		public void SetValues(float damage, Vector3 point, Vector3 normal, float range, Color color)
		{
			_damage = damage;
			_point = point;
			_normal = normal;
			_range = range;
			_color = color;
		}

		private void Awake() => _initialPosition = transform.position;

		private void Update()
		{
			// if the shot hasn't hit anything and has traveled its maximum distance
			if (Vector3.Distance(_initialPosition, transform.position) > _range)
			{
				if (FiredByPlayer)
					AccuracyManager.onUpdateAccuracy(false);
			
				Destroy(gameObject);
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			var damageable = other.GetComponent<Damageable>();

			// prevent enemies from shooting themselves accidentally
			if (damageable == null || damageable.gameObject == Shooter)
				return;

			// prevent friendly fire
			var enemy = damageable.GetComponent<Enemy>();
			if (enemy)
			{
				if (!FiredByPlayer || _color != enemy.Color)
					Destroy(gameObject);
				else
				{
					var detector = enemy.GetComponentInChildren<EnemyDetection>();
					if (detector)
						detector.PlayerDetected(_initialPosition);

					if (FiredByPlayer)
						AccuracyManager.onUpdateAccuracy(true);
				}
			}
		
			damageable.TakeDamage(_damage, _point, _normal);
			Destroy(gameObject);
		}

		public void SetIsPlayer() => FiredByPlayer = true;

		public void SetShooter(GameObject shooter) => Shooter = shooter;
	}
}