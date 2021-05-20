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
  int activeLevelFrame;

  void Start() {
    animator = GetComponent<Animator>();
    visibleLast = visible;
    UpdateVisibility();
    levelFrames = GetComponentsInChildren<LevelFrameController>();
    lvlUpChoices = GetComponentsInChildren<LvlUpChoicesController>();
  }

  void Update() {
    if (visibleLast != visible) {
      UpdateVisibility();
    }
    if (spellController && levelLast != spellController.level) {
      levelLast = spellController.level;
      UpdateLevel();
      if (levelLast == 1 || levelLast == 4 || levelLast == 7 || levelLast == 10 || levelLast == 13 || levelLast == 16 || levelLast == 20) {
        visible = true;
        SetActiveLevelFrame(levelLast);
      }
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
}
