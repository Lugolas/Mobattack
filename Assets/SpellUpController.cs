using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpellUpController : MonoBehaviour
{
  public Color ultimateColor;
  public Color ultimateBackColor;
  public Image colorFrame;
  public TMP_Text spellInputText;
  public Image arrow;
  bool quest;
  public GameObject questImage;
  public GameObject passive;
  string spellInput;
  float width = 20;
  float widthPassive = 32;
  public enum UpgradeType
  {
    Passive,
    Spell,
    Ult
  }

  public void SetSpellInput(string input)
  {
    spellInputText.text = input;
  }

  public void SetUpgradeType(UpgradeType choice) 
  {
    if (choice == UpgradeType.Passive) {
      RectTransform rectTransform = GetComponent<RectTransform>();
      rectTransform.sizeDelta = new Vector2(widthPassive, rectTransform.sizeDelta.y);
      RectTransform rectTransformColor = colorFrame.GetComponent<RectTransform>();
      rectTransformColor.sizeDelta = new Vector2(widthPassive, rectTransformColor.sizeDelta.y);
      colorFrame.gameObject.SetActive(true);
      spellInputText.gameObject.SetActive(false);
      passive.SetActive(true);
    } else {
      if (choice == UpgradeType.Ult) {
        arrow.color = ultimateColor;
        spellInputText.color = ultimateColor;
        colorFrame.color = ultimateBackColor;
        colorFrame.gameObject.SetActive(true);
      }
      if (quest) {
        questImage.SetActive(true);
      } else {
        arrow.gameObject.SetActive(true);
      }
    }
  }
  public void SetQuest(bool isQuestUpgrade) {
    quest = isQuestUpgrade;
  }
}
