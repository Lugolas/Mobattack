﻿using System.Collections;
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
  private Image manaBarImage;
  private TextMeshProUGUI headerText;
  public int currentMana = 0;
  public int maxMana = 100;
  public bool isManaDecreasingOnItsOwn = false;
  float lastManaUpdate = 0;
  float manaDecreasingDelay = 2;
  private int SPAWNING_LAYER = 9;
  private int CHARACTER_LAYER = 10;
  private int CHARACTER_DEAD_LAYER = 15;
  private CharacterManager characterManager;
  private CinemachineVirtualCamera virtualCamera;
  private CinemachineVirtualCamera verticalCamera;
  bool hasDied = false;
  UIBarController manaBar;
  int lastMaxManaKnown;
  public int currentExp = 0;
  public int maxExp = 100;
  float lastManaDecrease = -1;
  public float manaLostPerSecond = 10;
  float manaToDecrease = 0;

  // Start is called before the first frame update
  void Start()
  {
    Init();
    characterManager = GetComponent<CharacterManager>();

    virtualCamera = GameObject.Find("CameraInitiale").GetComponent<CinemachineVirtualCamera>();
    verticalCamera = GameObject.Find("CameraVerticale").GetComponent<CinemachineVirtualCamera>();

    baseMoveAttacc = GetComponent<BaseMoveAttacc>();

    playerHeaderInstantiate();
    lastMaxManaKnown = maxMana;
  }

  void Update() {
    UpdateProcess();
    if (healthRegen) {
      AddHealthRegenPerSecondAddition(maxHealthFinal * 0.005f, "0.5%NaturalRegen");
    }
    if (header) {
      // currentExp = currentMana;
    }
    if (isDead && !hasDied) {
      hasDied = true;
      headerEnhancedToggle(false);
    }
    if (!isDead && hasDied) {
      hasDied = false;
      headerEnhancedToggle(true);
      currentHealth = maxHealthFinal;
      currentMana = 0;
    }

    if (manaBar) {
      if (manaBar.IsInitiated()) {
        manaBar.SetCurrentValue(currentMana);
      } else {
        manaBar.Init(maxMana);
      }
      if (maxMana != lastMaxManaKnown) {
        lastMaxManaKnown = maxMana;
        manaBar.UpdateBar(maxMana);
      }
    }
  }

  void FixedUpdate()
  {
    if (isManaDecreasingOnItsOwn && currentMana > 0 && Time.time >= lastManaUpdate + manaDecreasingDelay) {
      if (lastManaDecrease != -1) {
        float timeSinceLastDecrease = Time.time - lastManaDecrease;
        float manaToDecreaseSinceLastDecrease = timeSinceLastDecrease * manaLostPerSecond;
        manaToDecrease += manaToDecreaseSinceLastDecrease;
        int manaDecreasing = Mathf.FloorToInt(manaToDecrease);
        if (manaDecreasing > 0) {
          manaToDecrease -= manaDecreasing;
          UpdateMana(Mathf.RoundToInt(currentMana - manaDecreasing));
        }
      }
      lastManaDecrease = Time.time;
    } else if (lastManaDecrease != -1) {
      lastManaDecrease = -1;
    }
  }

  void playerHeaderInstantiate()
  {
    manaBar = header.GetComponentsInChildren<UIBarController>()[1];
    manaBarImage = manaBar.valueBar;

    headerText = header.GetComponentInChildren<TextMeshProUGUI>();
    if (headerText) {
      headerText.text = entityName;
    }
  }

  protected void headerEnhancedToggle (bool state) {
    headerToggle(state);
    if (healthFrame && healthBar) {
      manaBarImage.enabled = state;
      headerText.enabled = state;
    }
  }

  protected void UpdateMana (int newMana) {
    currentMana = newMana;
    if (currentMana <= 0) {
      currentMana = 0;
    }

    if (currentMana >= maxMana) {
      currentMana = maxMana;
    }
  }

  public void ReceiveMana (int manaAmount) {
    if (!isDead) {
      // DamagePopUpController.CreateDamagePopUp (manaAmount.ToString (), transform, "blue");

      UpdateMana(currentMana + manaAmount);
      lastManaUpdate = Time.time;

      if (currentMana >= maxMana) {
        currentMana = maxMana;
      }
    }
  }
}