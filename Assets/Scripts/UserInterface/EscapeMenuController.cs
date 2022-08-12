using Managers;
using Player;
using UnityEngine;

namespace UserInterface
{
    public class EscapeMenuController : MonoBehaviour
    {
        private void Awake() => GameManager.onPlayerKill += OnPlayerDeath;

        private void OnEnable()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            TimeManager.onTimeScaleChange.Invoke(0.1f);
            InputManager.onDoNotMoveCamera.Invoke(true);
        }

        private void OnDisable()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            TimeManager.onTimeScaleChange.Invoke(1f);
            InputManager.onDoNotMoveCamera.Invoke(false);
        }

        private void OnPlayerDeath() => gameObject.SetActive(false);
    }
}
