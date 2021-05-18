using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpellController : MonoBehaviour
{
  public GameObject body;
  public HealthDamage healthScript;
  protected bool spell1Active = false;
  protected bool spell2Active = false;
  protected bool spell3Active = false;
  protected bool spell4Active = false;
  protected bool spell1Available = false;
  protected bool spell2Available = false;
  protected bool spell3Available = false;
  protected bool spell4Available = false;
  public int level = 1;
  public List<LevelUpChoice> levelUpChoices = new List<LevelUpChoice>();
  public struct LevelUpChoice
  {
    GameObject choicesPrefab;
    int level;
  }

  public bool GetSpell1Active() {
    return spell1Active;
  }
  public bool GetSpell2Active() {
    return spell2Active;
  }
  public bool GetSpell3Active() {
    return spell3Active;
  }
  public bool GetSpell4Active() {
    return spell4Active;
  }
  public bool GetSpell1Available() {
    return spell1Available;
  }
  public bool GetSpell2Available() {
    return spell2Available;
  }
  public bool GetSpell3Available() {
    return spell3Available;
  }
  public bool GetSpell4Available() {
    return spell4Available;
  }
  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }

  virtual public void Spell1()
  {
    Debug.Log("Default Spell1 Behaviour");
  }
  virtual public void Spell2()
  {
    Debug.Log("Default Spell2 Behaviour");
  }
  virtual public void Spell3()
  {
    Debug.Log("Default Spell3 Behaviour");
  }
  virtual public void Spell4()
  {
    Debug.Log("Default Spell4 Behaviour");
  }

  virtual public bool Fire1(bool down)
  {
    Debug.Log("Default Fire1 Behaviour");
    return true;
  }
  virtual public void Fire2(bool down)
  {
    Debug.Log("Default Fire2 Behaviour");
  }

  virtual public void GotAKill(int expValue) {
    healthScript.currentExp += expValue;
  }

  virtual public void LevelUpProcess() {
    if (healthScript.currentExp >= healthScript.maxExp) {
      healthScript.currentExp = 0;
      level++;
      healthScript.maxExp = Mathf.RoundToInt(healthScript.maxExp * 1.25f);
    }
  }
}
