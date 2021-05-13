using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIExpBarController : MonoBehaviour
{
  int maxExp;
  RectTransform expBar;
  public float expRotMinZ;
  public float expRotMaxZ;
  float expRotSizeZ;
  float expPercentage;
  float lastExp;
  float currentExp = -1;
  float currentVisibleExp = -1;
  bool initiated = false;
  float expChangeTime = 0;
  bool expChanging = false;
  float expChangeDuration = 0.5f;
  float currentRotationZ;
  float interpolation = 0;
  float expAtChange;

  void Start()
  {
    expRotSizeZ = expRotMaxZ - expRotMinZ;
    expBar = GetComponent<RectTransform>();
  }

  void Update()
  {
    if (initiated && expBar) {
      if (lastExp != currentExp) {
        expAtChange = currentVisibleExp;
        lastExp = currentExp;
        expChangeTime = Time.time;
        expChanging = true;
      }
      if (expChanging) {
        interpolation = (Time.time - expChangeTime) / expChangeDuration;
        if (interpolation > 1) {
          interpolation = 1;
        }

        currentVisibleExp = Mathf.Lerp(expAtChange, currentExp, interpolation);
        expPercentage = currentVisibleExp / maxExp;

        currentRotationZ = expRotMinZ + (expRotSizeZ * expPercentage);
        expBar.rotation = Quaternion.Euler(expBar.rotation.x, expBar.rotation.y, currentRotationZ);

        if (currentVisibleExp == currentExp) {
          expChanging = false;
        }
      }
    }
  }

  public void SetCurrentExp(int exp) {
    currentExp = exp;
  }

  public bool IsInitiated() {
    return initiated;
  }

  public void Init(int max) {
    UpdateBar(max);
    initiated = true;
  }

  public void UpdateBar(int max) {
    maxExp = max;
  }
}
