using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIBarsController : MonoBehaviour
{
  HealthDamage health;
  SpellController spellController;
  int lastMaxHealthKnown;
  int lastMaxManaKnown;
  int lastMaxExpKnown;
  GameObject player;
  GameObject character;
  public UIBarController healthBar;
  public UIBarController manaBar;
  public UIExpBarController expBar;
  public TMP_Text levelText;
  public LvlUpGroupController lvlUpGroup;

  void Update()
  {
    if (!health) {
      if (!player) {
        player = GameObject.Find("Player(Clone)");
      } else {
        if (!character) {
          character = player.GetComponent<PlayerInputManager>().character;
        } else {
          health = character.GetComponent<HealthDamage>();
          spellController = health.GetComponent<SpellController>();
          lastMaxHealthKnown = health.maxHealthFinal;
          lvlUpGroup.spellController = spellController;
        }
      }
    } else {
      if (healthBar.IsInitiated()) {
        healthBar.SetCurrentValue(health.currentHealth);
      } else {
        healthBar.Init(health.maxHealthFinal);
      }
      if (health.maxHealthFinal != lastMaxHealthKnown) {
        lastMaxHealthKnown = health.maxHealthFinal;
        healthBar.UpdateBar(health.maxHealthFinal);
      }

      if (manaBar.IsInitiated()) {
        manaBar.SetCurrentValue(health.currentMana);
      } else {
        manaBar.Init(health.maxMana);
      }
      if (health.maxMana != lastMaxManaKnown) {
        lastMaxManaKnown = health.maxMana;
        manaBar.UpdateBar(health.maxMana);
      }

      if (expBar.IsInitiated()) {
        expBar.SetCurrentExp(health.currentExp);
      } else {
        expBar.Init(health.maxExp);
      }
      if (health.maxExp != lastMaxExpKnown) {
        lastMaxExpKnown = health.maxExp;
        expBar.UpdateBar(health.maxExp);
      }

      levelText.text = spellController.level.ToString();
    }
  }
}
