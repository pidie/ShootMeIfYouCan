using Enemies.Behaviors;
using UnityEngine;
using Waypoints;

namespace Enemies
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private Transform spawnPosition;

        private Waypoint spawnWaypoint;

        private void Awake() => spawnWaypoint = GetComponentInChildren<Waypoint>();

        public void SpawnEnemy(EnemyBehavior enemyBehavior)
        {
            var parent = transform;
            
            if (enemyBehavior.GetType() == typeof(SphereBehavior))
                parent = GameObject.Find("--- Enemies/Spheres").transform;

            var e = Instantiate(enemyBehavior.gameObject, spawnPosition.position, Quaternion.identity, parent);

            var waypointNav = e.GetComponent<WaypointNavigator>();
            waypointNav.SetCurrentWaypoint(spawnWaypoint);
        }
    }
}