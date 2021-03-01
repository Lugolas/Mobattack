using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoPanelController : MonoBehaviour
{
  public TMP_Text objectNameText;
  public string objectName = "Pascal";
  public TMP_Text damageText;
  public int damage = 1;
  public TMP_Text attackPerSecondText;
  public float attackPerSecond = 27;
  public TMP_Text rangeText;
  public float range = 1;
  public TMP_Text priceTitleText;
  public string priceTitle = "Price";
  public Button priceButton;
  public TMP_Text priceText;
  public int price = 1;
  public TurretUpgradeManager upgradeManager;

  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    objectNameText.text = objectName;
    damageText.text = damage.ToString();
    attackPerSecondText.text = attackPerSecond.ToString();
    rangeText.text = range.ToString();
    priceText.text = "$" + price.ToString();
    priceTitleText.text = priceTitle;
  }

  public void UpgradeTurret()
  {
    if (upgradeManager)
    {
      upgradeManager.Upgrade();
    }
  }
}
