using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LvlUpGroupControllerAfro : LvlUpGroupController
{
  SpellControllerAfro spellControllerAfro;
  bool initialized = false;
  private void Start() {
    StartProcess();
  }
  void Update()
  {
    UpdateProcess();
    if (spellController && !initialized) {
      spellControllerAfro = spellController.gameObject.GetComponent<SpellControllerAfro>();
      initialized = true;
    }
  }

  public override void Choice1_1() {
    spellControllerAfro.armsQuestOn = true;
    SetActivatedChoice(1, 1);
  }
  public override void Choice1_2() {
    spellControllerAfro.fistBigger1 = true;
    SetActivatedChoice(1, 2);
  }
  public override void Choice1_3() {
    spellControllerAfro.rageQuestOn = true;
    SetActivatedChoice(1, 3);
  }

  public override void Choice4_1() {
    spellControllerAfro.fistLaunchUp = true;
    SetActivatedChoice(4, 1);
  }
  public override void Choice4_2() {
    spellControllerAfro.fistEnemyBounces1 = true;
    spellControllerAfro.fistEnemyBouncesQuest = true;
    SetActivatedChoice(4, 2);
  }
  public override void Choice4_3() {
    spellControllerAfro.rageDecreaseDecreased = true;
    SetActivatedChoice(4, 3);
  }

  public override void Choice7_1() {
    spellControllerAfro.rageSpeed = true;
    SetActivatedChoice(7, 1);
  }
  public override void Choice7_2() {
    spellControllerAfro.rageDamage = true;
    SetActivatedChoice(7, 2);
  }
  public override void Choice7_3() {
    spellControllerAfro.rageHealthRegenPerSecond = true;
    SetActivatedChoice(7, 3);
  }

  public override void Choice10_1() {
    SetActivatedChoice(10, 1);
  }
  public override void Choice10_2() {
    SetActivatedChoice(10, 2);
  }
  public override void Choice10_3() {
    SetActivatedChoice(10, 3);
  }

  public override void Choice13_1() {
    spellControllerAfro.fistIgnoreFists = true;
    SetActivatedChoice(13, 1);
  }
  public override void Choice13_2() {
    spellControllerAfro.fistBounceUp = true;
    SetActivatedChoice(13, 2);
  }
  public override void Choice13_3() {
    spellControllerAfro.rageArmor = true;
    SetActivatedChoice(13, 3);
  }

  public override void Choice16_1() {
    spellControllerAfro.fistDivide = true;
    SetActivatedChoice(16, 1);
  }
  public override void Choice16_2() {
    spellControllerAfro.rageContagion = true;
    SetActivatedChoice(16, 2);
  }
  public override void Choice16_3() {
    spellControllerAfro.fistExplode = true;
    SetActivatedChoice(16, 3);
  }

  public override void Choice20_1() {
    SetActivatedChoice(20, 1);
  }
  public override void Choice20_2() {
    SetActivatedChoice(20, 2);
  }
  public override void Choice20_3() {
    spellControllerAfro.armLeft3Active = true;
    spellControllerAfro.armRight3Active = true;
    SetActivatedChoice(20, 3);
  }
}
