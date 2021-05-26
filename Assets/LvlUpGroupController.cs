using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LvlUpGroupController : MonoBehaviour
{
  Animator animator;
  bool visible = false;
  bool visibleLast;
  int levelLast;
  public SpellController spellController;
  LevelFrameController[] levelFrames;
  LvlUpChoicesController[] lvlUpChoices;
  List<int> waitingChoices = new List<int>();
  int activeLevelFrame;
  bool currentLevelAdded = false;
  bool choice1Chosen = false;
  bool choice4Chosen = false;
  bool choice7Chosen = false;
  bool choice10Chosen = false;
  bool choice13Chosen = false;
  bool choice16Chosen = false;
  bool choice20Chosen = false;

  void Start() {
    animator = GetComponent<Animator>();
    visibleLast = visible;
    UpdateVisibility();
    levelFrames = GetComponentsInChildren<LevelFrameController>();
    lvlUpChoices = GetComponentsInChildren<LvlUpChoicesController>();
  }

  void Update() {
    if (spellController && levelLast != spellController.level) {
      levelLast = spellController.level;
      UpdateLevel();
      if (levelLast == 1 || levelLast == 4 || levelLast == 7 || levelLast == 10 || levelLast == 13 || levelLast == 16 || levelLast == 20) {
        visible = true;
        if (waitingChoices.Count <= 0) {
          SetActiveChoices(levelLast);
        }
        waitingChoices.Add(levelLast);
        waitingChoices.Sort((a, b) => a.CompareTo(b));
      }
    }
    if (visibleLast != visible) {
      UpdateVisibility();
    }
  }

  public void NextLvlUpChoice() {
    if (waitingChoices.Count >= 2) {
      waitingChoices.RemoveAt(0);
      visible = true;
      SetActiveChoices(waitingChoices[0]);
    } else {
      if (waitingChoices.Count == 1) {
        waitingChoices.RemoveAt(0);
      }
      visible = false;
      SetActiveChoices(-1);
      SetActiveLevelFrame(levelLast);
    }
  }
  public void UpdateVisibility() {
    visibleLast = visible;
    animator.SetBool("Visible", visible);
  }

  public void ToggleVisible() {
    visible = !visible;
  }

  void UpdateLevel() {
    foreach (LevelFrameController levelFrame in levelFrames)
    {
      bool unlocked;
      if (levelFrame.level <= levelLast) {
        unlocked = true;
      } else {
        unlocked = false;
      }
      levelFrame.SetUnlocked(unlocked);
    }
    foreach (LvlUpChoicesController lvlUpChoice in lvlUpChoices)
    {
      bool unlocked;
      if (lvlUpChoice.level <= levelLast) {
        unlocked = true;
      } else {
        unlocked = false;
      }
      lvlUpChoice.SetUnlocked(unlocked);
    }
  }

  public void SetActiveLevelFrame(int level) {
    activeLevelFrame = level;
    UpdateLevelActivity(activeLevelFrame);
  }
  public void SetActiveChoices(int level) {
    foreach (LvlUpChoicesController lvlUpChoice in lvlUpChoices)
    {
      if (lvlUpChoice.level == level)
      {
        lvlUpChoice.SetActive(true);
      } else 
      {
        lvlUpChoice.SetActive(false);
      }
    }
    if (level != -1) {
      SetActiveLevelFrame(level);
    } else {
      UpdateLevelActivity(activeLevelFrame);
    }
  }

  void UpdateLevelActivity(int level) {
    foreach (LevelFrameController levelFrame in levelFrames)
    {
      if (levelFrame.level == level) {
        levelFrame.SetActive(true);
      } else {
        levelFrame.SetActive(false);
      }
    }
    foreach (LvlUpChoicesController lvlUpChoice in lvlUpChoices)
    {
      if (lvlUpChoice.level == level) {
        lvlUpChoice.gameObject.SetActive(true);
      } else {
        lvlUpChoice.gameObject.SetActive(false);
      }
    }
  }

  void SetActivatedChoice(int choiceGroup, int choice) {
    bool validChoice = false;
    if (waitingChoices.Count >= 1 && waitingChoices[0] == choiceGroup)
    {
      switch (choiceGroup)
      {
        case 1:
          if (!choice1Chosen)
          {
            validChoice = true;
            choice1Chosen = true;
          }
          break;
        case 4:
          if (!choice4Chosen)
          {
            validChoice = true;
            choice4Chosen = true;
          }
          break;
        case 7:
          if (!choice7Chosen)
          {
            validChoice = true;
            choice7Chosen = true;
          }
          break;
        case 10:
          if (!choice10Chosen)
          {
            validChoice = true;
            choice10Chosen = true;
          }
          break;
        case 13:
          if (!choice13Chosen)
          {
            validChoice = true;
            choice13Chosen = true;
          }
          break;
        case 16:
          if (!choice16Chosen)
          {
            validChoice = true;
            choice16Chosen = true;
          }
          break;
        case 20:
          if (!choice20Chosen)
          {
            validChoice = true;
            choice20Chosen = true;
          }
          break;
      }
    }
    if (validChoice) {
      foreach (LvlUpChoicesController lvlUpChoice in lvlUpChoices)
      {
        if (lvlUpChoice.level == choiceGroup) {
          lvlUpChoice.SetActivatedChoice(choice);
        }
      }
      NextLvlUpChoice();
    }
  }

  public void Choice1_1() {
    SetActivatedChoice(1, 1);
  }
  public void Choice1_2() {
    SetActivatedChoice(1, 2);
  }
  public void Choice1_3() {
    SetActivatedChoice(1, 3);
  }

  public void Choice4_1() {
    SetActivatedChoice(4, 1);
  }
  public void Choice4_2() {
    SetActivatedChoice(4, 2);
  }
  public void Choice4_3() {
    SetActivatedChoice(4, 3);
  }

  public void Choice7_1() {
    SetActivatedChoice(7, 1);
  }
  public void Choice7_2() {
    SetActivatedChoice(7, 2);
  }
  public void Choice7_3() {
    SetActivatedChoice(7, 3);
  }

  public void Choice10_1() {
    SetActivatedChoice(10, 1);
  }
  public void Choice10_2() {
    SetActivatedChoice(10, 2);
  }
  public void Choice10_3() {
    SetActivatedChoice(10, 3);
  }

  public void Choice13_1() {
    SetActivatedChoice(13, 1);
  }
  public void Choice13_2() {
    SetActivatedChoice(13, 2);
  }
  public void Choice13_3() {
    SetActivatedChoice(13, 3);
  }

  public void Choice16_1() {
    SetActivatedChoice(16, 1);
  }
  public void Choice16_2() {
    SetActivatedChoice(16, 2);
  }
  public void Choice16_3() {
    SetActivatedChoice(16, 3);
  }

  public void Choice20_1() {
    SetActivatedChoice(20, 1);
  }
  public void Choice20_2() {
    SetActivatedChoice(20, 2);
  }
  public void Choice20_3() {
    SetActivatedChoice(20, 3);
  }
}
