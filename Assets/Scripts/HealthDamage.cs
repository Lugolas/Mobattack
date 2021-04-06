using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using Cinemachine;

public class HealthDamage : HealthSimple
{
  private float respawnTime = 0;
  private float deathTime = -1;
  private BaseMoveAttacc baseMoveAttacc;
  private Image manaBar;
  private TextMeshProUGUI headerText;
  private int SPAWNING_LAYER = 9;
  private int CHARACTER_LAYER = 10;
  private int CHARACTER_DEAD_LAYER = 15;
  private CharacterManager characterManager;
  private CinemachineVirtualCamera virtualCamera;
  private CinemachineVirtualCamera verticalCamera;
  bool hasDied = false;



  // Start is called before the first frame update
  void Start()
  {
    Init();
    characterManager = GetComponent<CharacterManager>();

    virtualCamera = GameObject.Find("CameraInitiale").GetComponent<CinemachineVirtualCamera>();
    verticalCamera = GameObject.Find("CameraVerticale").GetComponent<CinemachineVirtualCamera>();

    baseMoveAttacc = GetComponent<BaseMoveAttacc>();

    playerHeaderInstantiate();
  }

  void Update() {
    UpdateProcess();
    if (isDead && !hasDied) {
      hasDied = true;
      headerEnhancedToggle(false);
    }
    if (!isDead && hasDied) {
      hasDied = false;
      headerEnhancedToggle(true);
      currentHealth = maxHealth;
    }
  }

  void playerHeaderInstantiate()
  {
    manaBar = headerBars[2];

    headerText = header.GetComponentInChildren<TextMeshProUGUI>();
    if (headerText) {
      headerText.text = entityName;
    }
  }

  protected void headerEnhancedToggle (bool state) {
    headerToggle(state);
    if (healthFrame && healthBar) {
      manaBar.enabled = state;
      headerText.enabled = state;
    }
  }
}