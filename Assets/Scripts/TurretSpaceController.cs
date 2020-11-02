using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretSpaceController : MonoBehaviour
{
  TurretStatManager statManager;
  public float spaceRatio = 1f;
  private void Start()
  {
    statManager = transform.parent.GetComponent<TurretStatManager>();
    UpdateSpace();
  }

  void LateUpdate()
  {
    UpdateSpace();
  }
  
  public void UpdateSpace()
  {
    transform.localScale =
      new Vector3(statManager.space * spaceRatio, statManager.space * spaceRatio, statManager.space * spaceRatio);
  }
}
