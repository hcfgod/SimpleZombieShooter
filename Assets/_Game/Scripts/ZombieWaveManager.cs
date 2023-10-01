using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieWaveManager : MonoBehaviour
{
	public static ZombieWaveManager Instance;
	
	[SerializeField] private List<GameObject> zombiePrefabs;
	
	[SerializeField] private float timeBetweenWaves = 5.0f;
	[SerializeField] private float delayBetweenZombieSpawns = 5.0f;
	
	[SerializeField] private List<Transform> spawnpoints = new List<Transform>();
	
	[SerializeField] private int difficulty;
	
	[SerializeField] private int currentWave;
	[SerializeField] private int zombiesLeft;
	[SerializeField] private int zombiesToSpawn;

	void Awake()
	{
		Instance = this;
	}
	
	void Start()
	{
		StartCoroutine(WaveController());
	}

	IEnumerator WaveController()
	{
		while (true)
		{
			yield return StartCoroutine(HandleWave());
			yield return new WaitForSeconds(timeBetweenWaves);
		}
	}

	IEnumerator HandleWave()
	{
		StartNewWave(currentWave + 1);

		while (zombiesToSpawn > 0 || zombiesLeft > 0)
		{
			if (zombiesToSpawn > 0)
			{
				SpawnZombie();
				zombiesToSpawn--;
			}
			yield return new WaitForSeconds(delayBetweenZombieSpawns); // Time between each zombie spawn
		}
	}

	void StartNewWave(int waveNumber)
	{
		currentWave = waveNumber;
		zombiesToSpawn = CalculateZombiesToSpawn(currentWave);
		zombiesLeft = zombiesToSpawn;
	}

	int CalculateZombiesToSpawn(int wave)
	{
		return wave * difficulty;
	}

	void SpawnZombie()
	{
		// Instantiate your zombie prefab at a spawn location
		GameObject randomZombiePrefab = zombiePrefabs[Random.Range(0, zombiePrefabs.Count)];
		Transform randomSpawnpoint = spawnpoints[Random.Range(0, spawnpoints.Count)];
		
		GameObject instantiatedZombieObject = Instantiate(randomZombiePrefab, randomSpawnpoint.position, Quaternion.identity);
	}

	public void OnZombieKilled()
	{
		zombiesLeft--;
	}
}
