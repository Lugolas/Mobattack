using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TurretController : MonoBehaviour
{
  public List<GameObject> enemiesInRange = new List<GameObject>();

  public bool targetUpdateWanted = false;
}
