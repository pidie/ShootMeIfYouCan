using Guns;
using UnityEngine;

namespace Player
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private GunBehavior gun;

        private PlayerControls _playerControls;
        private bool _reportAccurateDelta;

        public bool IsPlayerDead { get; set; }

        public static InputManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;

            _playerControls = new PlayerControls();

            _playerControls.PlayerGeneric.Jump.performed += _ => PlayerMovement.jumpCallback.Invoke();
            _playerControls.PlayerGeneric.Fire.performed += _ => gun.Fire(IsPlayerDead);
        }

        private void OnEnable() => _playerControls.Enable();

        private void OnDisable() => _playerControls.Disable();

        public Vector2 GetPlayerMovement() => _playerControls.PlayerGeneric.Movement.ReadValue<Vector2>();

        public Vector2 GetMouseDelta()
        {
            if (IsPlayerDead) return Vector2.zero;
            return _reportAccurateDelta ? _playerControls.PlayerGeneric.Look.ReadValue<Vector2>() : Vector2.zero;
        }

        public bool PlayerJumpedThisFrame() => _playerControls.PlayerGeneric.Jump.WasPressedThisFrame();
    }
}