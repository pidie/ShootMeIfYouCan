using Managers;
using TMPro;
using UnityEngine;

namespace UserInterface
{
    public class Timer : MonoBehaviour
    {
        [SerializeField] private TMP_Text timeStamp;
        
        private TimeManager _timeManager;

        private void Awake() => _timeManager = GameObject.Find("GameManager").GetComponent<TimeManager>();
        
        private void Update()
        {
            var time = _timeManager.GetTimeElapsed;
            // todo : Localize
            timeStamp.text = $"{time.Hours:D2}:{time.Minutes:D2}:{time.Seconds:D2}:{time.Milliseconds:D3}";
        }
    }
}