using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretStatManager : MonoBehaviour
{
  public HealthSimple health;
  public int price = 3;
  public string turretName = "Default";
  public float lifeTimeBase = 10;
  List<Tools.StatModifier> lifeTimeBaseMultipliers = new List<Tools.StatModifier>();
  List<Tools.StatModifier> lifeTimeAdditions = new List<Tools.StatModifier>();
  List<Tools.StatModifier> lifeTimeMultipliers = new List<Tools.StatModifier>();
  public float lifeTimeFinal;


  void Start()
  {
    UpdateLifeTime();
  }

  void Update()
  {
  }

  void UpdateLifeTime() {
    float lifeTimeTemp = lifeTimeBase;
    foreach (Tools.StatModifier lifeTimeBaseMultiplier in lifeTimeBaseMultipliers)
    {
      lifeTimeTemp *= lifeTimeBaseMultiplier.value;
    }
    foreach (Tools.StatModifier lifeTimeAddition in lifeTimeAdditions)
    {
      lifeTimeTemp += lifeTimeAddition.value;
    }
    foreach (Tools.StatModifier lifeTimeMultiplier in lifeTimeMultipliers)
    {
      lifeTimeTemp *= lifeTimeMultiplier.value;
    }
    lifeTimeFinal = lifeTimeTemp;
    health.healthRegenPerSecondBase = health.maxHealthBase / lifeTimeFinal;
    health.UpdateHealthRegenPerSecond();
  }

  public void AddLifeTimeBaseMultiplier(float value, string identifier) {
    if (Tools.AddStatModifier(lifeTimeBaseMultipliers, value, identifier)) {
      UpdateLifeTime();
    }
  }
  public void RemoveLifeTimeBaseMultiplier(string identifier) {
    if (Tools.RemoveStatModifier(lifeTimeBaseMultipliers, identifier)) {
      UpdateLifeTime();
    }
  }
  public void AddLifeTimeAddition(float value, string identifier) {
    if (Tools.AddStatModifier(lifeTimeAdditions, value, identifier)) {
      UpdateLifeTime();
    }
  }
  public void RemoveLifeTimeAddition(string identifier) {
    if (Tools.RemoveStatModifier(lifeTimeAdditions, identifier)) {
      UpdateLifeTime();
    }
  }
  public void AddLifeTimeMultiplier(float value, string identifier) {
    if (Tools.AddStatModifier(lifeTimeMultipliers, value, identifier)) {
      UpdateLifeTime();
    }
  }
  public void RemoveLifeTimeMultiplier(string identifier) {
    if (Tools.RemoveStatModifier(lifeTimeMultipliers, identifier)) {
      UpdateLifeTime();
    }
  }
}
