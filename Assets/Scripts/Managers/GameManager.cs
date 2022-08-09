using System;
using System.Collections;
using Enemies;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Color[] colors;
        [SerializeField] private Material gunMaterial;
        [SerializeField] private Enemy[] enemies;
        [SerializeField] private Spawner[] spawners;
        [SerializeField] private GameObject gameOverMenu;

        private static Color _targetColor;
        private bool _canSpawn;
        private int _colorPoolSize;
        private int _activeEnemyCount;
        private int _maxEnemies;
        private bool _waitingToIncreaseEnemyStats;
        private bool _waitingToIncreaseMaxEnemies;
        private bool _waitingToAddNewColor;
        private int _increaseStatCounter;
        private int _increaseEnemiesCounter;
        private static float _changeGunColorChance;
        private static float _changeGunColorIncrement;

        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
        private static Action _onChangeColor;

        public static Action onEnemyKill;
        public static Action onPlayerKill;
        public static float EnemyHealthBonus { get; private set; }
        public static float EnemyDamageBonus { get; private set; }

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            _targetColor = Color.white;
            // _changeColor = true;
            _canSpawn = true;
            _colorPoolSize = 2;
            _activeEnemyCount = 0;
            _maxEnemies = 10;
            _changeGunColorChance = 0.015f;
            _changeGunColorIncrement = 0.0005f;

            EnemyHealthBonus = 1;
            EnemyDamageBonus = 1;

            spawners = FindObjectsOfType<Spawner>();

            StartCoroutine(AddNewColor());
            ChangeColor();
        }

        private void Update()
        {
            if (!_waitingToIncreaseEnemyStats)
                StartCoroutine(IncreaseEnemyStatBonus());

            if (!_waitingToIncreaseMaxEnemies)
                StartCoroutine(IncreaseMaxEnemies());

            if (!_waitingToAddNewColor)
                StartCoroutine(AddNewColor());

            if (_canSpawn)
                StartCoroutine(SpawnEnemy());
        }

        private void OnEnable()
        {
            onEnemyKill += DecrementActiveEnemyCount;
            onPlayerKill += GameOver;
            _onChangeColor += ChangeColor;
        }

        private void OnDisable()
        {
            onEnemyKill -= DecrementActiveEnemyCount;
            onPlayerKill -= GameOver;
        }

        private void GameOver()
        {
            if (gameOverMenu == null) return;
            
            gameOverMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            InputManager.Instance.IsPlayerDead = true;
        }

        public static Color GetColor() => _targetColor;

        private void ChangeColor()
        {
            var newColor = _targetColor;

            while (newColor == _targetColor)
                _targetColor = colors[Random.Range(0, _colorPoolSize)];

            gunMaterial.SetColor(EmissionColor, _targetColor);
            BatteryLevel.onUpdateColor(_targetColor);
        }

        public static bool CheckChangeGunColor()
        {
            var chanceQuantified = Random.Range(0f, 1f);
            var chance = chanceQuantified < _changeGunColorChance;
            
            if (chance == false)
                _changeGunColorChance += _changeGunColorIncrement;
            else
            {
                _changeGunColorChance = 0.015f;
                _changeGunColorIncrement += 0.000018f;
                _onChangeColor.Invoke();
            }
            
            return chance;
        }

        private IEnumerator SpawnEnemy()
        {
            _canSpawn = false;
            var spawner = spawners[Random.Range(0, spawners.Length)];
            var enemy = enemies[Random.Range(0, _colorPoolSize)];
            spawner.SpawnEnemy(enemy);
            IncrementActiveEnemyCount();
        
            if (_activeEnemyCount >= _maxEnemies) 
                yield return null;
            else
            {
                yield return new WaitForSeconds(5);
                _canSpawn = true;
            }
        }

        private IEnumerator AddNewColor()
        {
            _waitingToAddNewColor = true;
            yield return new WaitForSeconds(180);
        
            _colorPoolSize++;
            _waitingToAddNewColor = false;
        }

        private IEnumerator IncreaseEnemyStatBonus()
        {
            _increaseStatCounter++;
            _waitingToIncreaseEnemyStats = true;
            yield return new WaitForSeconds(20f);
        
            if (_increaseStatCounter == 6)
                _increaseStatCounter = 0;
            else
            {
                EnemyHealthBonus += 0.05f;
                EnemyDamageBonus += 0.075f;
            }
        
            _waitingToIncreaseEnemyStats = false;
        }

        private IEnumerator IncreaseMaxEnemies()
        {
            _waitingToIncreaseMaxEnemies = true;
            yield return new WaitForSeconds(60f);

            if (_increaseEnemiesCounter == 3)
                _increaseEnemiesCounter = 0;
            else
                _maxEnemies++;
    
            _waitingToIncreaseMaxEnemies = false;
        }
    
        private void IncrementActiveEnemyCount() => _activeEnemyCount++;

        private void DecrementActiveEnemyCount() => _activeEnemyCount--;
    }
}