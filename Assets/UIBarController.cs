using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIBarController : MonoBehaviour
{
  public DetailLevel detailLevel;
  int maxValue;
  public RectTransform barLimit;
  public float barLimitMinX;
  public float barLimitMaxX;
  float barLimitSizeX;
  public Image valueBar;
  public float valueFillMin;
  public float valueFillMax;
  float valuefillSize;
  public Image backValueBar;
  float valuePercentage;
  float lastValueDecreaseTime = -1;
  float backValueDecreaseDelay = 0.2f;
  float lastValue;
  float currentValue = -1;
  float backValueDecreaseStartFillAmount;
  float backValueDecreaseStartTime;
  bool backValueDecreasing = false;
  bool waitingForDecrease = false;
  float decreaseInterpolation;
  bool initiated = false;
  GameObject barLinePrefab;
  GameObject barLineThinPrefab;
  List<RectTransform> barLines = new List<RectTransform>();
  public TMP_Text textCurrent;
  public TMP_Text textDivide;
  public TMP_Text textMax;

  public enum DetailLevel
  {
    none,
    high
  }

  void Start()
  {
    valuefillSize = valueFillMax - valueFillMin;
    barLimitSizeX = barLimitMaxX - barLimitMinX;
    barLinePrefab = Resources.Load<GameObject>("Prefabs/UI/BarLine");
    barLineThinPrefab = Resources.Load<GameObject>("Prefabs/UI/BarLineThin");
    UpdateTextDivide();
  }

  void Update()
  {
    if (initiated && valueBar) {
      valuePercentage = currentValue / maxValue;
      if (textCurrent) {
        textCurrent.text = currentValue.ToString();
      }
      valueBar.fillAmount = valueFillMin + (valuefillSize * valuePercentage);
      if (barLimit)
      {
        barLimit.anchoredPosition = new Vector2(barLimitMinX + (barLimitSizeX * valuePercentage), barLimit.anchoredPosition.y);
      }
      if (lastValue != currentValue) {
        if (lastValue > currentValue) {
          lastValueDecreaseTime = Time.time;
          if (backValueDecreasing) {
            backValueDecreaseStartFillAmount = backValueBar.fillAmount;
            backValueDecreaseStartTime = Time.time;
          } else if (!waitingForDecrease) {
            if (name == "HealthBarMask") {
              Debug.Log("Ouno");
              Debug.Log(backValueBar.fillAmount);
            }
            backValueBar.fillAmount = valueFillMin + (valuefillSize * (lastValue / maxValue));
            if (name == "HealthBarMask") {
              Debug.Log("Ouno bis");
              Debug.Log(backValueBar.fillAmount);
            }
            waitingForDecrease = true;
          }
        }
        lastValue = currentValue;
      }
      if (!backValueDecreasing && lastValueDecreaseTime != -1 && Time.time > lastValueDecreaseTime + backValueDecreaseDelay) {
        backValueDecreaseStartFillAmount = backValueBar.fillAmount;
        backValueDecreaseStartTime = Time.time;
        backValueDecreasing = true;
        waitingForDecrease = false;
      }
      if (backValueDecreasing) {
        decreaseInterpolation = (Time.time - backValueDecreaseStartTime) * 2;
        if (name == "HealthBarMask") {
          Debug.Log("Dausse");
          Debug.Log(backValueBar.fillAmount);
        }
        backValueBar.fillAmount = Mathf.Lerp(backValueDecreaseStartFillAmount, valueBar.fillAmount, decreaseInterpolation);
        if (name == "HealthBarMask") {
          Debug.Log("Dausse bis");
          Debug.Log(backValueBar.fillAmount);
        }
        if (decreaseInterpolation > 1) {
          backValueDecreasing = false;
          lastValueDecreaseTime = -1;
        }
      }
    }
    // if (name == "HealthBarMask") {
    //   Debug.Log(name + " " + waitingForDecrease);
    //   Debug.Log(name + " " + backValueDecreasing);
    //   Debug.Log(name + " " + backValueDecreaseStartTime);
    //   Debug.Log(name + " " + decreaseInterpolation);
    // }
  }

  public void SetCurrentValue(int value) {
    currentValue = value;
  }

  public bool IsInitiated() {
    return initiated;
  }

  public void Init(int max) {
    UpdateBar(max);
    initiated = true;
  }

  public void UpdateBar(int max) {
    maxValue = max;
    if (textMax) {
      textMax.text = maxValue.ToString();
    }
    if (detailLevel == DetailLevel.high) {
      foreach (RectTransform barLine in barLines)
      {
        Destroy(barLine.gameObject);
      }
      barLines.Clear();

      int counter = 250;
      while (counter < maxValue)
      {
        float xPosition = barLimitMinX + (((float) counter / maxValue) * barLimitSizeX);
        GameObject prefab;
        if (counter % 1000 == 0) {
          prefab = barLinePrefab;
        } else {
          prefab = barLineThinPrefab;
        }
        RectTransform barLine = Instantiate(prefab, barLimit.position, barLimit.rotation).GetComponent<RectTransform>();
        barLine.transform.SetParent(valueBar.transform);
        barLine.localScale *= 2;
        barLine.anchoredPosition = new Vector2(xPosition, barLine.anchoredPosition.y);
        barLines.Add(barLine);
        counter += 250;
      }
    }
    UpdateTextDivide();
  }

  void UpdateTextDivide() {
    if (textDivide) {
      if (textCurrent && textMax && maxValue > 0) {
        textDivide.enabled = true;
      } else {
        textDivide.enabled = false;
      }
    }
  }
}
