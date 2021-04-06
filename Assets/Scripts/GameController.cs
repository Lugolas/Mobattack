using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
  public bool waitUntilAllDead = true;
  public bool onlyOne = false;
  public float timeBetweenWaves = 10;
  public List<GameObject> necessaryObjects = new List<GameObject> ();
  bool allNecessaryObjectsActive = false;
  public int currentWave = 0;
  public TMP_Text waveText;
  public Image timerImage;
  public TMP_Text timerText;
  public int currentWaveLooped = 0;
  public bool haventSpawnedYet = true;
  public bool forbiddenSpawn = true;
  float untilFirstWave;
  public float untilNextWave;
  float lastWaveEndTime;
  float beginningTime;
  public bool readyForNextWave = true;
  int wavesAmount = -1;
  public List<SpawnerController> spawners = new List<SpawnerController> ();
  bool currentWaveHasBegun = false;
  float currentWaveStartTime;
  float minimumWaveDuration = 2;
  bool waveInProgress = false;

  void Start () {
    foreach (SpawnerController spawner in spawners) {
      foreach (SpawnerController.Wave wave in spawner.waves) {
        if (wave.number > wavesAmount) {
          wavesAmount = wave.number;
        }
      }
    }
    UpdateWaveText ();
  }

  void UpdateWaveText () {
    if (currentWave != 0) {
      waveText.text = currentWave.ToString ();
    } else {
      waveText.text = "1";
    }
  }

  void UpdateTimer () {
    if (currentWave <= 1 && allNecessaryObjectsActive) {
      untilFirstWave = 22 - (Time.time - beginningTime);
      if (currentWave == 1 && untilFirstWave < 0) {
        timerText.text = "";
        timerImage.enabled = false;
      } else {
        timerText.text = Mathf.CeilToInt (untilFirstWave).ToString ();
        timerImage.enabled = true;
      }
    } else {
      if (untilNextWave > 0) {
        timerText.text = Mathf.CeilToInt (untilNextWave).ToString ();
        timerImage.enabled = true;
      } else {
        timerText.text = "";
        timerImage.enabled = false;
      }
    }
  }

  void Update () {
    if (!allNecessaryObjectsActive) {
      bool oneInactive = false;
      foreach (GameObject necessaryObject in necessaryObjects) {
        if (!necessaryObject.activeSelf) {
          oneInactive = true;
        }
      }
      if (!oneInactive) {
        allNecessaryObjectsActive = true;
        lastWaveEndTime = Time.time;
        beginningTime = Time.time;
      }
    } else {
      if (onlyOne) {
        if (!haventSpawnedYet) {
          forbiddenSpawn = true;
        }
      } else {
        untilNextWave = timeBetweenWaves - (Time.time - lastWaveEndTime);

        if ((Time.time >= lastWaveEndTime + timeBetweenWaves) && !waveInProgress) {
          Debug.Log ("Start wave " + currentWave + " (" + currentWaveLooped + ") ");
          forbiddenSpawn = false;
          currentWaveStartTime = Time.time;
          waveInProgress = true;
        }

        bool allWaiting = true;
        foreach (SpawnerController spawner in spawners) {
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
          Debug.Log ("End wave " + currentWave + " (" + currentWaveLooped + ") ");
          waveInProgress = false;
          readyForNextWave = false;
          forbiddenSpawn = true;
          currentWave++;
          UpdateWaveText ();
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
    UpdateTimer ();
  }
}