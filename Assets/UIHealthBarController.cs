using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBarController : MonoBehaviour
{
  HealthDamage health;
  int lastMaxHealthKnown;
  GameObject player;
  GameObject character;
  public UIBarController healthBar;
  public UIBarController manaBar;

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
    }
  }
}
