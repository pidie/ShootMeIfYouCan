using System;
using Cinemachine;
using Guns;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private GunBehavior gun;
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private CinemachineInputProvider inputProvider;
        [SerializeField] private InputActionReference playerXYAxis;

        private PlayerControls _playerControls;
        private bool _reportAccurateDelta;
        public bool DoNotMoveCamera { get; private set; }

        public bool IsPlayerDead { get; set; }

        public static InputManager Instance { get; private set; }
        public static Action<bool> onDoNotMoveCamera;

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;

            _playerControls = new PlayerControls();

            _playerControls.PlayerGeneric.Jump.performed += _ => PlayerMovement.jumpCallback.Invoke();
            _playerControls.PlayerGeneric.Fire.performed += _ => {
                if (!DoNotMoveCamera)
                    gun.Fire(IsPlayerDead);
            };
            _playerControls.PlayerGeneric.MenuScreen.performed += _ => pauseMenu.SetActive(!pauseMenu.activeSelf);
            onDoNotMoveCamera += DisableCameraMotion;
        }

        private void OnEnable() => _playerControls.Enable();

        private void OnDisable() => _playerControls.Disable();

        public Vector2 GetPlayerMovement() => _playerControls.PlayerGeneric.Movement.ReadValue<Vector2>();

        public Vector2 GetMouseDelta()
        {
            if (IsPlayerDead || DoNotMoveCamera) return Vector2.zero;
            return _reportAccurateDelta ? _playerControls.PlayerGeneric.Look.ReadValue<Vector2>() : Vector2.zero;
        }

        public bool PlayerJumpedThisFrame() => _playerControls.PlayerGeneric.Jump.WasPressedThisFrame();

        private void DisableCameraMotion(bool value)
        {
            DoNotMoveCamera = value;
            inputProvider.XYAxis = value ? null : playerXYAxis;
        }
    }
}