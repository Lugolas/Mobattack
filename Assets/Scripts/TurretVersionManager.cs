using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretVersionManager : MonoBehaviour
{
  TurretStatManager versionStats;
  TurretStatManager[] turretStatss;
  TurretStatManager turretStats;
  bool hasTransferedStats = false;
  TurretRangeController rangeController;
  TurretController[] turretControllers;
  CheckInfo checkInfo;

  // Start is called before the first frame update
  void Start()
  {
    versionStats = GetComponent<TurretStatManager>();
    turretStatss = GetComponentsInParent<TurretStatManager>();
    turretControllers = GetComponentsInChildren<TurretController>();

    foreach (TurretStatManager currentTurretStats in turretStatss)
    {
      if (currentTurretStats.gameObject != gameObject) {
        turretStats = currentTurretStats;
      }
    }

    if (turretStats)
    {
      rangeController = turretStats.GetComponentInChildren<TurretRangeController>();
      rangeController.UpdateRange();
      foreach (TurretController turretController in turretControllers)
      {
        rangeController.subscribeToRange(turretController);
      }
      checkInfo = turretStats.GetComponentInChildren<CheckInfo>();
      checkInfo.UpdateValues();
    }
  }

  // Update is called once per frame
  void Update()
  {
    if (!hasTransferedStats && turretStats)
    {
      // TransferStats(versionStats, turretStats);
    }
  }
}
