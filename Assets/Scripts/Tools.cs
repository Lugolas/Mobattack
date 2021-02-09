using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tools : MonoBehaviour
{
  [ColorUsageAttribute(true, true)]
  public Color white;
  [ColorUsageAttribute(true, true)]
  public Color green;
  [ColorUsageAttribute(true, true)]
  public Color red;

  public LayerMask enemyDetectionMask;
  public LayerMask enemyPartsDetectionMask;

  public static Color GetWhite() {
    return FindTools().white;
  }

  public static Color GetGreen() {
    return FindTools().green;
  }

  public static Color GetRed() {
    return FindTools().red;
  }

  public static LayerMask GetEnemyDetectionMask() {
    return FindTools().enemyDetectionMask;
  }

  public static LayerMask GetEnemyPartsDetectionMask() {
    return FindTools().enemyPartsDetectionMask;
  }

  private static Tools FindTools() {
    GameObject toolsObject = GameObject.Find("Tools");
    Tools tools = null;
    if (toolsObject) {
      tools = toolsObject.GetComponent<Tools>();
    }
    return tools; 
  }

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

  public static bool InflictDamage(Transform targetedEnemy, int damageAmount, MoneyManager moneyManager, GameObject attacker)
  {
    HealthSimple hs = targetedEnemy.GetComponent<HealthSimple>();
    if (!hs) {
      hs = targetedEnemy.GetComponentInChildren<HealthSimple>();
    }
    if (!hs) {
      hs = targetedEnemy.GetComponentInParent<HealthSimple>();
    }
    if (hs)
    {
      if (hs.TakeDamage(damageAmount, attacker))
      {
        moneyManager.AddMoney(hs.moneyToReward);
        return true;
      }
    } else {
      Debug.Log("No Health Script");
    }
    return false;
  }
}