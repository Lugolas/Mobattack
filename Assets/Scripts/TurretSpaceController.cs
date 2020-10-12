using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretSpaceController : MonoBehaviour
{
  TurretStatManager statManager;
  private void Start()
  {
    statManager = transform.parent.GetComponent<TurretStatManager>();
    UpdateSpace();
  }
  public void UpdateSpace()
  {
    transform.localScale =
      new Vector3(statManager.space, statManager.space, statManager.space);
  }
}
