using UnityEngine;

namespace Player
{
	public class PlayerController : MonoBehaviour
	{
		[SerializeField] private float playerSpeed = 2.0f;
		[SerializeField] private float jumpHeight = 1.0f;
		[SerializeField] private float gravityValue = -9.81f;
	
		private CharacterController _controller;
		private Vector3 _playerVelocity;
		private bool _groundedPlayer;
		private InputManager _inputManager;

		private void Awake()
		{
			_inputManager = InputManager.Instance;
			_controller = GetComponent<CharacterController>();
		}

		private void Update()
		{
			_groundedPlayer = _controller.isGrounded;
			if (_groundedPlayer && _playerVelocity.y < 0)
				_playerVelocity.y = -2f;

			var movement = _inputManager.GetPlayerMovement();
			var move = new Vector3(movement.x, 0, movement.y);
			_controller.Move(move * (Time.deltaTime * playerSpeed));

			if (move != Vector3.zero)
			{
				gameObject.transform.forward = move;
			}

			if (_inputManager.PlayerJumpedThisFrame() && _groundedPlayer)
			{
				_playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
			}

			_playerVelocity.y += gravityValue * Time.deltaTime;
			_controller.Move(_playerVelocity * Time.deltaTime);
		}
	}
}