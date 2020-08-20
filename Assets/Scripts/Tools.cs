using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tools : MonoBehaviour
{
  public static GameObject FindObjectOrParentWithTag(GameObject childObject, string tag)
  {
    Transform t = childObject.transform;
    if (tag == "Character")
    {
      if (t.tag == "PlayerCharacter" || t.tag == "TeamCharacter" || t.tag == "EnemyCharacter")
      {
        return t.gameObject;
      }
    }

    if (t.tag == tag)
    {
      return t.gameObject;
    }
    else
    {
      if (tag == "Character")
      {
        while (t.parent != null)
        {
          if (t.parent.tag == "PlayerCharacter" || t.parent.tag == "TeamCharacter" || t.parent.tag == "EnemyCharacter" || t.parent.tag == "Character")
          {
            return t.parent.gameObject;
          }
          t = t.parent.transform;
        }
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

  public static GameController getGameController()
  {
    GameObject gameManager = GameObject.Find("GameManager");
    GameController gameController = null;
    if (gameManager)
    {
      gameController = gameManager.GetComponent<GameController>();
    }
    return gameController;
  }

  public static void SetLayerRecursively(GameObject obj, int newLayer)
  {
    if (null == obj)
    {
      return;
    }

    if (obj.layer != 12)
      obj.layer = newLayer;

    foreach (Transform child in obj.transform)
    {
      if (null == child)
      {
        continue;
      }
      SetLayerRecursively(child.gameObject, newLayer);
    }
  }


}