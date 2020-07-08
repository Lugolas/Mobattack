﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.Networking;

public class HealthDamage : NetworkBehaviour
{
  private Animator anim;
  private UnityEngine.AI.NavMeshAgent navAgent;

  public int maxHealth = 200;
  private float respawnTime = 10;
  [SyncVar]
  private float deathTime = -1;
  [SyncVar]
  public int currentHealth;
  [SyncVar]
  public bool isDead = true;
  private BaseMoveAttacc baseMoveAttacc;
  public Transform waitingForRespawnPoint;
  public Transform RespawnPoint;
  public GameObject playerHeaderPrefab;
  public GameObject playerCorpsePrefab;
  private GameObject playerHeader;
  private Image[] playerHeaderBars;
  private Image playerHealthBar;
  private Image playerManaBar;
  private TextMeshProUGUI playerHeaderText;
  public string playerName;
  private GameObject canvas;
  public bool spawning = false;
  public string playerNumber = "";

  // Start is called before the first frame update
  void Start()
  {
    navAgent = GetComponentInChildren<UnityEngine.AI.NavMeshAgent>();

    if (!waitingForRespawnPoint)
    {
      GameObject spawnPoint = GameObject.Find("waitingForRespawnPoint");
      if (spawnPoint)
      {
        waitingForRespawnPoint = spawnPoint.transform;
      }
    }
    if (!RespawnPoint)
    {
      GameObject spawns = GameObject.Find("Spawns");
      if (spawns)
      {
        RespawnPoint = spawns.transform.Find("RespawnPoint" + playerNumber);
      }
    }

    baseMoveAttacc = GetComponent<BaseMoveAttacc>();
    canvas = GameObject.Find("Canvas");

    playerHeaderInstantiate();

    DamagePopUpController.Initialize();
    currentHealth = maxHealth;
    anim = GetComponent<Animator>();
    if (!anim)
    {
      anim = GetComponentInChildren<Animator>();
    }
  }

  // Update is called once per frame
  void Update()
  {
    if (isDead && deathTime != -1 && Time.time >= deathTime + respawnTime)
    {
      Spawn();
      respawnTime = 10;
    }
    if (playerHeader)
    {
      Vector2 playerHeaderScreenPosition = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z));
      playerHeader.transform.position = new Vector2(playerHeaderScreenPosition.x, playerHeaderScreenPosition.y + 75);
      playerHealthBar.fillAmount = (float)currentHealth / maxHealth;
    }

    if (Input.GetKeyDown(KeyCode.V))
    {
      TakeDamage(Random.Range(1, 200));
    }

    if (Input.GetKeyDown(KeyCode.C))
    {
      ReceiveHealing(Random.Range(1, 200));
    }
    if (this.anim.GetCurrentAnimatorStateInfo(0).IsName("Spawn"))
    {
      spawning = true;
    }
    else if (spawning && isDead)
    {
      spawning = false;
      CmdUpdateDeathStatus(false);
    }
  }

  [Command]
  void CmdUpdateDeathStatus(bool deathStatus)
  {
    RpcUpdateDeathStatus(deathStatus);
  }
  [ClientRpc]
  void RpcUpdateDeathStatus(bool deathStatus)
  {
    isDead = deathStatus;
  }

  void playerHeaderInstantiate()
  {
    Vector2 playerHeaderScreenPosition = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z));
    playerHeader = Instantiate(playerHeaderPrefab);
    playerHeader.transform.SetParent(canvas.transform, false);
    playerHeader.transform.position = new Vector2(playerHeaderScreenPosition.x, playerHeaderScreenPosition.y + 75);

    playerHeaderBars = playerHeader.GetComponentsInChildren<Image>();
    if (playerHeaderBars[0].name == "HealthBar")
    {
      playerHealthBar = playerHeaderBars[0];
      playerManaBar = playerHeaderBars[1];
    }
    else
    {
      playerHealthBar = playerHeaderBars[1];
      playerManaBar = playerHeaderBars[0];
    }

    playerHeaderText = playerHeader.GetComponentsInChildren<TextMeshProUGUI>()[0];
    playerHeaderText.text = playerName;
  }

  public void TakeDamage(int damageAmount)
  {
    if (!isDead)
    {
      DamagePopUpController.CreateDamagePopUp(damageAmount.ToString(), transform, "red");
      // currentHealth -= damageAmount;

      if (isServer)
      {
        CmdTakeDamage(damageAmount);
      }

      if (currentHealth <= 0)
      {
        currentHealth = 0;
        if (!isDead)
          Die();
      }
    }
  }

  [Command]
  void CmdTakeDamage(int damageAmount)
  {
    RpcUpdateDamage(currentHealth - damageAmount);
  }

  [ClientRpc]
  void RpcUpdateDamage(int newHealth)
  {
    currentHealth = newHealth;
    if (currentHealth <= 0)
    {
      currentHealth = 0;
      if (!isDead)
        Die();
    }
  }

  void Spawn()
  {
    // transform.position = RespawnPoint.position;
    navAgent.Warp(RespawnPoint.position);
    anim.SetTrigger("Spawn");
    deathTime = -1;
    currentHealth = maxHealth;
    playerHeaderInstantiate();
  }
  void ReceiveHealing(int healingAmount)
  {
    if (!isDead)
    {
      DamagePopUpController.CreateDamagePopUp(healingAmount.ToString(), transform, "green");
      currentHealth += healingAmount;

      if (currentHealth >= maxHealth)
      {
        currentHealth = maxHealth;
      }
    }
  }

  void Die()
  {
    CmdDie();
    // currentHealth = 0;
    // if (playerHeader)
    //   Destroy(playerHeader);
    // GameObject playerCorpse = Instantiate(playerCorpsePrefab);
    // playerCorpse.transform.position = transform.position - new Vector3(0, 0.2f, 0);
    // playerCorpse.transform.rotation = transform.rotation;
    // Destroy(playerCorpse, 60);
    // CmdUpdateDeathStatus(true);
    // deathTime = Time.time;
    // // anim.SetTrigger("Die");
    // if (baseMoveAttacc)
    // {
    //   baseMoveAttacc.stopMoving();
    // }
    // if (waitingForRespawnPoint)
    // {
    //   navAgent.Warp(waitingForRespawnPoint.position);
    //   transform.rotation = waitingForRespawnPoint.rotation;
    // }
  }
  [Command]
  void CmdDie()
  {
    RpcDie();
  }
  [ClientRpc]
  void RpcDie()
  {
    currentHealth = 0;
    if (playerHeader)
      Destroy(playerHeader);
    GameObject playerCorpse = Instantiate(playerCorpsePrefab);
    playerCorpse.transform.position = transform.position - new Vector3(0, 0.2f, 0);
    playerCorpse.transform.rotation = transform.rotation;
    Destroy(playerCorpse, 60);
    CmdUpdateDeathStatus(true);
    deathTime = Time.time;
    // anim.SetTrigger("Die");
    if (baseMoveAttacc)
    {
      baseMoveAttacc.stopMoving();
    }
    if (waitingForRespawnPoint)
    {
      navAgent.Warp(waitingForRespawnPoint.position);
      transform.rotation = waitingForRespawnPoint.rotation;
    }
  }
  void OnDisable()
  {
    if (playerHeader)
      Destroy(playerHeader);
  }

}