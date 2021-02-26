using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class Round : MonoBehaviour
{
	[SerializeField] private float roundTime, nextRoundDelay;
	[SerializeField] private Text roundText, pointsText;

	private List<SteeringController> flies = new List<SteeringController>();
	private List<SteeringController> accountedFlies = new List<SteeringController>();
	private int roundPoints = 1;
	private int highscore;
	private bool gameOver = false, pauseCountdown;

	// Start is called before the first frame update
	void Awake()
    {
		flies.AddRange(FindObjectsOfType<SteeringController>());
		pointsText.text = string.Format("Round: {0}", roundPoints);
		StartCoroutine(RoundTime());
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.P))
			pauseCountdown = !pauseCountdown;
	}

    IEnumerator RoundTime()
    {
		float time = roundTime;
		accountedFlies = new List<SteeringController>();
		roundPoints = 0;
		while (!gameOver)
        {
			if (!pauseCountdown)
				time -= Time.deltaTime;
			roundText.text = string.Format("{0:00.00}", time);
            foreach (SteeringController steering in flies)
            {
                if (!steering.gameObject.activeSelf && !accountedFlies.Contains(steering))
				{
					accountedFlies.Add(steering);
				}
			}
			if (time <= 0.0f && accountedFlies.Count == flies.Count)
            {
				roundPoints++;
				roundText.text = string.Format("<color=green>{0:00.00}</color>", 0.00f);
				pointsText.text = string.Format("Round: {0}", roundPoints);
				yield return new WaitForSeconds(nextRoundDelay);
				time = roundTime;
				accountedFlies = new List<SteeringController>();
				//roundPoints = 0;
			}
			else if (time <= 0.0f && accountedFlies.Count < flies.Count)
			{
				roundText.text = string.Format("<color=red>{0:00.00}. Press Space To Restart</color>", 0.00f);
				gameOver = true;
				highscore = roundPoints;
				//SaveScore();
				foreach (SteeringController fly in flies)
				{
					fly.gameObject.SetActive(false);
				}
			}
			yield return null;
		}
    }

	public void Restart()
	{
		if (gameOver)
		{
			gameOver = false;
			foreach (SteeringController fly in flies)
			{
				fly.gameObject.SetActive(true);
			}
			StartCoroutine(RoundTime());
		}
	}

	public bool IsGameOver() => gameOver;

	void SaveScore()
	{
		PlayerPrefs.SetInt("highscore", highscore);
	}

	int GetScore()
	{
		return PlayerPrefs.GetInt("highscore");
	}
}
