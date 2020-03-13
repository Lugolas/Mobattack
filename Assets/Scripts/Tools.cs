using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tools : MonoBehaviour
{
  public static GameObject FindObjectOrParentWithTag(GameObject childObject, string tag)
  {
    Transform t = childObject.transform;
    if (t.tag == tag)
    {
      return t.gameObject;
    }
    else
    {
      while (t.parent != null)
      {
        if (t.parent.tag == tag)
        {
          return t.parent.gameObject;
        }
        t = t.parent.transform;
      }
    }
    return null; // Could not find a parent with given tag.
  }

  static Transform GetClosestTarget(Transform source, Transform[] targets)
  {
    Transform bestTarget = null;
    float closestDistanceSqr = Mathf.Infinity;
    Vector3 currentPosition = source.position;
    foreach (Transform potentialTarget in targets)
    {
      Vector3 directionToTarget = potentialTarget.position - currentPosition;
      float dSqrToTarget = directionToTarget.sqrMagnitude;
      if (dSqrToTarget < closestDistanceSqr)
      {
        closestDistanceSqr = dSqrToTarget;
        bestTarget = potentialTarget;
      }
    }

    return bestTarget;
  }

}