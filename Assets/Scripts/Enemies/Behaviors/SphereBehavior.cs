using System;
using Guns;
using UnityEngine;

namespace Enemies.Behaviors
{
    public class SphereBehavior : EnemyBehavior
    {
        [Header("Sphere Zooming")]
        [SerializeField] private int zoomWaypointWaitMinimum = 6;
        [Tooltip("Percentage chance of a zoom expressed as a decimal")]
        [SerializeField] private float zoomChance = 0.15f;
        
        private int _waypointsSinceLastZoom;

        protected override void Awake()
        {
            base.Awake();
            if (physicalWidth == 0)
                physicalWidth = 1f;
        }
        
        protected override void Update()
        {
            base.Update();

            var pos = transform.position;

            if (stateController.State == EnemyState.Attack) { }
            else if (detector.IsPlayerDetected)
                stateController.State = EnemyState.Hunting;
            else if (detector.PlayerDetectedFromPoint != Vector3.zero)
                stateController.State = EnemyState.Searching;
            else if (stateController.State == EnemyState.Patrolling && _waypointsSinceLastZoom >= zoomWaypointWaitMinimum)
            {
                canZoom = true;
            }
            else
            {
                canZoom = false;
            }

            switch (stateController.State)
            {
                case EnemyState.Idle:
                {
                    // bobbing
                    transform.position = new Vector3(pos.x, pos.y * Mathf.Sin(Time.time * bobbingSpeed) * bobbingHeight, pos.z);
                    break;
                }
                case EnemyState.Patrolling:
                {
                    break;
                }
                case EnemyState.Searching:
                {
                    // move to lastKnownPlayerCoordinate
                    transform.LookAt(detector.PlayerDetectedFromPoint);
                    transform.position += transform.forward * (movementSpeed * Time.deltaTime);

                    var tx = (float)Math.Round(pos.x, 2);
                    var ty = (float)Math.Round(pos.y, 2);
                    var tz = (float)Math.Round(pos.z, 2);
                    var dx = (float)Math.Round(detector.PlayerDetectedFromPoint.x, 2);
                    var dy = (float)Math.Round(detector.PlayerDetectedFromPoint.y, 2);
                    var dz = (float)Math.Round(detector.PlayerDetectedFromPoint.z, 2);

                    if (tx == dx && ty == dy && tz == dz)
                    {
                        detector.PlayerLost();
                        stateController.State = EnemyState.Patrolling;
                    }
                    
                    break;
                }
                case EnemyState.Hunting:
                {
                    // track player
                    transform.LookAt(detector.Target);
                    transform.position += transform.forward * (movementSpeed * Time.deltaTime);

                    if (canPrimaryAttack && detector.Distance(transform) <= range)
                        stateController.State = EnemyState.Attack;
                    break;
                }
                case EnemyState.Zooming:
                {
                    // zoom across map
                    break;
                }
                case EnemyState.Attack:
                {
                    // fire a projectile
                    // todo : determine whether to primary or secondary attack here
                    canPrimaryAttack = false;
                    PrimaryAttack();
                    break;
                }
                default:
                    break;
            }
        }

        protected override void PrimaryAttack()
        {
            var projectile = Instantiate(primaryProjectile, transform.position, transform.rotation);
            projectile.transform.LookAt(detector.Target);

            var hits = Physics.RaycastAll(transform.position, transform.forward, range);

            foreach (var hit in hits)
            {
                if (hit.collider.CompareTag("Player"))
                {
                    projectile.transform.LookAt(hit.point);
                    var p = projectile.GetComponent<Projectile>();
                    p.SetValues(Extensions.Damage(primaryDamage, primaryDamageBonus), hit.point, hit.normal, 60, Color);
                    p.SetShooter(gameObject);
                    break;
                }
            }
            
            base.PrimaryAttack();
        }

        protected override void SecondaryAttack()
        {
            base.SecondaryAttack();
        }
    }
}