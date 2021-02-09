using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagersManager : Manager
{
  public List<Manager> managers = new List<Manager>();
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
      foreach (Manager manager in managers)
      {
        manager.rangeSnapshotWanted = true;
      }
    }
  }
}
