using System;
using System.Diagnostics;
using UnityEngine;

namespace Managers
{
    public class TimeManager : MonoBehaviour
    {
        private readonly Stopwatch _timer = new ();

        private TimeSpan _timeElapsedSinceLastFrame;
        
        public TimeSpan GetTimeElapsed { get; private set; }

        public static Action<float> onTimeScaleChange;

        private void Awake()
        {
            onTimeScaleChange += ChangeTimeScale;
            _timer.Start();
        }

        private void Update()
        {
            GetTimeElapsed += (_timer.Elapsed - _timeElapsedSinceLastFrame) * Time.timeScale;
            _timeElapsedSinceLastFrame = _timer.Elapsed;
        }

        private void OnEnable() => GameManager.onPlayerKill += OnPlayerDeath;

        private void OnDisable() => GameManager.onPlayerKill -= OnPlayerDeath;

        private void OnPlayerDeath() => _timer.Stop();

        private void OnDestroy() => _timer.Stop();

        private void ChangeTimeScale(float value)
        {
            Time.timeScale = value;
            _timer.Restart();
            _timeElapsedSinceLastFrame = new TimeSpan();
        }
    }
}