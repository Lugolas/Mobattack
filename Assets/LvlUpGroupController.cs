using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LvlUpGroupController : MonoBehaviour
{
  Animator animator;
  bool visible = false;
  bool visibleLast;

  void Start() {
    animator = GetComponent<Animator>();
    visibleLast = visible;
    UpdateVisibility();
  }

  void Update() {
    if (visibleLast != visible) {
      UpdateVisibility();
    }
  }

  public void UpdateVisibility() {
    visibleLast = visible;
    animator.SetBool("Visible", visible);
  }

  public void ToggleVisible() {
    visible = !visible;
  }
}
