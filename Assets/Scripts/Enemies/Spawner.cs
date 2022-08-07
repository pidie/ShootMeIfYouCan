using Enemies.Behaviors;
using UnityEngine;

namespace Enemies
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private Transform spawnPosition;

        private Waypoint spawnWaypoint;

        private void Awake() => spawnWaypoint = GetComponentInChildren<Waypoint>();

        public void SpawnEnemy(Enemy enemy)
        {
            var parent = transform;
            
            if (enemy.GetType() == typeof(SphereBehavior))
                parent = GameObject.Find("--- Enemies/Spheres").transform;

            enemy.SetTargetWaypoint(spawnWaypoint);
            var e = Instantiate(enemy.gameObject, spawnPosition.position, Quaternion.identity, parent);
        }
    }
}