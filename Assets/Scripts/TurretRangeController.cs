using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretRangeController : MonoBehaviour
{
  CapsuleCollider rangeCollider;
  Transform rangeMesh;
  TurretStatManager statManager;
  TurretPlayerLink playerLink;
  List<TurretCanonController> canonControllers = new List<TurretCanonController>();
  float scaleRatio = 5f;

  private void Start()
  {
    rangeCollider = GetComponent<CapsuleCollider>();
    rangeMesh = transform.GetChild(0);
    statManager = transform.parent.GetComponent<TurretStatManager>();
    playerLink = transform.parent.GetComponent<TurretPlayerLink>();
    UpdateRange();
  }

  public void UpdateRange()
  {
    rangeCollider.radius = statManager.range;
    rangeMesh.transform.localScale =
      new Vector3(statManager.range / scaleRatio, statManager.range / scaleRatio, statManager.range / scaleRatio);
  }

  public void subscribeToRange(TurretCanonController canonController)
  {
    canonControllers.Add(canonController);
  }

  void OnTriggerEnter(Collider collider)
  {
    EnemyController enemy = collider.gameObject.GetComponent<EnemyController>();

    if (playerLink.activated && enemy)
    {
      foreach (TurretCanonController canonController in canonControllers)
      {
        if (!canonController.enemiesInRange.Contains(enemy.gameObject))
        {
          canonController.enemiesInRange.Add(enemy.gameObject);
          canonController.targetUpdateWanted = true;
        }
      }
    }
  }

  void OnTriggerExit(Collider collider)
  {
    EnemyController enemy = collider.gameObject.GetComponent<EnemyController>();

    if (playerLink.activated && enemy)
    {
      foreach (TurretCanonController canonController in canonControllers)
      {
        canonController.enemiesInRange.Remove(enemy.gameObject);
        canonController.targetUpdateWanted = true;
      }
    }
  }
}
