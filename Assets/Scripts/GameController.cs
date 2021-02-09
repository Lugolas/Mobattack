using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class GameController : MonoBehaviour
{
  public bool waitUntilAllDead = true;
  public bool onlyOne = false;
  public float timeBetweenWaves = 10;
  public List<GameObject> necessaryObjects = new List<GameObject>();
  bool allNecessaryObjectsActive = false;
  public int currentWave = 0;
  public int currentWaveLooped = 0;
  public bool haventSpawnedYet = true;
  public bool forbiddenSpawn = true;
  public float untilNextWave;
  float lastWaveEndTime;
  public bool readyForNextWave = true;
  int wavesAmount = -1;
  public List<SpawnerController> spawners = new List<SpawnerController>();
  bool currentWaveHasBegun = false;
  float currentWaveStartTime;
  float minimumWaveDuration = 2;
  bool waveInProgress = false;

  void Start()
  {
    foreach (SpawnerController spawner in spawners)
    {
      foreach (SpawnerController.Wave wave in spawner.waves)
      {
        if (wave.number > wavesAmount) {
          wavesAmount = wave.number;
        }
      }
    }
  }

  void Update()
  {
    if (!allNecessaryObjectsActive) {
      bool oneInactive = false;
      foreach (GameObject necessaryObject in necessaryObjects)
      {
        if (!necessaryObject.activeSelf) {
          oneInactive = true;
        }
      }
      if (!oneInactive) {
        allNecessaryObjectsActive = true;
        lastWaveEndTime = Time.time;
      }
    } else {
      if (onlyOne) {
        if (!haventSpawnedYet) {
          forbiddenSpawn = true;
        }
      } else {
        untilNextWave = timeBetweenWaves - (Time.time - lastWaveEndTime);

        if ((Time.time >= lastWaveEndTime + timeBetweenWaves) && !waveInProgress) {
          Debug.Log("Start wave " + currentWave + " (" + currentWaveLooped + ") ");
          forbiddenSpawn = false;
          currentWaveStartTime = Time.time;
          waveInProgress = true;
        }

        bool allWaiting = true;
        foreach (SpawnerController spawner in spawners)
        {
          if (spawner.state != SpawnerController.SpawnerState.WaitingForNextWave) {
            allWaiting = false;
          }
        }

        if (allWaiting) {
          readyForNextWave = true;
        } else {
          readyForNextWave = false;
        }

        if (Time.time > currentWaveStartTime + minimumWaveDuration && readyForNextWave && waveInProgress) {
          Debug.Log("End wave " + currentWave + " (" + currentWaveLooped + ") ");
          waveInProgress = false;
          readyForNextWave = false;
          forbiddenSpawn = true;
          currentWave++;
          currentWaveHasBegun = false;
          if (currentWave > wavesAmount) {
            currentWaveLooped = currentWave % wavesAmount;
            if (currentWaveLooped == 0) {
              currentWaveLooped = wavesAmount;
            }
          } else {
            currentWaveLooped = currentWave;
          }
          lastWaveEndTime = Time.time;
        }
      }
    }
  }
}
