using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactersManager : Manager
{
  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    if (rangeSnapshotWanted)
    {
      rangeSnapshotWanted = false;
      TurretController[] turretControllers = GetComponentsInChildren<TurretController>();
      foreach (TurretController turretController in turretControllers)
      {
        turretController.snapshotWanted = true;
        turretController.targetUpdateWanted = true;
      }
    }
  }
}