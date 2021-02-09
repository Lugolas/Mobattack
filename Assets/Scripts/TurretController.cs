using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TurretController : MonoBehaviour
{
  public List<GameObject> enemiesInRange = new List<GameObject>();

  public GameObject target;
  public bool targetUpdateWanted = false;
  public bool snapshotWanted = false;
  public float lastTargetUpdate;

  public void UpdateTarget(float range)
  {
    if (snapshotWanted)
    {
      enemiesInRange.Clear();
      Collider[] objectsHit = Physics.OverlapSphere(transform.position, range, Tools.GetEnemyDetectionMask(), QueryTriggerInteraction.Collide);
      foreach (Collider objectHit in objectsHit)
      {
        HealthSimple health = objectHit.GetComponentInParent<HealthSimple>();
        if (health && !health.isDead)
        {
          enemiesInRange.Add(objectHit.gameObject);
        }
      }
    }
    else
    {
      List<int> invalidEnemiesIndexes = new List<int> ();

      for (int i = 0; i < enemiesInRange.Count; i++) {
        GameObject enemy = enemiesInRange[i];
        if (enemy)
        {
          HealthSimple health = enemy.GetComponentInParent<HealthSimple>();
          if (!health || health.isDead) {
            invalidEnemiesIndexes.Add (i);
          }
        }
        else
        {
          invalidEnemiesIndexes.Add (i);
        }
      }

      foreach (int index in invalidEnemiesIndexes) {
        if (index < enemiesInRange.Count) {
          enemiesInRange.RemoveAt (index);
        }
      }
    }

    if (enemiesInRange.Count > 0) {
      target = enemiesInRange[0];
    } else {
      target = null;
    }
    snapshotWanted = false;
    targetUpdateWanted = false;
    lastTargetUpdate = Time.time;
  }
}
