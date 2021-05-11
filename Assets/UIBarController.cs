using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBarController : MonoBehaviour
{
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
  List<RectTransform> barLines = new List<RectTransform>();
  public DetailLevel detailLevel;
  public enum DetailLevel
  {
    none,
    medium,
    high
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

    foreach (RectTransform barLine in barLines)
    {
      Destroy(barLine.gameObject);
    }
    barLines.Clear();

    int counter = 250;
    while (counter < maxValue)
    {
      float xPosition = barLimitMinX + (((float) counter / maxValue) * barLimitSizeX);
      RectTransform barLine = Instantiate(barLinePrefab, barLimit.position, barLimit.rotation).GetComponent<RectTransform>();
      barLine.transform.SetParent(valueBar.transform);
      barLine.localScale *= 2;
      barLine.anchoredPosition = new Vector2(xPosition, barLine.anchoredPosition.y);
      barLines.Add(barLine);
      counter += 250;
    }
  }

  void Start()
  {
    valuefillSize = valueFillMax - valueFillMin;
    barLimitSizeX = barLimitMaxX - barLimitMinX;
    barLinePrefab = Resources.Load<GameObject>("Prefabs/UI/BarLine");
  }

  void Update()
  {
    if (initiated && valueBar) {
      valuePercentage = currentValue / maxValue;
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
            backValueBar.fillAmount = valueFillMin + (valuefillSize * (lastValue / maxValue));
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
        backValueBar.fillAmount = Mathf.Lerp(backValueDecreaseStartFillAmount, valueBar.fillAmount, decreaseInterpolation);
        if (decreaseInterpolation > 1) {
          backValueDecreasing = false;
          lastValueDecreaseTime = -1;
        }
      }
    }
  }
}
