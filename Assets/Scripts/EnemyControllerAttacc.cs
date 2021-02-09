using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyControllerAttacc : EnemyController
{
  public float damage = 20;
  public float range = 20;
  public GameObject rangeObject;
  float lastRange;
  public TriggerCheck triggerCheck;
  bool fireControl = false;
  public FireMomentListener fireMomentListener;
  GameObject target;
  bool attacking = false;
  string attackName = "Attack";
  BaseMoveAttacc moveScript;
  float walkBaseDistancePerSecond = 2.8125f;
  float runBaseDistancePerSecond = 8.48f;


  void Start()
  {
    Init();
    spawnAnimationName = "Spawn";
    moveScript = GetComponent<BaseMoveAttacc>();
    moveScript.walkBaseDistancePerSecond = walkBaseDistancePerSecond;
    moveScript.runBaseDistancePerSecond = runBaseDistancePerSecond;
    MoveTo(objective.position);
  }

  // Update is called once per frame
  void Update()
  {
    if (!hasDied && lastRange != range)
    {
      lastRange = range;
      rangeObject.transform.localScale = new Vector3(range, range, range);
    }

    if (anim.GetCurrentAnimatorStateInfo(0).IsName(spawnAnimationName) && !hasSpawned) {
      SpawnAnimationProcess();
    }
    else if (spawning)
    {
      PostSpawnAnimationProcess();
      if (isRunner)
      {
        moveScript.moveSpeed = runBaseDistancePerSecond;
      }
      else
      {
        moveScript.moveSpeed = walkBaseDistancePerSecond;
      }
    }

    if (triggerCheck.triggered)
    {
      Attack();
    }
    else
    {
      MoveTo(objective.position);
    }

    if (HasTouchedObjective()) {
      TouchedObjectiveProcess();
    }

    if (health.isDead && !hasDied && hasSpawned)
    {
      DyingProcess();
      rangeObject.SetActive(false);
    }
  }
  void Attack () {
    if (moveScript) {
      if (!hasDied) {
        moveScript.hasNavigationTarget = true;
        moveScript.navigationTargetMovable = triggerCheck.GetClosestObject().transform;
        moveScript.isNavigationTargetMovable = true;
      }
    }
  }
  void MoveTo (Vector3 point) {
    if (moveScript) {
      if (!hasDied) {
        moveScript.hasNavigationTarget = true;
        moveScript.navigationTarget = point;
        moveScript.isNavigationTargetMovable = false;
      }
    }
  }
}