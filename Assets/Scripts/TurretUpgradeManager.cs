using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretUpgradeManager : MonoBehaviour
{
  public Transform turretSlot;
  GameObject currentTurret;
  public GameObject baseTurret;
  public GameObject upgradedTurret;
  public CheckInfo checkInfo;
  TurretPlayerLink playerLink;
  bool hasUpgraded = false;


  // Start is called before the first frame update
  void Start()
  {
    playerLink = GetComponent<TurretPlayerLink>();
    SpawnTurret(baseTurret);
  }

  // Update is called once per frame
  void Update()
  {

  }

  void SpawnTurret(GameObject turretPrefab)
  {
    currentTurret = Instantiate(turretPrefab, transform.position, transform.rotation);
    currentTurret.transform.SetParent(turretSlot);
  }

  void ClearTurret()
  {
    Destroy(currentTurret);
    currentTurret = null;
  }

  public void Upgrade()
  {
    if (!hasUpgraded && playerLink.characterWallet.GetMoney() >= upgradedTurret.GetComponent<TurretStatManager>().price)
    {
      ClearTurret();
      SpawnTurret(upgradedTurret);
      hasUpgraded = true;
    }
  }
}
