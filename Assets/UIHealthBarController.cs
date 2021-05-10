using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBarController : MonoBehaviour
{
  HealthDamage health;
  GameObject player;
  GameObject character;
  public RectTransform barLimit;
  public float barLimitMinX;
  public float barLimitMaxX;
  float barLimitSizeX;
  public Image healthBar;
  public float healthFillMin;
  public float healthFillMax;
  float healthfillSize;
  public Image backHealthBar;
  float healthPercentage;
  float lastHealthDecreaseTime = -1;
  float backHealthDecreaseDelay = 0.2f;
  float lastHealth;
  float currentHealth;
  float backHealthDecreaseStartFillAmount;
  float backHealthDecreaseStartTime;
  bool backHealthDecreasing = false;
  bool waitingForDecrease = false;
  float decreaseInterpolation;


  void Start()
  {
    healthfillSize = healthFillMax - healthFillMin;
    barLimitSizeX = barLimitMaxX - barLimitMinX;
  }

  void Update()
  {
    if (!health) {
      if (!player) {
        player = GameObject.Find("Player(Clone)");
      } else {
        if (!character) {
          character = player.GetComponent<PlayerInputManager>().character;
        } else {
          health = character.GetComponent<HealthDamage>();
        }
      }
    } else {
      currentHealth = health.currentHealth;
      healthPercentage = currentHealth / health.maxHealth;
      healthBar.fillAmount = healthFillMin + (healthfillSize * healthPercentage);
      barLimit.anchoredPosition = new Vector2(barLimitMinX + (barLimitSizeX * healthPercentage), barLimit.anchoredPosition.y);

      if (lastHealth != currentHealth) {
        if (lastHealth > currentHealth) {
          lastHealthDecreaseTime = Time.time;
          if (backHealthDecreasing) {
            backHealthDecreaseStartFillAmount = backHealthBar.fillAmount;
            backHealthDecreaseStartTime = Time.time;
          } else if (!waitingForDecrease) {
            backHealthBar.fillAmount = healthFillMin + (healthfillSize * (lastHealth / health.maxHealth));
            waitingForDecrease = true;
          }
        }
        lastHealth = currentHealth;
      }
      if (!backHealthDecreasing && lastHealthDecreaseTime != -1 && Time.time > lastHealthDecreaseTime + backHealthDecreaseDelay) {
        backHealthDecreaseStartFillAmount = backHealthBar.fillAmount;
        backHealthDecreaseStartTime = Time.time;
        backHealthDecreasing = true;
        waitingForDecrease = false;
      }
      if (backHealthDecreasing) {
        decreaseInterpolation = (Time.time - backHealthDecreaseStartTime) * 2;
        backHealthBar.fillAmount = Mathf.Lerp(backHealthDecreaseStartFillAmount, healthBar.fillAmount, decreaseInterpolation);
        if (decreaseInterpolation > 1) {
          backHealthDecreasing = false;
          lastHealthDecreaseTime = -1;
        }
      }
    }
  }
}
