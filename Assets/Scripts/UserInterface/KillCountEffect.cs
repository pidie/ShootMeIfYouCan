using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
	public class KillCountEffect : MonoBehaviour
	{
		[SerializeField] private float speed = 0.2f;
		[SerializeField] private float fade = 0.8f;
		
		private bool _move;
		private Image _image;
		private Transform _player;
		private Quaternion _lastRot = Quaternion.identity;

		private void Awake()
		{
			_image = GetComponent<Image>();
			StartCoroutine(FadeImage());
			_player = GameObject.Find("Player/Main").transform;
			transform.LookAt(_player);
		}

		private void FixedUpdate()
		{
			var pos = transform.position;
			transform.position = new Vector3(pos.x, pos.y + speed, pos.z);
		
			if (transform.rotation != _lastRot)
			{
				transform.rotation = Quaternion.LookRotation(transform.position - _player.position);
				_lastRot = Quaternion.identity;
			}
		}

		private IEnumerator FadeImage()
		{
			var color = new Color(_image.color.r, _image.color.g, _image.color.b, 1);

			while (_image.color.a > 0.0f)
			{
				_image.color = new Color(color.r, color.g, color.b, _image.color.a - Time.deltaTime / fade);
				yield return null;
			}
		}
	}
}
