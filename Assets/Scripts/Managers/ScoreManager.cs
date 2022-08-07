using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Managers
{
	public class ScoreManager : MonoBehaviour
	{
		[SerializeField] private TMP_Text scoreText;
		[SerializeField] private GameObject bonusScoreText;
		[SerializeField] private AudioClip scoreUpSound;
    
		private static ScoreManager _instance;
		private bool _scoreIncrementTracker;
		private bool _stopIncrement;

		public static Action<int, bool> onScoreUpdate;
		public int Score { get; private set; }

		private void Awake()
		{
			if (_instance != null && _instance != this)
				Destroy(this);
			else
				_instance = this;

			Score = 0;
			_scoreIncrementTracker = true;

			onScoreUpdate += ScoreUpdate;
		}

		private void Update()
		{
			if (_scoreIncrementTracker && !_stopIncrement)
				StartCoroutine(IncrementScoreOverTime());
		}

		private void OnEnable() => GameManager.onPlayerKill += OnPlayerDeath;

		private void OnDisable() => GameManager.onPlayerKill -= OnPlayerDeath;

		private void OnPlayerDeath() => _stopIncrement = true;
		
		private void ScoreUpdate(int amount, bool showFlash)
		{
			Score += amount;
			scoreText.text = Score.ToString("D10");	// todo : Localize

			if (amount > 1)
				AudioManager.instance.PlayOneShot(scoreUpSound);

			// todo : this method generates both a green and a red text each time.
			if (showFlash)
			{
				var floatingText = Instantiate(bonusScoreText, GameObject.Find("Score").transform);
				var text = floatingText.GetComponentInChildren<TMP_Text>();

				if (text)
				{
					text.color = amount > 0 ? Color.green : Color.red;
					text.text = amount.ToString();
				}
				else
					Destroy(floatingText);
			}
		}

		private IEnumerator IncrementScoreOverTime()
		{
			_scoreIncrementTracker = false;
			yield return new WaitForSeconds(0.1f);
			ScoreUpdate(1, false);
			_scoreIncrementTracker = true;
		}
	}
}