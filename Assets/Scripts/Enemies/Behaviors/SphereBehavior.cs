using System;
using System.Collections;
using Guns;
using Managers;
using Unity.Mathematics;
using UnityEngine;

namespace Enemies.Behaviors
{
    public class SphereBehavior : Enemy
    {
        [Header("Bobbing")]
        [SerializeField] private float bobbingDistance;
        [SerializeField] private float bobbingSpeed;

        private bool _canFire;
        private bool _isHunting;
        private Vector3 _restPosition;

        protected override void Awake()
        {
            base.Awake();
            _canFire = true;
            _restPosition = transform.position;
        }

        protected override void Update()
        {
            base.Update();
            
            if (_canFire && detector.Distance(transform) <= range)
            {
                _canFire = false;
                Fire();
            }

            var pos = transform.position;

            // transform.position = new Vector3(
            //     pos.x, _restPosition.y + (float) Math.Sin(Time.time * bobbingSpeed) * bobbingDistance, pos.z);

            if (detector.IsPlayerDetected)
            {
                _isHunting = false;
                transform.LookAt(detector.Target);
                transform.position += transform.forward * (movementSpeed * Time.deltaTime);
            }
            else if (detector.PlayerDetectedFromPoint != Vector3.zero)
            {
                _isHunting = true;
                transform.LookAt(detector.PlayerDetectedFromPoint);
                transform.position += transform.forward * (movementSpeed * Time.deltaTime);

                var tx = (float) Math.Round(pos.x, 2);
                var tz = (float) Math.Round(pos.z, 2);
                var dx = (float) Math.Round(detector.PlayerDetectedFromPoint.x, 2);
                var dz = (float) Math.Round(detector.PlayerDetectedFromPoint.z, 2);

                if (tx == dx && tz == dz && _isHunting)
                {
                    _isHunting = false;
                    detector.PlayerLost();
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("Floor"))
            {
                var x = transform.localEulerAngles.x;
                var y = transform.localEulerAngles.y + 90f;
                var z = transform.localEulerAngles.z;
                transform.rotation = quaternion.Euler(x, y, z);
            }
        }

        private void Fire()
        {
            var projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
            projectile.transform.LookAt(detector.Target);

            var hits = Physics.RaycastAll(transform.position, transform.forward, range);

            foreach (var hit in hits)
            {
                if (hit.collider.CompareTag("Player"))
                {
                    projectile.transform.LookAt(hit.point);
                    var p = projectile.GetComponent<Projectile>();
                    p.SetValues(damage, hit.point, hit.normal, 60, Color);
                    p.SetShooter(gameObject);
                    break;
                }
            }
            StartCoroutine(Cooldown());
            AudioManager.instance.PlayOneShot(weaponFireSound);
        }

        private IEnumerator Cooldown()
        {
            yield return new WaitForSeconds(weaponCooldown);
            _canFire = true;
        }
    }
}