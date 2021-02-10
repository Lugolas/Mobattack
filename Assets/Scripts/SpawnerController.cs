using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerController : MonoBehaviour {
  [System.Serializable]
  public class Wave {
    public int number;
    public GameObject enemy;
    public int amount;
    public float delay;
  }
  float startTime;
  public GameObject necessaryObject;
  public List<SpawnPointController> spawnPointObjects = new List<SpawnPointController> ();
  public bool allDead = true;
  public int currentWaveAmount;
  public int currentWaveIndex = -1;

  public List<Wave> waves = new List<Wave> ();
  public enum SpawnerState
  {
    Spawning,
    WaitingForDeath,
    WaitingForNextWave
  }
  public SpawnerState state = SpawnerState.WaitingForNextWave;
  public List<EnemyController> spawnedEnemies = new List<EnemyController>();
  GameController gameController;
  bool waitUntilAllDead = true;
  int lastWaveNumberKnown;

  void Start () {
    gameController = GameObject.Find("GameController").GetComponent<GameController>();
    startTime = Time.time;
    waitUntilAllDead = gameController.waitUntilAllDead;
  }

  void Update () {
    if (!gameController.forbiddenSpawn) {
      if (state == SpawnerState.WaitingForNextWave && lastWaveNumberKnown != gameController.currentWaveLooped) {
        currentWaveIndex = -1;
        lastWaveNumberKnown = gameController.currentWaveLooped;
        for (int i = 0; i < waves.Count; i++)
        {
          if (waves[i].number == gameController.currentWaveLooped) {
            currentWaveIndex = i;
            break;
          }
        }
        if (currentWaveIndex > -1) {
          currentWaveAmount = waves[currentWaveIndex].amount;
          state = SpawnerState.Spawning;
        }
      }
      if (currentWaveIndex > -1)
      {
        float oldStartTime = startTime;
        startTime = Spawn(startTime, waves[currentWaveIndex].delay, waves[currentWaveIndex].enemy);
        if (oldStartTime != startTime)
        {
          currentWaveAmount -= 1;
        }

        if (state == SpawnerState.Spawning && currentWaveAmount <= 0)
        {
          state = SpawnerState.WaitingForDeath;
        }
      }
    }
    if (waitUntilAllDead) {
      if (state == SpawnerState.WaitingForDeath && spawnedEnemies.Count > 0) {
        bool oneAlive = false;
        for (int i = spawnedEnemies.Count-1; i > -1; i--)
        {
          if (!spawnedEnemies[i] || spawnedEnemies[i].hasDied) {
            spawnedEnemies.RemoveAt(i);
          } else {
            oneAlive = true;
          }
        }
        if (oneAlive) {
          allDead = false;
        } else {
          allDead = true;
          state = SpawnerState.WaitingForNextWave;
        }
      }
    } else {
      if (state == SpawnerState.WaitingForDeath) {
        state = SpawnerState.WaitingForNextWave;
      }
    }
  }

  SpawnPointController GetASpawnPoint() {
    for (int i = 0; i < spawnPointObjects.Count; i++) {
      SpawnPointController temp = spawnPointObjects[i];
      int randomIndex = Random.Range (i, spawnPointObjects.Count);
      spawnPointObjects[i] = spawnPointObjects[randomIndex];
      spawnPointObjects[randomIndex] = temp;
    }

    SpawnPointController spawnPoint = null;
    foreach (SpawnPointController spawnPointObject in spawnPointObjects) {
      if (spawnPointObject.used == false) {
        spawnPoint = spawnPointObject;
        break;
      }
    }
    return spawnPoint;
  }

  float Spawn(float startTime, float delay, GameObject enemyPrefab) {
    if (state == SpawnerState.Spawning && necessaryObject.activeSelf && Time.time > startTime + delay) {
      SpawnPointController spawnPoint = GetASpawnPoint();
      if (spawnPoint) {
        startTime = Time.time;
        Quaternion rotation = Quaternion.Euler (
          transform.rotation.eulerAngles.x,
          spawnPoint.transform.parent.rotation.y,
          transform.rotation.eulerAngles.z
        );
        GameObject enemy = Instantiate (enemyPrefab, spawnPoint.transform.position, rotation);
        EnemyController enemyController = enemy.GetComponent<EnemyController> ();
        enemyController.spawnPoint = spawnPoint;
        allDead = false;
        spawnedEnemies.Add(enemyController);
        gameController.haventSpawnedYet = false;
      }
    }
    return startTime;
  }
}