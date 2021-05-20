using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LvlUpChoicesController : MonoBehaviour
{
  public Color unlockedColorTitle;
  public Color unlockedColorText;
  public Color lockedColorTitle;
  public Color lockedColorText;
  public Color activeColorTitle;
  public Color activeColorText;
  bool active = false;
  bool activeLast;
  bool unlocked = false;
  bool unlockedLast;
  public List<TMP_Text> titles = new List<TMP_Text>();
  public List<TMP_Text> descriptions = new List<TMP_Text>();
  public int level;

  void Start() {
    UpdateTextColor();
  }

  public void SetUnlocked(bool state) {
    bool updateNeeded = false;
    if (unlocked != state) {
      updateNeeded = true;
    }
    unlocked = state;
    if (updateNeeded) {
      UpdateTextColor();
    }
  }

  public void SetActive(bool state) {
    bool updateNeeded = false;
    if (active != state) {
      updateNeeded = true;
    }
    active = state;
    if (updateNeeded) {
      UpdateTextColor();
    }
  }

  void UpdateTextColor() {
    Color colorTitle = Color.red;
    Color colorText = Color.red;
    if (active) {
      colorTitle = activeColorTitle;
      colorText = activeColorText;
    } else
    {
      if (unlocked)
      {
        colorTitle = unlockedColorTitle;
        colorText = unlockedColorText;
      }
      else
      {
        colorTitle = lockedColorTitle;
        colorText = lockedColorText;
      }
    }
    foreach (TMP_Text title in titles)
    {
      title.color = colorTitle;
    }
    foreach (TMP_Text description in descriptions)
    {
      description.color = colorText;
    }
  }
}
