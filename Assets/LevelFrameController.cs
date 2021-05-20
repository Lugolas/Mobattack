using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelFrameController : MonoBehaviour
{
  public Color unlockedColor;
  public Color unlockedColorText;
  public Color lockedColor;
  public Color lockedColorText;
  public Color activeColor;
  public Color activeColorText;
  bool active = false;
  bool activeLast;
  bool unlocked = false;
  bool unlockedLast;
  public int level;
  TMP_Text levelText;
  public Image colorFrame;

  void Start() {
    levelText = GetComponentInChildren<TMP_Text>();
    levelText.text = level.ToString();
    UpdateFrameColor();
  }

  public void SetUnlocked(bool state) {
    bool updateNeeded = false;
    if (unlocked != state) {
      updateNeeded = true;
    }
    unlocked = state;
    if (updateNeeded) {
      UpdateFrameColor();
    }
  }

  public void SetActive(bool state) {
    bool updateNeeded = false;
    if (active != state) {
      updateNeeded = true;
    }
    active = state;
    if (updateNeeded) {
      UpdateFrameColor();
    }
  }

  void UpdateFrameColor() {
    Color color = Color.red;
    Color colorText = Color.red;
    if (active) {
      color = activeColor;
      colorText = activeColorText;
    } else
    {
      if (unlocked)
      {
        color = unlockedColor;
        colorText = unlockedColorText;
      }
      else
      {
        color = lockedColor;
        colorText = lockedColorText;
      }
    }
    colorFrame.color = color;
    levelText.color = colorText;
  }
}
