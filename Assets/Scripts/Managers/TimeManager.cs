using System;
using System.Diagnostics;
using UnityEngine;

namespace Managers
{
    public class TimeManager : MonoBehaviour
    {
        private readonly Stopwatch _timer = new ();

        public TimeSpan GetTimeElapsed => _timer.Elapsed;

        private void Awake() => _timer.Start();

        private void OnEnable() => GameManager.onPlayerKill += OnPlayerDeath;

        private void OnDisable() => GameManager.onPlayerKill -= OnPlayerDeath;

        private void OnPlayerDeath() => _timer.Stop();

        private void OnDestroy()
        {
            // todo : this will have to stop at the end of a level
            _timer.Stop();
        }
    }
}