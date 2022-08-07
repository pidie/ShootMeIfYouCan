using System.Collections.Generic;
using System.Linq;
using Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemies.Behaviors
{
    public class Waypoint : MonoBehaviour
    {
        [SerializeField] private bool isStartingNode;
        [SerializeField] protected int level;
        [SerializeField] protected List<Waypoint> neighbors = new ();
        [SerializeField] protected List<Waypoint> blacklist = new ();

        private Waypoint _origin;
        
        public bool IsStartingNode => isStartingNode;
        public bool HasIncoming { get; private set; }
        public int Level => level;

        private void OnTriggerEnter(Collider other)
        {
            var waypoint = other.GetComponent<Waypoint>();
            
            if (waypoint && !waypoint.IsStartingNode && !neighbors.Contains(waypoint) && waypoint.Level == level)
                neighbors.Add(waypoint);
        }

        public virtual Waypoint GetRandomNeighbor(Waypoint[] compareArray = null)
        {
            var validNeighbors = new List<Waypoint>();
            validNeighbors.AddRange(neighbors);
            
            foreach (var neighbor in neighbors)
            {
                if (neighbor.HasIncoming)
                {
                    validNeighbors.Remove(neighbor);
                    blacklist.Add(neighbor);
                }
                
                if (compareArray != null)
                    if (compareArray.Contains(neighbor))
                    {
                        validNeighbors.Remove(neighbor);
                        blacklist.Add(neighbor);
                    }
            }

            if (validNeighbors.Count < 1)
            {
                var allWaypoints = new List<Waypoint>();
                allWaypoints.AddRange(WaypointManager.allWaypoints);

                foreach (var waypoint in allWaypoints.Where(waypoint => waypoint.GetType() == typeof(AscendingWaypoint)))
                    blacklist.Add(waypoint);

                foreach (var waypoint in blacklist)
                    allWaypoints.Remove(waypoint);

                validNeighbors = allWaypoints;
            }
            
            var newTargetWaypoint = validNeighbors[Random.Range(0, validNeighbors.Count)];
            blacklist.Clear();
            return newTargetWaypoint;
        }

        public Waypoint[] GetAllNeighbors() => neighbors.ToArray();

        public bool RequestIncoming(Waypoint origin)
        {
            if (HasIncoming)
                return false;

            HasIncoming = true;
            _origin = origin;
            return true;
        }

        public void Depart() => HasIncoming = false;
        
        public virtual void SetIsStartingAscent(bool value) {}
    }
}