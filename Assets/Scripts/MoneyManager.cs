using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoneyManager : MonoBehaviour
{
  int money = 6;
  TMP_Text moneyText;

  // Start is called before the first frame update
  void Start()
  {
    moneyText = GameObject.Find("MoneyAmount").GetComponent<TMP_Text>();
    moneyText.text = money.ToString();
  }

  // Update is called once per frame
  void Update()
  {

  }

  void UpdateMoney(int newAmount)
  {
    money = newAmount;
    moneyText.text = money.ToString();
  }

  public void AddMoney(int amount)
  {
    UpdateMoney(money + amount);
  }

  public void SubstractMoney(int amount)
  {
    UpdateMoney(money - amount);
  }

  public int GetMoney()
  {
    return money;
  }
}