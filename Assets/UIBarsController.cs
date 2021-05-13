using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBarsController : MonoBehaviour
{
  HealthDamage health;
  int lastMaxHealthKnown;
  int lastMaxManaKnown;
  int lastMaxExpKnown;
  GameObject player;
  GameObject character;
  public UIBarController healthBar;
  public UIBarController manaBar;
  public UIExpBarController expBar;

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
          lastMaxHealthKnown = health.maxHealth;
        }
      }
    } else {
      if (healthBar.IsInitiated()) {
        healthBar.SetCurrentValue(health.currentHealth);
      } else {
        healthBar.Init(health.maxHealth);
      }
      if (health.maxHealth != lastMaxHealthKnown) {
        lastMaxHealthKnown = health.maxHealth;
        healthBar.UpdateBar(health.maxHealth);
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
    }
  }
}
