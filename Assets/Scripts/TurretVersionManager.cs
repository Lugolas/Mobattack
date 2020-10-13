﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretVersionManager : MonoBehaviour
{
  TurretStatManager versionStats;
  TurretStatManager turretStats;
  bool hasTransferedStats = false;
  TurretRangeController rangeController;
  TurretSpaceController spaceController;
  TurretCanonController[] canonControllers;
  CheckInfo checkInfo;

  // Start is called before the first frame update
  void Start()
  {
    versionStats = GetComponent<TurretStatManager>();
    turretStats = transform.parent.parent.GetComponent<TurretStatManager>();
    canonControllers = GetComponentsInChildren<TurretCanonController>();

    if (turretStats)
    {
      TransferStats(versionStats, turretStats);
    }
    rangeController = transform.parent.parent.GetComponentInChildren<TurretRangeController>();
    rangeController.UpdateRange();
    foreach (TurretCanonController canonController in canonControllers)
    {
      rangeController.subscribeToRange(canonController);
    }
    checkInfo = transform.parent.parent.GetComponentInChildren<CheckInfo>();
    checkInfo.UpdateValues();
    spaceController = transform.parent.parent.GetComponentInChildren<TurretSpaceController>();
    spaceController.UpdateSpace();
  }

  // Update is called once per frame
  void Update()
  {
    if (!hasTransferedStats)
    {
      TransferStats(versionStats, turretStats);
    }
  }

  void TransferStats(TurretStatManager from, TurretStatManager to)
  {
    to.delay = from.delay;
    to.damage = from.damage;
    to.price = from.price;
    to.range = from.range;
    to.space = from.space;
    to.turretName = from.turretName;
    hasTransferedStats = true;
  }
}