using System;
using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float movementSpeed;
        [SerializeField] private float jumpHeight;
        [SerializeField] private FloorSensor floorSensor;

        private CharacterController _controller;
        private const float Gravity = -9.81f;
        private Vector3 _downwardVelocity;
        private InputManager _inputManager;
        private Transform _cameraTransform;

        public static Action jumpCallback;

        private void Awake()
        {
            _inputManager = InputManager.Instance;
            _controller = GetComponent<CharacterController>();
            _cameraTransform = Camera.main.transform;
            jumpCallback += Jump;
        }

        private void Update()
        {
            // x/z axis movement
            var move = _inputManager.GetPlayerMovement();
        
            var movement = new Vector3(move.x, 0, move.y) * movementSpeed;

            movement = _cameraTransform.forward * movement.z + _cameraTransform.right * movement.x;
            movement.y = 0;
        
            if (!_inputManager.DoNotMoveCamera)
                _controller.Move(movement * Time.deltaTime);
        
            // gravity
            if (!floorSensor.IsGrounded)
                _downwardVelocity.y += Gravity * Time.deltaTime;
            else
                _downwardVelocity = new Vector3(0, -2f, 0);

            _controller.Move(_downwardVelocity * Time.deltaTime);
        }

        public void Jump()
        {
            if (floorSensor.IsGrounded)
            {
                _downwardVelocity.y = Mathf.Sqrt(jumpHeight * -2f * Gravity);
                _controller.Move(_downwardVelocity * Time.deltaTime);
            }
        }
    }
}