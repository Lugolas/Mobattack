using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurateShootController : MonoBehaviour
{
  TurateShootPoint[] shootPoints;
  public GameObject target;
  bool fireballsBehaviour;
  bool HEAL = true;
  bool HURT = false;
  public int team;
  private List<GameObject> alreadyShotAt = new List<GameObject>();
  public bool collapseTrigger = false;
  bool hasCollapsed = false;
  public GameObject[] parts;
  GameController gameController;

  // Start is called before the first frame update
  void Start()
  {
    shootPoints = GetComponentsInChildren<TurateShootPoint>();
    Physics.IgnoreLayerCollision(9, 16, true);
    Physics.IgnoreLayerCollision(10, 16, true);
    gameController = Tools.getGameController();
  }

  // Update is called once per frame
  void Update()
  {
    if (gameController)
    {
      if (team == 1)
      {
        if (gameController.team2ScoredLimit)
        {
          collapseTrigger = true;
        }
      }
      else
      {
        if (gameController.team1ScoredLimit)
        {
          collapseTrigger = true;
        }
      }
    }
    else
    {
      gameController = Tools.getGameController();
    }

    if (collapseTrigger && !hasCollapsed)
    {
      collapse();
    }
  }

  void collapse()
  {
    hasCollapsed = true;
    collapseTrigger = false;

    MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
    meshRenderer.enabled = false;

    BoxCollider boxCollider = GetComponent<BoxCollider>();
    boxCollider.enabled = false;

    foreach (GameObject part in parts)
    {
      part.SetActive(true);
    }
  }

  void OnTriggerEnter(Collider collider)
  {
    if (Tools.FindObjectOrParentWithTag(collider.gameObject, "Character"))
    {
      if (!alreadyShotAt.Contains(collider.gameObject))
      {
        target = collider.gameObject;
        alreadyShotAt.Add(target);
        CharacterManager targetManager = target.GetComponent<CharacterManager>();

        if (targetManager && targetManager.team == team)
        {
          fireballsBehaviour = HEAL;
        }
        else
        {
          fireballsBehaviour = HURT;
        }

        foreach (TurateShootPoint shootPoint in shootPoints)
        {
          // shootPoint.target = target;
          shootPoint.fire(target, fireballsBehaviour);
        }
      }
    }
  }

  void OnTriggerExit(Collider collider)
  {
    if (Tools.FindObjectOrParentWithTag(collider.gameObject, "Character"))
    {
      alreadyShotAt.Remove(collider.gameObject);
    }
  }
}