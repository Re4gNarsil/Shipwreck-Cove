using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public AudioMixer myAudioMixer;
	public LevelManager levelManager;
	public GameObject[] customs;
	public Text pages;
	public float timer = 240;
	public Text currentLevel;
	public Text lifeCounter;
	public Text chestCounter;
	public Text levelTimer;
	public Text enemyCounter;
	public Text rules;

	private float timeLeft;
	private bool gameHasStarted = false;
	private int page = 0;

	static int difficulty = 1;
	static float gameSpeed = 1;
	static float gameAudio = .5f;

	// Use this for initialization
	void Start () {
		myAudioMixer.SetFloat("MasterVolume", gameAudio);
		if (SceneManager.GetActiveScene().name != "Scene00")
		{
			//livesLeft = initialPlayerLives;
			//StartLevel(true);
			gameHasStarted = true;
		}
	}

	void Update()
	{
		if (gameHasStarted)
		{
			if (timeLeft > 0)
			{
				timeLeft -= Time.deltaTime * gameSpeed;
				levelTimer.text = Mathf.Round(timeLeft).ToString();

			}
			else
			{
				levelManager.PlacePlayerShip(true);
			}
		}
	}

	public void GameStarted(int level)
	{
		currentLevel.text = "Level " + level.ToString();
	}

	public void EnemyCount(int enemies)
	{
		enemyCounter.text = "Enemies " + enemies.ToString();
	}

	public void ChestCounter(int chests)
	{
		chestCounter.text = "Chests " + chests.ToString();
	}

	public void LifeCounter(int lives)
	{
		lifeCounter.text = "Lives " + lives.ToString();
	}

	public void SetAudio(Slider slider)
	{
		gameAudio = slider.value;
		myAudioMixer.SetFloat("MasterVolume", gameAudio);
	}

	public void SetSpeed(Slider slider)
	{
		gameSpeed = slider.value;
		FindObjectOfType<DayNight>().SetSpeed(gameSpeed);
		FindObjectOfType<Rotate>().SetGameSpeed(gameSpeed);
	}

	public void SetDifficulty(Slider slider)
	{
		difficulty = Mathf.RoundToInt(slider.value);
	}

	public void LoadLevel(int nextLevel)
	{
		SceneManager.LoadScene("Scene0" + nextLevel.ToString());
	}

	public void StartLevel(bool newLevel, bool carryOver)
	{
		if (newLevel)
		{
			if (carryOver) { timeLeft += timer; }
			else { timeLeft = timer; }
		}
		else { timeLeft = timer; }
	}

	public void BringUpRules()
	{
		if (rules.text == "Game Rules") { 
			foreach (GameObject obj in customs)
			{
				obj.SetActive(false);
			}
			pages.gameObject.SetActive(true);
			rules.text = "Return";
			NextPage(0);
		} else
		{
			pages.gameObject.SetActive(false);
			rules.text = "Game Rules";
			foreach (GameObject obj in customs)
			{
				obj.SetActive(true);
			}
		}
	}

	public void NextPage(int flip)
	{
		page += flip;
		if (page == 0) { pages.text = "Welcome to Shipwreck Cove, a game of collecting treasure from the nearby island while avoiding the pirates that guard it.The goal is simple, take what you can, and give nothing back!"; }
		else if (page == 1) { pages.text = "The rules and controls are simple enough: use the mouse to control your camera and fire your cannons, press 'q' to reset the camera, and use the arrows to steer and move around"; }
		else if (page == 2) { pages.text = "Sail into the cove to find the treasure chests, which will glow every now and then, knocking out or avoiding enemy ships before your ship is sunk.  The chests take a little time to retrieve."; }
		else if (page == 3) { pages.text = "Once you have gotten the chests flee to your convoy to make your escape. Game speed, audio, and difficulty can all be easily adjusted as you see fit at the beginning of a game.  Good luck!"; }
		else if (page == 4)
		{
			pages.gameObject.SetActive(false);
			rules.text = "Game Rules";
			foreach (GameObject obj in customs)
			{
				obj.SetActive(true);
			}
			page = 0;
		}
	}
}
