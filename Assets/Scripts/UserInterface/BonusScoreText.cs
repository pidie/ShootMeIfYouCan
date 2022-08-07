using System.Collections;
using TMPro;
using UnityEngine;

namespace UserInterface
{
	public class BonusScoreText : MonoBehaviour
	{
		[SerializeField] private float speed;
		[SerializeField] private float timeToFade;
		[SerializeField] private TMP_Text text;
	
		private void Awake()
		{
			StartCoroutine(KillSelf());
			StartCoroutine(FadeText());
			text = GetComponentInChildren<TextMeshProUGUI>();
		}

		private void FixedUpdate()
		{
			var pos = transform.position;

			transform.position = new Vector3(pos.x, pos.y - speed, pos.z);
		}

		private IEnumerator KillSelf()
		{
			yield return new WaitForSeconds(2f);
			Destroy(gameObject);
		}
	
		private IEnumerator FadeText()
		{
			text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
			while (text.color.a > 0.0f)
			{
				text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - (Time.deltaTime / timeToFade));
				yield return null;
			}
		}
	}
}
