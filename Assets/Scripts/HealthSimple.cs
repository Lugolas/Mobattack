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
  protected int lastMaxHealthKnown = 0;
  public int currentHealth;
  public int armorBase = 0;
  List<Tools.StatModifier> armorBaseMultipliers = new List<Tools.StatModifier>();
  List<Tools.StatModifier> armorAdditions = new List<Tools.StatModifier>();
  List<Tools.StatModifier> armorMultipliers = new List<Tools.StatModifier>();
  public int armorFinal;
  public int damageBase = 0;
  List<Tools.StatModifier> damageBaseMultipliers = new List<Tools.StatModifier>();
  List<Tools.StatModifier> damageAdditions = new List<Tools.StatModifier>();
  List<Tools.StatModifier> damageMultipliers = new List<Tools.StatModifier>();
  public int damageFinal;
  public bool healthRegen = false;
  public float healthRegenPerSecondBase = 0;
  List<Tools.StatModifier> healthRegenPerSecondBaseMultipliers = new List<Tools.StatModifier>();
  List<Tools.StatModifier> healthRegenPerSecondAdditions = new List<Tools.StatModifier>();
  List<Tools.StatModifier> healthRegenPerSecondMultipliers = new List<Tools.StatModifier>();
  public float healthRegenPerSecondFinal = 0;
  int healthRegenPerRegen = 0;
  float healthRegenLast = 0;
  float healthRegenDelay = 0;
  public bool isDead = false;
  public GameObject headerPrefab;
  protected GameObject header;
  protected Image[] headerBarImages;
  public Image healthFrame;
  public Image healthBarImage;
  protected UIBarController healthBar;
  public string entityName;
  protected GameObject canvas;
  protected bool isHeaderVisible = false;
  public GameObject corpse;
  public bool destroyOnDie = false;
  public int moneyToReward = 1;
  public SpellController fatalAttacker;
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
    lastMaxHealthKnown = maxHealth;

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
    UpdateProcess();
  }

  protected void UpdateProcess() {
    if (header) {
      Vector2 headerScreenPosition = Camera.main.WorldToScreenPoint (new Vector3 (headerAnchor.position.x, headerAnchor.position.y + 2f, headerAnchor.position.z));
      header.transform.position = new Vector2 (headerScreenPosition.x, headerScreenPosition.y + 75);
      if (healthBar) {
        if (healthBar.IsInitiated()) {
          healthBar.SetCurrentValue(currentHealth);
        } else {
          healthBar.Init(maxHealth);
        }
        if (maxHealth != lastMaxHealthKnown) {
          lastMaxHealthKnown = maxHealth;
          healthBar.UpdateBar(maxHealth);
        }
      }
    }

    if (healthRegen) {
      if (currentHealth < maxHealth && Time.time > healthRegenLast + healthRegenDelay) {
        ReceiveHealing(healthRegenPerRegen, false);
        healthRegenLast = Time.time;
      }
    }

    if (Input.GetKeyDown (KeyCode.V)) {
      TakeDamage (Random.Range (1, 21));
    }

    if (Input.GetKeyDown (KeyCode.C)) {
      ReceiveHealing (Random.Range (1, 21), true);
    }
  }

  protected void HeaderInstantiate () {
    Vector2 headerScreenPosition = Camera.main.WorldToScreenPoint (new Vector3 (headerAnchor.position.x, headerAnchor.position.y + 2f, headerAnchor.position.z));
    header = Instantiate (headerPrefab);
    header.transform.SetParent (canvas.transform, false);
    header.transform.position = new Vector2 (headerScreenPosition.x, headerScreenPosition.y + 75);

    headerBarImages = header.GetComponentsInChildren<Image> ();

    healthFrame = headerBarImages[0];
    healthBar = header.GetComponentInChildren<UIBarController>();
    if (healthBar) {
      healthBarImage = healthBar.valueBar;
    }

    isHeaderVisible = true;
  }

  public virtual bool TakeDamage (int damageAmount, SpellController attacker = null) {
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

  public void ReceiveHealing (int healingAmount, bool showText) {
    if (!isDead) {
      if (showText)
        DamagePopUpController.CreateDamagePopUp (healingAmount.ToString (), body, "green");

      UpdateDamage (currentHealth + healingAmount);

      if (currentHealth >= maxHealth) {
        currentHealth = maxHealth;
      }
    }
  }

  public void Die (SpellController attacker) {
    if (!isDead) {
      currentHealth = 0;
      if (isHeaderVisible)
        this.headerToggle(false);

      isDead = true;
      fatalAttacker = attacker;
      if (destroyOnDie) {
        Destroy (gameObject);
      }
    }
  }

  protected void headerToggle (bool state) {
    if (healthFrame && healthBar) {
      healthFrame.enabled = state;
      healthBar.enabled = state;
    }
  }

  void OnDisable () {
    if (header)
      Destroy (header);
  }

  void UpdateHealthRegenPerSecond() {
    float healthRegenPerSecondTemp = healthRegenPerSecondBase;
    foreach (Tools.StatModifier healthRegenPerSecondBaseMultiplier in healthRegenPerSecondBaseMultipliers)
    {
      healthRegenPerSecondTemp *= healthRegenPerSecondBaseMultiplier.value;
    }
    foreach (Tools.StatModifier healthRegenPerSecondAddition in healthRegenPerSecondAdditions)
    {
      healthRegenPerSecondTemp += healthRegenPerSecondAddition.value;
    }
    foreach (Tools.StatModifier healthRegenPerSecondMultiplier in healthRegenPerSecondMultipliers)
    {
      healthRegenPerSecondTemp *= healthRegenPerSecondMultiplier.value;
    }
    healthRegenPerSecondFinal = healthRegenPerSecondTemp;

    healthRegenPerRegen = Mathf.CeilToInt(healthRegenPerSecondFinal / 2);
    healthRegenDelay = healthRegenPerRegen / healthRegenPerSecondFinal;
  }
  void UpdateArmor() {
    float armorTemp = armorBase;
    foreach (Tools.StatModifier armorBaseMultiplier in armorBaseMultipliers)
    {
      armorTemp *= armorBaseMultiplier.value;
    }
    foreach (Tools.StatModifier armorAddition in armorAdditions)
    {
      armorTemp += armorAddition.value;
    }
    foreach (Tools.StatModifier armorMultiplier in armorMultipliers)
    {
      armorTemp *= armorMultiplier.value;
    }
    armorFinal = Mathf.RoundToInt(armorTemp);
  }
  void UpdateDamage() {
    float damageTemp = damageBase;
    foreach (Tools.StatModifier damageBaseMultiplier in damageBaseMultipliers)
    {
      damageTemp *= damageBaseMultiplier.value;
    }
    foreach (Tools.StatModifier damageAddition in damageAdditions)
    {
      damageTemp += damageAddition.value;
    }
    foreach (Tools.StatModifier damageMultiplier in damageMultipliers)
    {
      damageTemp *= damageMultiplier.value;
    }
    damageFinal = Mathf.RoundToInt(damageTemp);
  }

  public void AddHealthRegenPerSecondBaseMultiplier(float value, string identifier) {
    if (healthRegen) {
      if (Tools.AddStatModifier(healthRegenPerSecondBaseMultipliers, value, identifier)) {
        UpdateHealthRegenPerSecond();
      }
    }
  }
  public void RemoveHealthRegenPerSecondBaseMultiplier(string identifier) {
    if (healthRegen) {
      if (Tools.RemoveStatModifier(healthRegenPerSecondBaseMultipliers, identifier)) {
        UpdateHealthRegenPerSecond();
      }
    }
  }
  public void AddHealthRegenPerSecondAddition(float value, string identifier) {
    if (healthRegen) {
      if (Tools.AddStatModifier(healthRegenPerSecondAdditions, value, identifier)) {
        UpdateHealthRegenPerSecond();
      }
    }
  }
  public void RemoveHealthRegenPerSecondAddition(string identifier) {
    if (healthRegen) {
      if (Tools.RemoveStatModifier(healthRegenPerSecondAdditions, identifier)) {
        UpdateHealthRegenPerSecond();
      }
    }
  }
  public void AddHealthRegenPerSecondMultiplier(float value, string identifier) {
    if (healthRegen) {
      if (Tools.AddStatModifier(healthRegenPerSecondMultipliers, value, identifier)) {
        UpdateHealthRegenPerSecond();
      }
    }
  }
  public void RemoveHealthRegenPerSecondMultiplier(string identifier) {
    if (healthRegen) {
      if (Tools.RemoveStatModifier(healthRegenPerSecondMultipliers, identifier)) {
        UpdateHealthRegenPerSecond();
      }
    }
  }

  public void AddArmorBaseMultiplier(float value, string identifier) {
    if (Tools.AddStatModifier(armorBaseMultipliers, value, identifier)) {
      UpdateArmor();
    }
  }
  public void RemoveArmorBaseMultiplier(string identifier) {
    if (Tools.RemoveStatModifier(armorBaseMultipliers, identifier)) {
      UpdateArmor();
    }
  }
  public void AddArmorAddition(int value, string identifier) {
    if (Tools.AddStatModifier(armorAdditions, value, identifier)) {
      UpdateArmor();
    }
  }
  public void RemoveArmorAddition(string identifier) {
    if (Tools.RemoveStatModifier(armorAdditions, identifier)) {
      UpdateArmor();
    }
  }
  public void AddArmorMultiplier(float value, string identifier) {
    if (Tools.AddStatModifier(armorMultipliers, value, identifier)) {
      UpdateArmor();
    }
  }
  public void RemoveArmorMultiplier(string identifier) {
    if (Tools.RemoveStatModifier(armorMultipliers, identifier)) {
      UpdateArmor();
    }
  }

  public void AddDamageBaseMultiplier(float value, string identifier) {
    if (Tools.AddStatModifier(damageBaseMultipliers, value, identifier)) {
      UpdateDamage();
    }
  }
  public void RemoveDamageBaseMultiplier(string identifier) {
    if (Tools.RemoveStatModifier(damageBaseMultipliers, identifier)) {
      UpdateDamage();
    }
  }
  public void AddDamageAddition(int value, string identifier) {
    if (Tools.AddStatModifier(damageAdditions, value, identifier)) {
      UpdateDamage();
    }
  }
  public void RemoveDamageAddition(string identifier) {
    if (Tools.RemoveStatModifier(damageAdditions, identifier)) {
      UpdateDamage();
    }
  }
  public void AddDamageMultiplier(float value, string identifier) {
    if (Tools.AddStatModifier(damageMultipliers, value, identifier)) {
      UpdateDamage();
    }
  }
  public void RemoveDamageMultiplier(string identifier) {
    if (Tools.RemoveStatModifier(damageMultipliers, identifier)) {
      UpdateDamage();
    }
  }
}