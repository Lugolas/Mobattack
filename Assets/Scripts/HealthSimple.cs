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
  public int armorBase = 0;
  List<StatModifier> armorBaseMultipliers = new List<StatModifier>();
  List<StatModifier> armorAdditions = new List<StatModifier>();
  List<StatModifier> armorMultipliers = new List<StatModifier>();
  public int armorFinal;
  public int damageBase = 0;
  List<StatModifier> damageBaseMultipliers = new List<StatModifier>();
  List<StatModifier> damageAdditions = new List<StatModifier>();
  List<StatModifier> damageMultipliers = new List<StatModifier>();
  public int damageFinal;
  public bool healthRegen = false;
  public float healthRegenPerSecondBase = 0;
  List<StatModifier> healthRegenPerSecondBaseMultipliers = new List<StatModifier>();
  List<StatModifier> healthRegenPerSecondAdditions = new List<StatModifier>();
  List<StatModifier> healthRegenPerSecondMultipliers = new List<StatModifier>();
  public float healthRegenPerSecondFinal = 0;
  int healthRegenPerRegen = 0;
  float healthRegenLast = 0;
  float healthRegenDelay = 0;
  public bool isDead = false;
  public GameObject headerPrefab;
  protected GameObject header;
  protected Image[] headerBars;
  public Image healthFrame;
  public Image healthBar;
  public string entityName;
  protected GameObject canvas;
  protected bool isHeaderVisible = false;
  public GameObject corpse;
  public bool destroyOnDie = false;
  public int moneyToReward = 1;
  public GameObject fatalAttacker;
  public Transform body;
  public Transform headerAnchor;
  public class StatModifier {
    public float value;
    public string identifier;
    public StatModifier(float newValue = 0, string newIdentifier = "") {
      value = newValue;
      identifier = newIdentifier;
    }
  }

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
    UpdateProcess();
  }

  protected void UpdateProcess() {
    if (header) {
      Vector2 headerScreenPosition = Camera.main.WorldToScreenPoint (new Vector3 (headerAnchor.position.x, headerAnchor.position.y + 2f, headerAnchor.position.z));
      header.transform.position = new Vector2 (headerScreenPosition.x, headerScreenPosition.y + 75);
      healthBar.fillAmount = (float) currentHealth / maxHealth;
    }

    if (healthRegen) {
      if (currentHealth < maxHealth && Time.time > healthRegenLast + healthRegenDelay) {
        ReceiveHealing(healthRegenPerRegen, false);
        healthRegenLast = Time.time;
      }
    }

    if (Input.GetKeyDown (KeyCode.V)) {
      TakeDamage (Random.Range (1, 21), this.gameObject);
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

  public void Die (GameObject attacker) {
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
    foreach (StatModifier healthRegenPerSecondBaseMultiplier in healthRegenPerSecondBaseMultipliers)
    {
      healthRegenPerSecondTemp *= healthRegenPerSecondBaseMultiplier.value;
    }
    foreach (StatModifier healthRegenPerSecondAddition in healthRegenPerSecondAdditions)
    {
      healthRegenPerSecondTemp += healthRegenPerSecondAddition.value;
    }
    foreach (StatModifier healthRegenPerSecondMultiplier in healthRegenPerSecondMultipliers)
    {
      healthRegenPerSecondTemp *= healthRegenPerSecondMultiplier.value;
    }
    healthRegenPerSecondFinal = healthRegenPerSecondTemp;

    healthRegenPerRegen = Mathf.CeilToInt(healthRegenPerSecondFinal / 2);
    healthRegenDelay = healthRegenPerRegen / healthRegenPerSecondFinal;
  }
  void UpdateArmor() {
    float armorTemp = armorBase;
    foreach (StatModifier armorBaseMultiplier in armorBaseMultipliers)
    {
      armorTemp *= armorBaseMultiplier.value;
    }
    foreach (StatModifier armorAddition in armorAdditions)
    {
      armorTemp += armorAddition.value;
    }
    foreach (StatModifier armorMultiplier in armorMultipliers)
    {
      armorTemp *= armorMultiplier.value;
    }
    armorFinal = Mathf.RoundToInt(armorTemp);
  }
  void UpdateDamage() {
    float damageTemp = damageBase;
    foreach (StatModifier damageBaseMultiplier in damageBaseMultipliers)
    {
      damageTemp *= damageBaseMultiplier.value;
    }
    foreach (StatModifier damageAddition in damageAdditions)
    {
      damageTemp += damageAddition.value;
    }
    foreach (StatModifier damageMultiplier in damageMultipliers)
    {
      damageTemp *= damageMultiplier.value;
    }
    damageFinal = Mathf.RoundToInt(damageTemp);
  }

  public void AddHealthRegenPerSecondBaseMultiplier(float value, string identifier) {
    if (healthRegen) {
      if (AddStatModifier(healthRegenPerSecondBaseMultipliers, value, identifier)) {
        UpdateHealthRegenPerSecond();
      }
    }
  }
  public void RemoveHealthRegenPerSecondBaseMultiplier(string identifier) {
    if (healthRegen) {
      if (RemoveStatModifier(healthRegenPerSecondBaseMultipliers, identifier)) {
        UpdateHealthRegenPerSecond();
      }
    }
  }
  public void AddHealthRegenPerSecondAddition(float value, string identifier) {
    if (healthRegen) {
      if (AddStatModifier(healthRegenPerSecondAdditions, value, identifier)) {
        UpdateHealthRegenPerSecond();
      }
    }
  }
  public void RemoveHealthRegenPerSecondAddition(string identifier) {
    if (healthRegen) {
      if (RemoveStatModifier(healthRegenPerSecondAdditions, identifier)) {
        UpdateHealthRegenPerSecond();
      }
    }
  }
  public void AddHealthRegenPerSecondMultiplier(float value, string identifier) {
    if (healthRegen) {
      if (AddStatModifier(healthRegenPerSecondMultipliers, value, identifier)) {
        UpdateHealthRegenPerSecond();
      }
    }
  }
  public void RemoveHealthRegenPerSecondMultiplier(string identifier) {
    if (healthRegen) {
      if (RemoveStatModifier(healthRegenPerSecondMultipliers, identifier)) {
        UpdateHealthRegenPerSecond();
      }
    }
  }

  public void AddArmorBaseMultiplier(float value, string identifier) {
    if (AddStatModifier(armorBaseMultipliers, value, identifier)) {
      UpdateArmor();
    }
  }
  public void RemoveArmorBaseMultiplier(string identifier) {
    if (RemoveStatModifier(armorBaseMultipliers, identifier)) {
      UpdateArmor();
    }
  }
  public void AddArmorAddition(int value, string identifier) {
    if (AddStatModifier(armorAdditions, value, identifier)) {
      UpdateArmor();
    }
  }
  public void RemoveArmorAddition(string identifier) {
    if (RemoveStatModifier(armorAdditions, identifier)) {
      UpdateArmor();
    }
  }
  public void AddArmorMultiplier(float value, string identifier) {
    if (AddStatModifier(armorMultipliers, value, identifier)) {
      UpdateArmor();
    }
  }
  public void RemoveArmorMultiplier(string identifier) {
    if (RemoveStatModifier(armorMultipliers, identifier)) {
      UpdateArmor();
    }
  }

  public void AddDamageBaseMultiplier(float value, string identifier) {
    if (AddStatModifier(damageBaseMultipliers, value, identifier)) {
      UpdateDamage();
    }
  }
  public void RemoveDamageBaseMultiplier(string identifier) {
    if (RemoveStatModifier(damageBaseMultipliers, identifier)) {
      UpdateDamage();
    }
  }
  public void AddDamageAddition(int value, string identifier) {
    if (AddStatModifier(damageAdditions, value, identifier)) {
      UpdateDamage();
    }
  }
  public void RemoveDamageAddition(string identifier) {
    if (RemoveStatModifier(damageAdditions, identifier)) {
      UpdateDamage();
    }
  }
  public void AddDamageMultiplier(float value, string identifier) {
    if (AddStatModifier(damageMultipliers, value, identifier)) {
      UpdateDamage();
    }
  }
  public void RemoveDamageMultiplier(string identifier) {
    if (RemoveStatModifier(damageMultipliers, identifier)) {
      UpdateDamage();
    }
  }

  bool AddStatModifier(List<StatModifier> statModifiers, float value, string identifier) {
    bool found = false;
    bool updateNeeded = true;
    foreach (StatModifier statModifier in statModifiers)
    {
      if (statModifier.identifier == identifier) {
        if (statModifier.value == value) {
          updateNeeded = false;
        } else {
          statModifier.value = value;
        }
        found = true;
      }
    }
    if (!found) {
      statModifiers.Add(new StatModifier(value, identifier));
    }
    return updateNeeded;
  }
  bool RemoveStatModifier(List<StatModifier> statModifiers, string identifier) {
    bool updateNeeded = false;
    StatModifier stat = new StatModifier();
    foreach (StatModifier statModifier in statModifiers)
    {
      if (statModifier.identifier == identifier) {
        stat = statModifier;
        statModifiers.Remove(stat);
        updateNeeded = true;
      }
    }
    return updateNeeded;
  }
}