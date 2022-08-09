using System.Collections;
using Enemies;
using Managers;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace Guns
{
    [RequireComponent(typeof(Collider))]
    public class Damageable : MonoBehaviour
    {
        [Header("Health Info")]
        [SerializeField] private float maxHealth;
        [Tooltip("Percentage of health regenerated every 5 seconds")]
        [SerializeField] private float passiveRegenerationPercent;
        [Tooltip("Bonus health restored every second")] 
        [SerializeField] private float passiveRegenerationBonus;
        [SerializeField] private bool indestructible;
        [Header("SFX")]
        [SerializeField] private AudioClip hitSoundEffect;
        [SerializeField] private AudioClip killSoundEffect;
        [SerializeField] private GameObject hitEffect;
        [SerializeField] private GameObject killEffect;
        
        private float _lastTimeStamp;
        private GameObject _activeHitEffect;
        private Image _playerHurtUIImage;
        private int _timesShot;
        private bool _isPlayer;
        private bool _showingPlayerHurt;

        public float CurrentHealth { get; private set; }

        private void Awake()
        {
            _isPlayer = GetComponent<PlayerMovement>();
            gameObject.layer = 6; // damageable layer
            _lastTimeStamp = Time.time;
            _playerHurtUIImage = GameObject.Find("UICanvas/PlayerHurt").GetComponent<Image>();

            if (_isPlayer)
                CurrentHealth = maxHealth;
        }

        private void Update()
        {
            if (CurrentHealth < maxHealth)
                RegenerateHealth();

            // any active vfx move with the transform
            if (_activeHitEffect)
            {
                _activeHitEffect.transform.position = transform.position;
                _activeHitEffect.transform.rotation = transform.rotation;
            }

            _lastTimeStamp = Time.time;
        }

        public void Set(float mHealth, float passiveRegen, float passiveBonus)
        {
            maxHealth = mHealth;
            passiveRegenerationPercent = passiveRegen;
            passiveRegenerationBonus = passiveBonus;
            CurrentHealth = mHealth;
        }

        private void RegenerateHealth()
        {
            var percentage = passiveRegenerationPercent / 5;
            percentage *= Time.time - _lastTimeStamp;

            CurrentHealth += percentage + passiveRegenerationBonus;

            if (CurrentHealth > maxHealth)
                CurrentHealth = maxHealth;
        }

        public void TakeDamage(float damage, Vector3 hitPos, Vector3 hitNormal)
        {
            _timesShot++;
            
            if (hitEffect != null)
            {
                _activeHitEffect = Instantiate(hitEffect, hitPos, Quaternion.LookRotation(hitNormal));
                _activeHitEffect.transform.localScale *= 0.1f;
                Destroy(_activeHitEffect, 1);
            }

            if (_isPlayer && _showingPlayerHurt == false)
            {
                StartCoroutine(FlashPlayerHurt());
                print($"health: {CurrentHealth}");
            }

            AudioManager.instance.PlayOneShot(hitSoundEffect);

            if (!indestructible)
            {
                CurrentHealth -= damage;
            
                if (CurrentHealth <= 0)
                    KillMe();
            }
        }

        private void KillMe()
        {
            if (killEffect != null)
            {
                var fx = Instantiate(killEffect, transform.position, quaternion.identity);
                Destroy(fx, 1);
                AudioManager.instance.PlayOneShot(killSoundEffect);
            }

            if (_isPlayer)
                GameManager.onPlayerKill.Invoke();
            else
            {
                var enemy = GetComponent<Enemy>();
                ScoreManager.onScoreUpdate(enemy.GetScoreValue(), true);
		
                // todo : make this all happen with one Action
                KillCounter.onKillCountUpdate.Invoke();
                GameManager.onEnemyKill.Invoke();
                AccuracyManager.onIncreaseCombo.Invoke();
			
                var killCountCanvas = Instantiate(enemy.GetKillCountEffect(), transform.position, quaternion.identity);
                Destroy(killCountCanvas, 1f);
            }
            Destroy(gameObject);
        }

        private IEnumerator FlashPlayerHurt()
        {
            _showingPlayerHurt = true;
            var colorNoAlpha = _playerHurtUIImage.color;
            var colorAlpha = new Color(colorNoAlpha.r, colorNoAlpha.g, colorNoAlpha.b, 1);
            _playerHurtUIImage.color = colorAlpha;
            yield return new WaitForSeconds(0.1f);
            _playerHurtUIImage.color = colorNoAlpha;
            _showingPlayerHurt = false;
        }
    }
}