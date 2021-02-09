using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HealthSimple : MonoBehaviour {
  public Animator anim;
  protected UnityEngine.AI.NavMeshAgent navAgent;

  public int maxHealth = 200;
  public int currentHealth;
  public bool isDead = false;
  public GameObject headerPrefab;
  public GameObject corpsePrefab;
  protected GameObject header;
  protected MeshRenderer minimapToken;
  protected Image[] headerBars;
  public Image healthFrame;
  public Image healthBar;
  public string entityName;
  protected GameObject canvas;
  public bool spawning = false;
  protected bool isHeaderVisible = false;
  public GameObject corpse;
  public bool destroyOnDie = false;
  public int moneyToReward = 1;
  public GameObject fatalAttacker;
  public Transform body;
  public Transform headerAnchor;

  // Start is called before the first frame update
  void Start()
  {
    Init();
  }
  protected void Init()
  {
    navAgent = GetComponentInChildren<UnityEngine.AI.NavMeshAgent> ();

    canvas = GameObject.Find ("Canvas");

    if (headerPrefab)
      HeaderInstantiate ();

    DamagePopUpController.Initialize ();
    currentHealth = maxHealth;
    anim = GetComponent<Animator> ();
    if (!anim) {
      anim = GetComponent<Animator> ();
      if (!anim) {
        anim = GetComponentInChildren<Animator> ();
      }
    }

    if (!body) {
      body = transform;
    }
  }

  // Update is called once per frame
  void Update () {
    if (header) {
      Vector2 headerScreenPosition = Camera.main.WorldToScreenPoint (new Vector3 (headerAnchor.position.x, headerAnchor.position.y + 2f, headerAnchor.position.z));
      header.transform.position = new Vector2 (headerScreenPosition.x, headerScreenPosition.y + 75);
      healthBar.fillAmount = (float) currentHealth / maxHealth;
    }

    if (Input.GetKeyDown (KeyCode.V)) {
      TakeDamage (Random.Range (1, 21), this.gameObject);
    }

    if (Input.GetKeyDown (KeyCode.C)) {
      ReceiveHealing (Random.Range (1, 21));
    }
  }

  protected void HeaderInstantiate () {
    Vector2 headerScreenPosition = Camera.main.WorldToScreenPoint (new Vector3 (headerAnchor.position.x, headerAnchor.position.y + 2f, headerAnchor.position.z));
    header = Instantiate (headerPrefab);
    header.transform.SetParent (canvas.transform, false);
    header.transform.position = new Vector2 (headerScreenPosition.x, headerScreenPosition.y + 75);

    headerBars = header.GetComponentsInChildren<Image> ();

    healthFrame = headerBars[0];
    healthBar = headerBars[1];

    isHeaderVisible = true;
  }

  public virtual bool TakeDamage (int damageAmount, GameObject attacker) {
    if (!isDead) {
      DamagePopUpController.CreateDamagePopUp (damageAmount.ToString (), body, "red");

      UpdateDamage (currentHealth - damageAmount);

      if (currentHealth <= 0) {
        currentHealth = 0;
        if (!isDead)
          Die (attacker);
        return true;
      }
    }
    return false;
  }

  protected void UpdateDamage (int newHealth) {
    currentHealth = newHealth;
    if (currentHealth <= 0) {
      currentHealth = 0;
    }

    if (currentHealth >= maxHealth) {
      currentHealth = maxHealth;
    }
  }

  public void ReceiveHealing (int healingAmount) {
    if (!isDead) {
      DamagePopUpController.CreateDamagePopUp (healingAmount.ToString (), transform, "green");

      UpdateDamage (currentHealth + healingAmount);

      if (currentHealth >= maxHealth) {
        currentHealth = maxHealth;
      }
    }
  }

  public void Die (GameObject attacker) {
    if (!isDead) {
      currentHealth = 0;
      if (isHeaderVisible)
        headerToggle (false);

      isDead = true;
      fatalAttacker = attacker;
      if (destroyOnDie) {
        Destroy (gameObject);
      }
    }
  }

  void headerToggle (bool state) {
    if (healthFrame && healthBar) {
      healthFrame.enabled = state;
      healthBar.enabled = state;
    }
  }

  void OnDisable () {
    if (header)
      Destroy (header);
  }
}