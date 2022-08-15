using System;
using System.Collections;
using System.Net;
using Guns;
using Managers;
using Unity.Mathematics;
using UnityEngine;

namespace Enemies.Behaviors
{
    public class SphereBehavior : Enemy
    {
        protected override void Update()
        {
            base.Update();

            var pos = transform.position;

            if (state == EnemyState.Attack) { }
            else if (detector.IsPlayerDetected)
                state = EnemyState.Hunting;
            else if (detector.PlayerDetectedFromPoint != Vector3.zero)
                state = EnemyState.Searching;
            else
                state = EnemyState.Patrolling;

            switch (state)
            {
                case EnemyState.Idle:
                {
                    // bobbing
                    break;
                }
                case EnemyState.Patrolling:
                {
                    // move to waypoint
                    if (Vector3.Distance(transform.position, targetWaypoint.transform.position) < 0.2f)
                    {
                        AddVisitedWaypoint(targetWaypoint);
                        targetWaypoint.Depart();
                        var newWaypoint = SetTargetWaypoint(targetWaypoint.GetRandomNeighbor());
                        newWaypoint.RequestIncoming(waypointsVisited[0]);
                        transform.LookAt(targetWaypoint.transform);
                    }
                    transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.transform.position, movementSpeed * Time.deltaTime);

                    break;
                }
                case EnemyState.Searching:
                {
                    // move to lastKnownPlayerCoordinate
                    transform.LookAt(detector.PlayerDetectedFromPoint);
                    transform.position += transform.forward * (movementSpeed * Time.deltaTime);

                    var tx = (float)Math.Round(pos.x, 2);
                    var tz = (float)Math.Round(pos.z, 2);
                    var dx = (float)Math.Round(detector.PlayerDetectedFromPoint.x, 2);
                    var dz = (float)Math.Round(detector.PlayerDetectedFromPoint.z, 2);

                    if (tx == dx && tz == dz)
                        detector.PlayerLost();
                    
                    break;
                }
                case EnemyState.Hunting:
                {
                    // track player
                    transform.LookAt(detector.Target);
                    transform.position += transform.forward * (movementSpeed * Time.deltaTime);

                    if (canPrimaryAttack && detector.Distance(transform) <= range)
                        state = EnemyState.Attack;
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