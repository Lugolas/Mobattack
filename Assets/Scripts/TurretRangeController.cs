using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretRangeController : MonoBehaviour
{
  Collider rangeCollider;
  Transform rangeMesh;
  TurretStatManager statManager;
  TurretPlayerLink playerLink;
  public bool zIsUp = false;
  List<TurretController> turretControllers = new List<TurretController>();
  public float scaleRatio = 5f;

  private void Start()
  {
    rangeCollider = GetComponent<Collider>();
    rangeMesh = transform.GetChild(0);
    statManager = transform.parent.GetComponent<TurretStatManager>();
    playerLink = transform.parent.GetComponent<TurretPlayerLink>();
    UpdateRange();
  }

  void LateUpdate()
  {
    UpdateRange();
  }

  public void UpdateRange()
  {
    float scaleY = rangeCollider.transform.localScale.y;
    float scaleZ = statManager.range * scaleRatio;

    if (zIsUp) {
      scaleY = statManager.range * scaleRatio;
      scaleZ = rangeCollider.transform.localScale.y;
    }

    transform.localScale = new Vector3(statManager.range * scaleRatio, scaleY, scaleZ);
  }

  public void subscribeToRange(TurretController turretController)
  {
    turretControllers.Add(turretController);
  }

  void OnTriggerEnter(Collider collider)
  {
    // AskForSnapshot();
    EnemyController enemy = collider.gameObject.GetComponentInParent<EnemyController>();

    if (enemy && !collider.isTrigger)
    {
      AnimatorNavAgentRootMotion enemyBody = collider.gameObject.GetComponentInParent<AnimatorNavAgentRootMotion> ();
      if (enemyBody)
      {
        foreach (TurretController turretController in turretControllers)
        {
          if (!turretController.enemiesInRange.Contains(enemyBody.gameObject))
          {
            turretController.enemiesInRange.Add(enemyBody.gameObject);
            turretController.targetUpdateWanted = true;
          }
        }
      }
    }
  }

  void OnTriggerExit(Collider collider)
  {
    // AskForSnapshot();
    EnemyController enemy = collider.gameObject.GetComponentInParent<EnemyController>();

    if (enemy && !collider.isTrigger)
    {
      AnimatorNavAgentRootMotion enemyBody = collider.gameObject.GetComponentInParent<AnimatorNavAgentRootMotion> ();
      if (enemyBody)
      {
        foreach (TurretController turretController in turretControllers)
        {
          if (turretController.enemiesInRange.Contains(enemyBody.gameObject))
          {
            turretController.enemiesInRange.Remove(enemyBody.gameObject);
            turretController.targetUpdateWanted = true;
          }
        }
      }
    }
  }

  void AskForSnapshot()
  {
    foreach (TurretController turretController in turretControllers)
    {
      turretController.targetUpdateWanted = true;
      turretController.snapshotWanted = true;
    }
  }
}
