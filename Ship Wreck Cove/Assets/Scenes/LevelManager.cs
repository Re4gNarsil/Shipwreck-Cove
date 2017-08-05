using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

	public GameObject[] rocks;
	public GameObject enemyShip;
	public GameObject playerShip;
	public GameObject playersConvoy;
	public GameObject treasureChest;

	public int rockClusterMax = 16;
	public int initialEnemyShips = 5;
	public int initialTreasureChests = 2;
	public int initialPlayerLives = 10;
	public float maxScale = 7.5f;
	public bool regenHealthEachLevel = true;
	public bool carryOverTimeLeft = true;

	private List<Vector2> levelObjects = new List<Vector2>();
	private GameObject[] rocksInLevel;
	private bool gameHasStarted = false;
	private int levelCount = 0;
	private int chestsLeft;
	private int livesLeft;
	private int enemiesLeft;
	private float timeLeft;

	static float gameSpeed = 1;

	// Use this for initialization
	void Start () {
		FindObjectOfType<DayNight>().SetSpeed(gameSpeed);
		if (SceneManager.GetActiveScene().name != "Scene00")
		{
			playerShip.GetComponent<GroundCharacter>().SetGameSpeed(gameSpeed);
			playerShip.GetComponent<CharacterMultiFiring>().SetGameSpeed(gameSpeed);
			livesLeft = initialPlayerLives;
			StartLevel(true);
			gameHasStarted = true;
		}
	}

	public void StartLevel(bool newLevel)
	{
		enemiesLeft = 0;
		chestsLeft = 0;
		levelCount++;
		FindObjectOfType<GameManager>().StartLevel(newLevel, carryOverTimeLeft);

		if (levelObjects.Count > 0) { levelObjects.Clear(); }
		DestroyAllObjects();
		CreateRockClusters();

		int numberOfShips = initialEnemyShips + levelCount + Random.Range(-2, 2);
		SpawnEnemyShips(numberOfShips);

		int numberOfChests = Mathf.Clamp(levelCount + Random.Range(-2, 2), 1, 100);
		SpawnTreasureChests(numberOfChests);

		PlacePlayerShip(false);
		FindObjectOfType<GameManager>().GameStarted(levelCount);
	}

	void CreateRockClusters()
	{
		int rockClusters = Random.Range(Mathf.RoundToInt(rockClusterMax/2), rockClusterMax);
		for (int i = 0; i < rockClusters; i++)
		{
		tryagain:
			float newPlacementX = Random.Range(2.5f, 47.5f) * 20;
			float newPlacementZ = Random.Range(25, 40) * 20;

			if (levelObjects.Count > 0)
			{
				foreach (Vector2 objectPlace in levelObjects)
				{
					if (objectPlace == new Vector2(newPlacementX, newPlacementZ)) { goto tryagain; }
				}
			}
			GameObject newCluster = new GameObject();
			newCluster.transform.position = new Vector3(newPlacementX, 6, newPlacementZ);
			newCluster.tag = "Rock";

			int rocksInCluster = Random.Range(1, 5);
			for (int n = 0; n < rockClusters; n++)
			{
				int rockNum = Random.Range(0, rocks.Length);
				float randomScale = Random.Range(3, maxScale);
				float newPositionX = Random.Range(-10, 10);
				float newPositionZ = Random.Range(-10, 10);
				float newRotationY = Random.Range(-180, 180);

				GameObject newRock = Instantiate(rocks[rockNum]);
				newRock.transform.parent = newCluster.transform;
				newRock.transform.localPosition = new Vector3(newPositionX, 0, newPositionZ);
				newRock.transform.rotation = Quaternion.Euler(0, newRotationY, 0);
				newRock.transform.localScale = Vector3.one * randomScale;
				newRock.tag = "Rock";
			}
			levelObjects.Add(new Vector2(newPlacementX, newPlacementZ));
		}
	}

	void SpawnEnemyShips(int numOfShips)
	{
		for (int i = 0; i < numOfShips; i++)
		{
		tryagain:
			float newPositionX = Random.Range(15, 40) * 20;
			float newPositionZ = Random.Range(25, 30) * 20;
			float newRotationY = Random.Range(-180, 180);

			if (levelObjects.Count > 0)
			{
				foreach (Vector2 objectPlace in levelObjects)
				{
					if (objectPlace == new Vector2(newPositionX, newPositionZ)) { goto tryagain; }
				}
			}
			GameObject newShip = Instantiate(enemyShip);
			newShip.transform.position = new Vector3(newPositionX, 8, newPositionZ);
			newShip.transform.rotation = Quaternion.Euler(0, newRotationY, 0);
			newShip.tag = "Ship";
			enemiesLeft++;
			FindObjectOfType<GameManager>().EnemyCount(enemiesLeft);
			levelObjects.Add(new Vector2(newPositionX, newPositionZ));
			newShip.GetComponent<GroundCharacter>().SetGameSpeed(gameSpeed);
			newShip.GetComponent<CharacterMultiFiring>().SetGameSpeed(gameSpeed);
		}
	}

	void SpawnTreasureChests(int numOfChests)
	{
		for (int i = 0; i < numOfChests; i++)
		{
			int randomLocation = Random.Range(0, 1);
			float newPositionX;
			float newPositionZ;

			if (randomLocation == 0)
			{
				newPositionX = Random.Range(275, 500);
				newPositionZ = Random.Range(240, 380);
			} else
			{
				newPositionX = Random.Range(680, 790);
				newPositionZ = Random.Range(240, 420);
			}
			float newRotationY = Random.Range(-180, 180);

			GameObject newChest = Instantiate(treasureChest);
			newChest.transform.position = new Vector3(newPositionX, 1, newPositionZ);
			newChest.transform.rotation = Quaternion.Euler(0, newRotationY, 0);

			chestsLeft++;
			FindObjectOfType<GameManager>().ChestCounter(chestsLeft);
		}
	}

	void DestroyAllObjects()
	{
		EnemyShip[] ships = FindObjectsOfType<EnemyShip>();
		GameObject[] rocks = GameObject.FindGameObjectsWithTag("Rock");
		GameObject[] convoy = GameObject.FindGameObjectsWithTag("Convoy");

		foreach (EnemyShip ship in ships)
		{
			DestroyObject(ship.gameObject);
		}
		foreach (GameObject rock in rocks)
		{
			DestroyObject(rock);
		}
		foreach (GameObject ship in convoy)
		{
			DestroyObject(ship);
		}
	}

	void EndGame()
	{
		FindObjectOfType<PlayerShip>().gameObject.GetComponentInChildren<CharacterCamera>().gameObject.transform.parent = null;
		FindObjectOfType<PlayerShip>().GetComponent<Ship>().Sinking();
	}

	void ResetAudio()
	{
		GameObject[] audioChildren = GameObject.FindGameObjectsWithTag("Respawn");
		foreach(GameObject obj in audioChildren)
		{
			Destroy(obj);
		}
	}

	public float GetGameSpeed()
	{
		return gameSpeed;
	}

	public void PlacePlayerShip(bool died)
	{
		if (died)
		{
			if (playerShip.GetComponent<HealthBar>()) { playerShip.GetComponent<HealthBar>().RestoreHealth(); }
			livesLeft--;
			if (livesLeft == 0) {
				EndGame();
				return;
			} else
			{
				Invoke("ResetAudio", .05f);
			}
		}
		else if (regenHealthEachLevel) {
			if (playerShip.GetComponent<HealthBar>()) { playerShip.GetComponent<HealthBar>().RestoreHealth(); }
			Invoke("ResetAudio", .05f);
		}

		int newPositionX = Random.Range(100, 900);
		float newPositionZ = Random.Range(900, 1100);
		float newRotationY = Random.Range(-180, 180);

		playerShip.transform.position = new Vector3(newPositionX, 8, newPositionZ);
		playerShip.transform.rotation = Quaternion.Euler(0, newRotationY, 0);

		FindObjectOfType<GameManager>().LifeCounter(livesLeft);
		playerShip.GetComponent<Rigidbody>().velocity = Vector3.zero;
	}

	public void SpawnPlayersConvoy()
	{
		float newPositionX = (Random.Range(0, 1) * 900) + 50;
		float newPositionZ = Random.Range(700, 1100);
		float newRotationY = Random.Range(-180, 180);

		GameObject newConvoy = Instantiate(playersConvoy);
		newConvoy.transform.position = new Vector3(newPositionX, 8, newPositionZ);
		newConvoy.transform.rotation = Quaternion.Euler(0, newRotationY, 0);
	}

	public void RetrieveChest()
	{
		chestsLeft--;
		if (chestsLeft <= 0)
		{
			SpawnPlayersConvoy();
		}
		FindObjectOfType<GameManager>().ChestCounter(chestsLeft);
	}

	public void ReduceEnemyCount(int number)
	{
		enemiesLeft -= number;
		FindObjectOfType<GameManager>().ChestCounter(chestsLeft);
	}
}
