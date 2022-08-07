using System.Collections;
using Managers;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Guns
{
    public class GunBehavior : MonoBehaviour
    {
        [SerializeField] private float range;
        [SerializeField] private Damage damageRange;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private float fireRate;
        [SerializeField] private float maxCharge;
        [SerializeField] private float chargeCost;
        [SerializeField] private bool rechargeable;
        [SerializeField] private float rechargeTime;
        [Tooltip("Amount of charge gained each second")]
        [SerializeField] private float rechargeRate;
        [SerializeField] private GameObject muzzleFlashEffect;
        [SerializeField] private AudioClip weaponFireSound;
        [SerializeField] private Vector3 recoil;
        [SerializeField] private Sprite[] batteryImages;
        [SerializeField] private Image batteryImage;

        private Transform _cam;
        private bool _fired;
        private GameObject _activeFlashEffect;
        private float _currentCharge;
        private bool _canFire;
        private bool _canRecharge;
        private bool _weaponDepleted;

        private void Awake()
        {
            _cam = Camera.main.transform;
            _currentCharge = maxCharge;
            _canFire = true;
        }

        private void Update()
        {
            if (_activeFlashEffect)
            {
                _activeFlashEffect.transform.position = transform.position;
                _activeFlashEffect.transform.rotation = transform.rotation;
            }

            if (_canRecharge)
            {
                _currentCharge += rechargeRate * Time.deltaTime;
            
                if (_currentCharge > maxCharge)
                    _currentCharge = maxCharge;
            }
        
            batteryImage.sprite = (_currentCharge / maxCharge) switch
            {
                > .8f => batteryImages[0],
                > .6f => batteryImages[1],
                > .4f => batteryImages[2],
                > .2f => batteryImages[3],
                > .05f => batteryImages[4],
                > 0f => batteryImages[5],
                _ => batteryImages[6]
            };
        }

        public Vector3 GetRecoil() => recoil;

        public void Fire(bool isPlayerDead)
        {
            if (isPlayerDead) return;
            
            var damageModifier = 1f;
        
            if (_currentCharge - chargeCost < 0)
            {
                damageModifier = _currentCharge / chargeCost;
                _weaponDepleted = true;
            }
        
            if (_fired || _currentCharge == 0) return;
        
            _canRecharge = false;

            _fired = true;
            StartCoroutine(WaitToFire(fireRate));
        
            // add recoil
            GunRecoil.onApplyRecoil.Invoke();

            // muzzle flash
            _activeFlashEffect = Instantiate(muzzleFlashEffect, transform.position, transform.rotation);
            var timeToDestroyFlash = Mathf.Min(fireRate, _activeFlashEffect.GetComponent<ParticleSystem>().main.duration);
            Destroy(_activeFlashEffect, timeToDestroyFlash);

            // create projectile
            var projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);

            // aim and initialize projectile
            var hits = Physics.RaycastAll(_cam.position, _cam.forward, 1000);
        
            foreach (var hit in hits)
            {
                if (hit.collider.GetComponent<Damageable>())
                {
                    projectile.transform.LookAt(hit.point);
                    var p = projectile.GetComponent<Projectile>();
                    p.SetIsPlayer();
                    p.SetValues(Damage(damageModifier), hit.point, hit.normal, range, GameManager.GetColor());
                    break;
                }
            }
            AudioManager.instance.PlayOneShot(weaponFireSound);
        
            Destroy(projectile, 2f);

            _currentCharge -= chargeCost;
            if (_currentCharge < 0)
                _currentCharge = 0;

            if (rechargeable && _canFire)
            {
                StopCoroutine(RechargeWeapon());
                StartCoroutine(RechargeWeapon());
            }
        }

        private float Damage(float modifier) => Random.Range(damageRange.Min, damageRange.Max) * modifier;

        private IEnumerator WaitToFire(float timeToWait)
        {
            yield return new WaitForSeconds(timeToWait);
            _fired = false;
        }

        private IEnumerator RechargeWeapon()
        {
            if (_weaponDepleted)
            {
                BatteryLevel.onIsDepleted.Invoke(true);
                yield return new WaitForSeconds(5f);
                _weaponDepleted = false;
                BatteryLevel.onIsDepleted.Invoke(false);
            }
            yield return new WaitForSeconds(rechargeTime);
            _canRecharge = true;
        }
    }
}