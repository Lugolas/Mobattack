using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.Networking;
using Cinemachine;

public class HealthSimple : NetworkBehaviour
{
  private Animator anim;
  private UnityEngine.AI.NavMeshAgent navAgent;

  public int maxHealth = 200;
  [SyncVar]
  public int currentHealth;
  [SyncVar]
  public bool isDead = false;
  public GameObject headerPrefab;
  public GameObject corpsePrefab;
  private GameObject header;
  private MeshRenderer minimapToken;
  private Image[] headerBars;
  public Image healthFrame;
  public Image healthBar;
  public string entityName;
  private GameObject canvas;
  public bool spawning = false;
  private bool isHeaderVisible = false;
  public GameObject corpse;
  public bool destroyOnDie = false;
  public int moneyToReward = 1;


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

    navAgent = GetComponentInChildren<UnityEngine.AI.NavMeshAgent>();

    canvas = GameObject.Find("Canvas");

    if (headerPrefab)
      headerInstantiate();

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
    if (header)
    {
      Vector2 headerScreenPosition = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z));
      header.transform.position = new Vector2(headerScreenPosition.x, headerScreenPosition.y + 75);
      healthBar.fillAmount = (float)currentHealth / maxHealth;
    }

    if (Input.GetKeyDown(KeyCode.V))
    {
      TakeDamage(Random.Range(1, 200));
    }

    if (Input.GetKeyDown(KeyCode.C))
    {
      ReceiveHealing(Random.Range(1, 200));
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

  void headerInstantiate()
  {
    Vector2 headerScreenPosition = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z));
    header = Instantiate(headerPrefab);
    header.transform.SetParent(canvas.transform, false);
    header.transform.position = new Vector2(headerScreenPosition.x, headerScreenPosition.y + 75);

    headerBars = header.GetComponentsInChildren<Image>();

    healthFrame = headerBars[0];
    healthBar = headerBars[1];

    isHeaderVisible = true;
  }

  public bool TakeDamage(int damageAmount)
  {
    if (!isDead)
    {
      DamagePopUpController.CreateDamagePopUp(damageAmount.ToString(), transform, "red");

      UpdateDamage(currentHealth - damageAmount);

      if (currentHealth <= 0)
      {
        currentHealth = 0;
        if (!isDead)
          Die();
        return true;
      }
    }
    return false;
  }

  void UpdateDamage(int newHealth)
  {
    currentHealth = newHealth;
    if (currentHealth <= 0)
    {
      currentHealth = 0;
      if (!isDead)
        Die();
    }

    if (currentHealth >= maxHealth)
    {
      currentHealth = maxHealth;
    }
  }

  public void ReceiveHealing(int healingAmount)
  {
    if (!isDead)
    {
      DamagePopUpController.CreateDamagePopUp(healingAmount.ToString(), transform, "green");

      UpdateDamage(currentHealth + healingAmount);

      if (currentHealth >= maxHealth)
      {
        currentHealth = maxHealth;
      }
    }
  }

  void Die()
  {
    if (!isDead)
    {
      currentHealth = 0;
      if (isHeaderVisible)
        headerToggle(false);

      isDead = true;
      if (destroyOnDie)
      {
        Destroy(gameObject);
      }
    }
  }

  void headerToggle(bool state)
  {
    if (healthFrame && healthBar)
    {
      healthFrame.enabled = state;
      healthBar.enabled = state;
    }
  }

  void OnDisable()
  {
    if (header)
      Destroy(header);
  }
}