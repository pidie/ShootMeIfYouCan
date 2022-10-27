using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Enemies
{
    [RequireComponent(typeof(SphereCollider))]
    public class CollisionAvoidanceSystem : MonoBehaviour
    {
        [Range(0.5f, 20f)]
        [SerializeField] private float bufferSize;
        [SerializeField] private float objectRadius;
        
        private bool isAvoidingCollision;
        private List<CollisionAvoidanceSystem> activeCollisions = new ();
        private SphereCollider bufferCollider;

        public float ObjectRadius
        {
            get => objectRadius;
            set => objectRadius = value;
        }

        public Vector3 Direction { get; private set; }

        private void Awake()
        {
            bufferCollider = GetComponent<SphereCollider>();
            bufferCollider.isTrigger = true;
            bufferCollider.radius = bufferSize;
        }

        private void Update()
        {
            if (isAvoidingCollision)
            {
                /*
                 * the logic for determining collision avoidance will go here
                 */
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var incomingCollision = other.GetComponent<CollisionAvoidanceSystem>();
            
            if (incomingCollision && incomingCollision != this)
            {
                if (!activeCollisions.Contains(incomingCollision))
                {
                    activeCollisions.Add(incomingCollision);
                    isAvoidingCollision = true;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var avoidedCollision = other.GetComponent<CollisionAvoidanceSystem>();

            if (avoidedCollision && avoidedCollision != this)
            {
                var _ = activeCollisions.Where(collision => collision == avoidedCollision).ToList();

                foreach (var collision in _)
                    activeCollisions.Remove(collision);
                
                if (activeCollisions.Count == 0)
                    isAvoidingCollision = false;
                
            }
        }
    }
}
