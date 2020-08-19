using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.Networking;
using Cinemachine;

public class HealthDamage : NetworkBehaviour
{
  private Animator anim;
  private UnityEngine.AI.NavMeshAgent navAgent;

  public int maxHealth = 200;
  private float respawnTime = 0;
  [SyncVar]
  private float deathTime = -1;
  [SyncVar]
  public int currentHealth;
  [SyncVar]
  public bool isDead = true;
  private BaseMoveAttacc baseMoveAttacc;
  public Transform waitingForRespawnPoint;
  public Transform respawnPoint;
  public GameObject playerHeaderPrefab;
  public GameObject playerCorpsePrefab;
  private GameObject playerHeader;
  private MeshRenderer tokenPointer;
  private MeshRenderer minimapToken;
  private Image[] playerHeaderBars;
  public Image playerHealthManaFrame;
  public Image playerHealthBar;
  private Image playerManaBar;
  private TextMeshProUGUI playerHeaderText;
  [SyncVar]
  public string playerName;
  private GameObject canvas;
  public bool spawning = false;
  public string playerNumber = "";
  private bool isPlayerHeaderVisible = false;
  private int SPAWNING_LAYER = 9;
  private int CHARACTER_LAYER = 10;
  private int CHARACTER_DEAD_LAYER = 15;
  private CharacterManager characterManager;
  private CinemachineVirtualCamera virtualCamera;
  public GameObject playerCorpse;


  // Start is called before the first frame update
  void Start()
  {
    GameObject charactersManager = GameObject.Find("CharactersManager");
    if (charactersManager)
    {
      HealthDamage[] characters = charactersManager.GetComponentsInChildren<HealthDamage>();
      foreach (HealthDamage character in characters)
      {
        if (character.currentHealth > 0)
        {
          character.isDead = false;
        }
      }
    }

    tokenPointer = GetComponentInChildren<TokenPointer>().GetComponent<MeshRenderer>();
    minimapToken = GetComponentInChildren<MinimapToken>().GetComponent<MeshRenderer>();

    navAgent = GetComponentInChildren<UnityEngine.AI.NavMeshAgent>();

    characterManager = GetComponent<CharacterManager>();

    virtualCamera = GameObject.Find("VirtualCamera").GetComponent<CinemachineVirtualCamera>();

    isDead = true;
    deathTime = Time.time;
    respawnTime = 0;

    if (!waitingForRespawnPoint)
    {
      GameObject spawnPoint = GameObject.Find("waitingForRespawnPoint");
      if (spawnPoint)
      {
        waitingForRespawnPoint = spawnPoint.transform;
      }
    }
    if (!respawnPoint)
    {
      GameObject spawns = GameObject.Find("Spawns");
      if (spawns)
      {
        Transform teamSpawn = spawns.transform.Find("Team" + characterManager.team);

        RespawnPointManager[] teamRespawnPoints = teamSpawn.GetComponentsInChildren<RespawnPointManager>();
        foreach (RespawnPointManager teamRespawnPoint in teamRespawnPoints)
        {
          if (teamRespawnPoint.characterName == "")
          {
            teamRespawnPoint.characterName = gameObject.name;
            respawnPoint = teamRespawnPoint.transform;
            break;
          }
        }
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
    // if (isDead && gameObject.layer == CHARACTER_LAYER)
    // {
    //   Tools.SetLayerRecursively(gameObject, CHARACTER_DEAD_LAYER);
    // }
    // else if (!isDead && gameObject.layer == CHARACTER_DEAD_LAYER)
    // {
    //   Tools.SetLayerRecursively(gameObject, CHARACTER_LAYER);
    // }
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
    else
    {
      // playerHeaderInstantiate();
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
      if (isServer)
      {
        CmdUpdateDeathStatus(false);
      }
      Tools.SetLayerRecursively(gameObject, CHARACTER_LAYER);
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
    // Debug.Log("Trying to Instantiate PlayerHeader");
    // if (isServer)
    // {
    //   Debug.Log("About to really do it because I am the host :)");
    //   CmdPlayerHeaderInstantiate();
    // }
    // else
    //   Debug.Log("Can't quite manage because I am a lowly client :(");
    Vector2 playerHeaderScreenPosition = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z));
    playerHeader = Instantiate(playerHeaderPrefab);
    playerHeader.transform.SetParent(canvas.transform, false);
    playerHeader.transform.position = new Vector2(playerHeaderScreenPosition.x, playerHeaderScreenPosition.y + 75);

    playerHeaderBars = playerHeader.GetComponentsInChildren<Image>();

    playerHealthManaFrame = playerHeaderBars[0];
    playerHealthBar = playerHeaderBars[1];
    playerManaBar = playerHeaderBars[2];

    playerHeaderText = playerHeader.GetComponentInChildren<TextMeshProUGUI>();
    playerHeaderText.text = playerName;
    isPlayerHeaderVisible = true;
    // characterManager.assignTeam();
  }

  [Command]
  void CmdPlayerHeaderInstantiate()
  {
    RpcPlayerHeaderInstantiate();
  }
  [ClientRpc]
  void RpcPlayerHeaderInstantiate()
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
    isPlayerHeaderVisible = true;
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
    if (isServer)
    {
      CmdSpawn();
    }
  }
  [Command]
  void CmdSpawn()
  {
    RpcSpawn();
    RpcPlayerHeaderToggle(true);
  }
  [ClientRpc]
  void RpcSpawn()
  {
    if (respawnPoint)
    {
      navAgent.Warp(respawnPoint.position);
      transform.rotation = respawnPoint.rotation;
      if (characterManager.player)
        characterManager.player.GetComponent<PlayerInputManager>().cameraOnCharacter();
      anim.SetTrigger("Spawn");
      deathTime = -1;
      Tools.SetLayerRecursively(gameObject, SPAWNING_LAYER);
      currentHealth = maxHealth;
    }
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

    if (isServer)
    {
      CmdDie();
      CmdUpdateDeathStatus(true);
    }
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
    Tools.SetLayerRecursively(gameObject, CHARACTER_DEAD_LAYER);
    currentHealth = 0;
    if (isPlayerHeaderVisible)
      playerHeaderToggle(false);
    playerCorpse = Instantiate(playerCorpsePrefab);
    playerCorpse.transform.position = transform.position - new Vector3(0, 0.2f, 0);
    playerCorpse.transform.rotation = transform.rotation;
    if (characterManager.player)
      characterManager.player.GetComponent<PlayerInputManager>().cameraOnCorpse();
    Destroy(playerCorpse, 60);
    isDead = true;
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

  void playerHeaderToggle(bool state)
  {
    if (playerHealthManaFrame && playerHealthBar && playerManaBar && playerHeaderText && tokenPointer && minimapToken)
    {
      playerHealthManaFrame.enabled = state;
      playerHealthBar.enabled = state;
      playerManaBar.enabled = state;
      playerHeaderText.enabled = state;
      tokenPointer.enabled = state;
      minimapToken.enabled = state;
    }
    // characterManager.assignTeam();
  }

  [ClientRpc]
  void RpcPlayerHeaderToggle(bool state)
  {
    playerHeaderToggle(state);
  }

  void OnDisable()
  {
    if (playerHeader)
      Destroy(playerHeader);
  }

}