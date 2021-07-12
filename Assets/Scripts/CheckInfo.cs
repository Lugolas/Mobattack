using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckInfo : MonoBehaviour
{
  bool isBeingChecked = false;
  public List<GameObject> visibleOnCheck = new List<GameObject>();
  public GameObject infoPanelPrefab;
  GameObject canvas;
  GameObject infoPanel;
  InfoPanelController infoPanelController;
  public TurretStatManager statManager;
  TurretUpgradeManager upgradeManager;

  string objectName;
  int damage;
  float attackPerSecond;
  float range;
  string priceTitle = "Upgrade";
  int price;

  void Start()
  {
    if (!statManager) {
      statManager = GetComponent<TurretStatManager>();
      if (!statManager)
      {
        statManager = transform.parent.GetComponent<TurretStatManager>();
      }
    }

    upgradeManager = GetComponent<TurretUpgradeManager>();
    if (!upgradeManager)
    {
      upgradeManager = transform.parent.GetComponent<TurretUpgradeManager>();
    }

    objectName = statManager.turretName;
    // damage = statManager.damage;
    // attackPerSecond = 1f / statManager.delay;
    price = statManager.price;
    // range = statManager.range;
    canvas = GameObject.Find("Canvas");
  }

  void Update()
  {
    if (isBeingChecked && infoPanel)
    {
      Vector2 infoPanelPosition = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z));
      infoPanel.transform.position = new Vector2(infoPanelPosition.x + 75, infoPanelPosition.y + 75);
    }
  }

  public void UpdateValues()
  {
    if (infoPanel)
    {
      infoPanelController.damage = 1;
      // infoPanelController.damage = statManager.damage;
      infoPanelController.attackPerSecond = Mathf.Round((1f / 2) * 100.0f) / 100.0f;
      // infoPanelController.attackPerSecond = Mathf.Round((1f / statManager.delay) * 100.0f) / 100.0f;
      // infoPanelController.range = statManager.range;
      infoPanelController.price = statManager.price;
      infoPanelController.objectName = statManager.turretName;
      infoPanelController.priceTitle = priceTitle;
    }
  }

  public void Check()
  {
    if (!infoPanel)
    {
      infoPanel = Instantiate(infoPanelPrefab);
      infoPanel.transform.SetParent(canvas.transform, false);
      infoPanelController = infoPanel.GetComponent<InfoPanelController>();
      infoPanelController.upgradeManager = upgradeManager;

      visibleOnCheck.Add(infoPanel);
    }

    UpdateValues();

    foreach (GameObject visibleObject in visibleOnCheck)
    {
      visibleObject.SetActive(true);
    }
    isBeingChecked = true;
  }

  public void StopCheck()
  {
    foreach (GameObject visibleObject in visibleOnCheck)
    {
      visibleObject.SetActive(false);
    }
    isBeingChecked = false;
  }
}
