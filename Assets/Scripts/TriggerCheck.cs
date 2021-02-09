using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCheck : MonoBehaviour
{
  public bool useTagName = false;
  public string tagNameToCheck;
  public List<string> tagNamesToIgnore = new List<string>();
  public bool triggered = false;
  GameObject closestObject = null;
  float closestObjectDistance = -1;
  List<GameObject> gameObjects = new List<GameObject>();

  void OnTriggerEnter(Collider collider)
  {
    bool invalidObject = false;
    GameObject objectHit = collider.gameObject;

    foreach (string tagNameToIgnore in tagNamesToIgnore)
    {
      if (objectHit.tag == tagNameToIgnore)
      {
        invalidObject = true;
      }
    }
    if (useTagName)
    {
      GameObject objectHead = Tools.FindObjectOrParentWithTag(objectHit, tagNameToCheck);
      if (objectHead)
      {
        SpellController spellController = objectHead.GetComponent<SpellController>();
        if (spellController)
        {
          objectHit = spellController.body;
        }
        else
        {
          invalidObject = true;
        }
      }
      else
      {
        invalidObject = true;
      }
    }
    if (!invalidObject && !gameObjects.Contains(objectHit))
    {
      gameObjects.Add(objectHit);
      triggered = true;
    }
    else
    {
      if (gameObjects.Count == 0)
      {
        triggered = false;
        closestObject = null;
        closestObjectDistance = -1;
      }
    }
  }

  void OnTriggerExit(Collider collider)
  {
    bool invalidObject = false;
    GameObject objectHit = collider.gameObject;

    foreach (string tagNameToIgnore in tagNamesToIgnore)
    {
      if (objectHit.tag == tagNameToIgnore)
      {
        invalidObject = true;
      }
    }
    if (useTagName)
    {
      GameObject objectHead = Tools.FindObjectOrParentWithTag(objectHit, tagNameToCheck);
      if (objectHead)
      {
        SpellController spellController = objectHead.GetComponent<SpellController>();
        if (spellController)
        {
          objectHit = spellController.body;
        }
      }
      else
      {
        invalidObject = true;
      }
    }
    if (!invalidObject && gameObjects.Contains(objectHit))
    {
      gameObjects.Remove(objectHit);
      if (gameObjects.Count == 0)
      {
        triggered = false;
        closestObject = null;
        closestObjectDistance = -1;
      }
    }
  }

  public GameObject GetClosestObject()
  {
    foreach (GameObject currentGameObject in gameObjects)
    {
      float currentObjectDistance = Vector3.Distance(currentGameObject.transform.position, transform.position);
      if (closestObjectDistance == -1 || currentObjectDistance < closestObjectDistance)
      {
        closestObject = currentGameObject;
        closestObjectDistance = currentObjectDistance;
      }
    }
    if (closestObject)
    {
      return closestObject;
    } else return null;
  }
}
