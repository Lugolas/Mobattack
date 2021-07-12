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

  protected int lastMaxHealthKnown = 0;
  public int currentHealth;
  public int maxHealthBase = 100;
  List<Tools.StatModifier> maxHealthBaseMultipliers = new List<Tools.StatModifier>();
  List<Tools.StatModifier> maxHealthAdditions = new List<Tools.StatModifier>();
  List<Tools.StatModifier> maxHealthMultipliers = new List<Tools.StatModifier>();
  public int maxHealthFinal;
  public float speedBase = 0;
  List<Tools.StatModifier> speedBaseMultipliers = new List<Tools.StatModifier>();
  List<Tools.StatModifier> speedAdditions = new List<Tools.StatModifier>();
  List<Tools.StatModifier> speedMultipliers = new List<Tools.StatModifier>();
  public float speedFinal;
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
  public float delayBase = 0;
  List<Tools.StatModifier> delayBaseMultipliers = new List<Tools.StatModifier>();
  List<Tools.StatModifier> delayAdditions = new List<Tools.StatModifier>();
  List<Tools.StatModifier> delayMultipliers = new List<Tools.StatModifier>();
  public float delayFinal;
  public float rangeBase = 0;
  List<Tools.StatModifier> rangeBaseMultipliers = new List<Tools.StatModifier>();
  List<Tools.StatModifier> rangeAdditions = new List<Tools.StatModifier>();
  List<Tools.StatModifier> rangeMultipliers = new List<Tools.StatModifier>();
  public float rangeFinal;
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
    UpdateMaxHealth();
    lastMaxHealthKnown = maxHealthFinal;

    if (headerPrefab)
      HeaderInstantiate ();

    DamagePopUpController.Initialize ();
    currentHealth = maxHealthFinal;
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
    UpdateHealthRegenPerSecond();
    UpdateArmor();
    UpdateDamage();
    UpdateSpeed();
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
          healthBar.Init(maxHealthFinal);
        }
        if (maxHealthFinal != lastMaxHealthKnown) {
          lastMaxHealthKnown = maxHealthFinal;
          healthBar.UpdateBar(maxHealthFinal);
        }
      }
    }

    if (healthRegen && Time.time > healthRegenLast + healthRegenDelay) {
      if (healthRegenPerRegen > 0) {
        if (currentHealth < maxHealthFinal) {
          ReceiveHealing(healthRegenPerRegen, false);
          healthRegenLast = Time.time;
        }
      } else if (healthRegenPerRegen < 0) {
        if (currentHealth > 0) {
          TakeDamage(Mathf.Abs(healthRegenPerRegen), null, false);
          healthRegenLast = Time.time;
        }
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

  public virtual bool TakeDamage (int damageAmount, SpellController attacker = null, bool showText = true) {
    if (!isDead) {
      if (showText)
        DamagePopUpController.CreateDamagePopUp (damageAmount.ToString (), body, "red");

      UpdateHealth (currentHealth - damageAmount);

      if (currentHealth <= 0) {
        currentHealth = 0;
        if (!isDead)
          Die (attacker);
        return true;
      }
    }
    return false;
  }

  protected void UpdateHealth (int newHealth) {
    currentHealth = newHealth;
    if (currentHealth <= 0) {
      currentHealth = 0;
    }

    if (currentHealth >= maxHealthFinal) {
      currentHealth = maxHealthFinal;
    }
  }

  public void ReceiveHealing (int healingAmount, bool showText = true) {
    if (!isDead) {
      if (showText)
        DamagePopUpController.CreateDamagePopUp (healingAmount.ToString (), body, "green");

      UpdateHealth (currentHealth + healingAmount);

      if (currentHealth >= maxHealthFinal) {
        currentHealth = maxHealthFinal;
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

  public void UpdateHealthRegenPerSecond() {
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
  public void UpdateArmor() {
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
  public void UpdateDamage() {
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
  public void UpdateMaxHealth() {
    float maxHealthTemp = maxHealthBase;
    foreach (Tools.StatModifier maxHealthBaseMultiplier in maxHealthBaseMultipliers)
    {
      maxHealthTemp *= maxHealthBaseMultiplier.value;
    }
    foreach (Tools.StatModifier maxHealthAddition in maxHealthAdditions)
    {
      maxHealthTemp += maxHealthAddition.value;
    }
    foreach (Tools.StatModifier maxHealthMultiplier in maxHealthMultipliers)
    {
      maxHealthTemp *= maxHealthMultiplier.value;
    }
    maxHealthFinal = Mathf.RoundToInt(maxHealthTemp);
  }
  public void UpdateDelay() {
    float delayTemp = delayBase;
    foreach (Tools.StatModifier delayBaseMultiplier in delayBaseMultipliers)
    {
      delayTemp *= delayBaseMultiplier.value;
    }
    foreach (Tools.StatModifier delayAddition in delayAdditions)
    {
      delayTemp += delayAddition.value;
    }
    foreach (Tools.StatModifier delayMultiplier in delayMultipliers)
    {
      delayTemp *= delayMultiplier.value;
    }
    delayFinal = delayTemp;
  }
  public void UpdateRange() {
    float rangeTemp = rangeBase;
    foreach (Tools.StatModifier rangeBaseMultiplier in rangeBaseMultipliers)
    {
      rangeTemp *= rangeBaseMultiplier.value;
    }
    foreach (Tools.StatModifier rangeAddition in rangeAdditions)
    {
      rangeTemp += rangeAddition.value;
    }
    foreach (Tools.StatModifier rangeMultiplier in rangeMultipliers)
    {
      rangeTemp *= rangeMultiplier.value;
    }
    rangeFinal = rangeTemp;
  }
  public void UpdateSpeed() {
    float speedTemp = speedBase;
    foreach (Tools.StatModifier speedBaseMultiplier in speedBaseMultipliers)
    {
      speedTemp *= speedBaseMultiplier.value;
    }
    foreach (Tools.StatModifier speedAddition in speedAdditions)
    {
      speedTemp += speedAddition.value;
    }
    foreach (Tools.StatModifier speedMultiplier in speedMultipliers)
    {
      speedTemp *= speedMultiplier.value;
    }
    speedFinal = speedTemp;
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
public void AddMaxHealthBaseMultiplier(float value, string identifier) {
    if (Tools.AddStatModifier(maxHealthBaseMultipliers, value, identifier)) {
      UpdateMaxHealth();
    }
  }
  public void RemoveMaxHealthBaseMultiplier(string identifier) {
    if (Tools.RemoveStatModifier(maxHealthBaseMultipliers, identifier)) {
      UpdateMaxHealth();
    }
  }
  public void AddMaxHealthAddition(int value, string identifier) {
    if (Tools.AddStatModifier(maxHealthAdditions, value, identifier)) {
      UpdateMaxHealth();
    }
  }
  public void RemoveMaxHealthAddition(string identifier) {
    if (Tools.RemoveStatModifier(maxHealthAdditions, identifier)) {
      UpdateMaxHealth();
    }
  }
  public void AddMaxHealthMultiplier(float value, string identifier) {
    if (Tools.AddStatModifier(maxHealthMultipliers, value, identifier)) {
      UpdateMaxHealth();
    }
  }
  public void RemoveMaxHealthMultiplier(string identifier) {
    if (Tools.RemoveStatModifier(maxHealthMultipliers, identifier)) {
      UpdateMaxHealth();
    }
  }
  public void AddDelayBaseMultiplier(float value, string identifier) {
    if (Tools.AddStatModifier(delayBaseMultipliers, value, identifier)) {
      UpdateDelay();
    }
  }
  public void RemoveDelayBaseMultiplier(string identifier) {
    if (Tools.RemoveStatModifier(delayBaseMultipliers, identifier)) {
      UpdateDelay();
    }
  }
  public void AddDelayAddition(float value, string identifier) {
    if (Tools.AddStatModifier(delayAdditions, value, identifier)) {
      UpdateDelay();
    }
  }
  public void RemoveDelayAddition(string identifier) {
    if (Tools.RemoveStatModifier(delayAdditions, identifier)) {
      UpdateDelay();
    }
  }
  public void AddDelayMultiplier(float value, string identifier) {
    if (Tools.AddStatModifier(delayMultipliers, value, identifier)) {
      UpdateDelay();
    }
  }
  public void RemoveDelayMultiplier(string identifier) {
    if (Tools.RemoveStatModifier(delayMultipliers, identifier)) {
      UpdateDelay();
    }
  }
  public void AddRangeBaseMultiplier(float value, string identifier) {
    if (Tools.AddStatModifier(rangeBaseMultipliers, value, identifier)) {
      UpdateRange();
    }
  }
  public void RemoveRangeBaseMultiplier(string identifier) {
    if (Tools.RemoveStatModifier(rangeBaseMultipliers, identifier)) {
      UpdateRange();
    }
  }
  public void AddRangeAddition(float value, string identifier) {
    if (Tools.AddStatModifier(rangeAdditions, value, identifier)) {
      UpdateRange();
    }
  }
  public void RemoveRangeAddition(string identifier) {
    if (Tools.RemoveStatModifier(rangeAdditions, identifier)) {
      UpdateRange();
    }
  }
  public void AddRangeMultiplier(float value, string identifier) {
    if (Tools.AddStatModifier(rangeMultipliers, value, identifier)) {
      UpdateRange();
    }
  }
  public void RemoveRangeMultiplier(string identifier) {
    if (Tools.RemoveStatModifier(rangeMultipliers, identifier)) {
      UpdateRange();
    }
  }
  public void AddSpeedBaseMultiplier(float value, string identifier) {
    if (Tools.AddStatModifier(speedBaseMultipliers, value, identifier)) {
      UpdateSpeed();
    }
  }
  public void RemoveSpeedBaseMultiplier(string identifier) {
    if (Tools.RemoveStatModifier(speedBaseMultipliers, identifier)) {
      UpdateSpeed();
    }
  }
  public void AddSpeedAddition(int value, string identifier) {
    if (Tools.AddStatModifier(speedAdditions, value, identifier)) {
      UpdateSpeed();
    }
  }
  public void RemoveSpeedAddition(string identifier) {
    if (Tools.RemoveStatModifier(speedAdditions, identifier)) {
      UpdateSpeed();
    }
  }
  public void AddSpeedMultiplier(float value, string identifier) {
    if (Tools.AddStatModifier(speedMultipliers, value, identifier)) {
      UpdateSpeed();
    }
  }
  public void RemoveSpeedMultiplier(string identifier) {
    if (Tools.RemoveStatModifier(speedMultipliers, identifier)) {
      UpdateSpeed();
    }
  }

  public List<Tools.StatModifier> GetHealthRegenPerSecondBaseMultipliers() {
    return healthRegenPerSecondBaseMultipliers;
  }
  public List<Tools.StatModifier> GetHealthRegenPerSecondAdditions() {
    return healthRegenPerSecondAdditions;
  }
  public List<Tools.StatModifier> GetHealthRegenPerSecondMultipliers() {
    return healthRegenPerSecondMultipliers;
  }
  public List<Tools.StatModifier> GetArmorBaseMultipliers() {
    return armorBaseMultipliers;
  }
  public List<Tools.StatModifier> GetArmorAdditions() {
    return armorAdditions;
  }
  public List<Tools.StatModifier> GetArmorMultipliers() {
    return armorMultipliers;
  }
  public List<Tools.StatModifier> GetDamageBaseMultipliers() {
    return damageBaseMultipliers;
  }
  public List<Tools.StatModifier> GetDamageAdditions() {
    return damageAdditions;
  }
  public List<Tools.StatModifier> GetDamageMultipliers() {
    return damageMultipliers;
  }
  public List<Tools.StatModifier> GetSpeedMultipliers() {
    return speedMultipliers;
  }
}