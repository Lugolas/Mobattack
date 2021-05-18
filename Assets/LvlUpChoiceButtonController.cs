using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LvlUpChoiceButtonController : MonoBehaviour
{
  public string title;
  public string description;
  public bool quest;
  public SpellInput spellInput;
  string spellInputText;
  SpellUpController spellUp;
  SpellUpController.UpgradeType upgradeType = SpellUpController.UpgradeType.Passive;
  TMP_Text titleText;
  TMP_Text descriptionText;
  public enum SpellInput
  {
    None,
    A,
    Z,
    E,
    R
  }

  private void Start() {
    TMP_Text[] texts = GetComponentsInChildren<TMP_Text>();
    titleText = texts[0];
    titleText.text = title;
    descriptionText = texts[1];
    descriptionText.text = description;
    spellUp = GetComponentInChildren<SpellUpController>();
    if (spellInput != SpellInput.None) {
      upgradeType = SpellUpController.UpgradeType.Spell;
      switch (spellInput)
      {
        case SpellInput.A:
          spellInputText = "A";
          break;
        case SpellInput.Z:
          spellInputText = "Z";
          break;
        case SpellInput.E:
          spellInputText = "E";
          break;
        case SpellInput.R:
          spellInputText = "R";
          upgradeType = SpellUpController.UpgradeType.Ult;
          break;
        default:
          upgradeType = SpellUpController.UpgradeType.Passive;
          break;
      }
    } else {
      upgradeType = SpellUpController.UpgradeType.Passive;
    }

      spellUp.SetQuest(quest);
      spellUp.SetSpellInput(spellInputText);
      spellUp.SetUpgradeType(upgradeType);
  }
}
