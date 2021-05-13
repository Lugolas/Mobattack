using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.Networking;
using Cinemachine;

public class HealthAfroBall : HealthSimple
{
  public TurretAfroBallController afroBall;
  bool layerChanged = false;
  float layerChangeTime;
  int initLayer;
  SpellControllerAfro spellControllerAfro;

  void Start()
  {
    initLayer = gameObject.layer;
  }

  void Update()
  {
    if (layerChanged && Time.time > layerChangeTime + 0.25f) {
      Tools.SetLayerRecursively(spellControllerAfro.gameObject, initLayer);
      layerChanged = false;
    }
  }

  public override bool TakeDamage(int damageAmount, SpellController attacker = null)
  {
    if (!isDead)
    {
      DamagePopUpController.CreateDamagePopUp(damageAmount.ToString(), transform, "red");

      spellControllerAfro = attacker.GetComponentInParent<SpellControllerAfro>();
      if (spellControllerAfro) {
        Tools.SetLayerRecursively(spellControllerAfro.gameObject, 24);
        layerChanged = true;
        layerChangeTime = Time.time;
      }

      isDead = true;
      afroBall.Launch(attacker);
      return true;
    }
    return false;
  }
}