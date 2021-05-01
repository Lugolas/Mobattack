using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfController : SpellController
{
  public int id;
  public MoneyManager moneyManager;
  BaseMoveAttacc moveScript;
  HealthSimple healthScript;
  bool createModeOn = false;
  Vector3 turretCreationPoint;
  GameObject previewTurret;
  TurretPlayerLink previewTurretPlayerLink;
  GameObject enemiesManager;
  bool moveClickDown = false;
  Vector3 moveClickPosition;
  GameObject attackTarget;
  bool isInAttackStance = false;
  public Animator animator;
  TogglableRagdollController[] togglableRagdollControllers;
  List<Rigidbody> armsRigidbodies = new List<Rigidbody>();
  bool attackTriggered = false;
  bool previewTurretNeedsOrientation = false;
  bool previewTurretPlaced = false;
  bool attacking = false;
  float walkBaseDistancePerSecond = 1.6f;
  float runBaseDistancePerSecond = 6.4f;
  public float turretRange = 10f;
  public int gunDamage = 10;
  public float delay = 1f;
  public TurretWolfDenController den;
  public Vector3 waitPoint = Vector3.zero;
  public bool readyToFight = false;
  public TurretStatManager statManager;
  bool attackBegan = false;

  // Start is called before the first frame update
  void Start()
  {
    togglableRagdollControllers = GetComponentsInChildren<TogglableRagdollController>();
    moveScript = GetComponent<BaseMoveAttacc>();

    if (moveScript) {
      moveScript.walkBaseDistancePerSecond = walkBaseDistancePerSecond;
      moveScript.runBaseDistancePerSecond = runBaseDistancePerSecond;

      moveScript.moneyManager = moneyManager;
    }

    healthScript = GetComponent<HealthSimple>();

    enemiesManager = GameObject.Find("EnemiesManager");

    if (animator)
    {
      body = animator.gameObject;
    }
  }

  // Update is called once per frame
  void Update()
  {
    if (den) {
      attackTarget = den.target;
      gunDamage = statManager.damage;
      healthScript.damageBase = gunDamage;
      moveScript.timeBetweenShots = delay;
      delay = statManager.delay;
    }

    if (!attackTarget) {
      attackTarget = null;
    }

    if (waitPoint != Vector3.zero && (!readyToFight || (readyToFight && !attackTarget))) {
      WalkTo(waitPoint);
    }

    if (!readyToFight && waitPoint != Vector3.zero && Vector3.Distance(waitPoint, body.transform.position) < 1) {
      readyToFight = true;
    }

    if (readyToFight && attackTarget && ((id == 0) || (Time.time >= den.alphaWolfFireTime + (0.15 * id)))) {
      Attack();
    }

    if (moveScript.attacking && !attackBegan)
    {
      attackBegan = true;
      if (id == 0)
      {
        den.alphaWolfFireTime = Time.time;
      }
    }
    else if (attackBegan)
    {
      attackBegan = false;
    }
  }

  void Attack () {
    if (moveScript && healthScript) {
      if (!healthScript.isDead) {
        moveScript.moveSpeed = runBaseDistancePerSecond;
        moveScript.hasNavigationTarget = true;
        moveScript.navigationTargetMovable = attackTarget.transform;
        moveScript.isNavigationTargetMovable = true;
      }
    }
  }

  void WalkTo (Vector3 point) {
    if (moveScript && healthScript) {
      if (!healthScript.isDead) {
        moveScript.moveSpeed = walkBaseDistancePerSecond;
        moveScript.hasNavigationTarget = true;
        moveScript.navigationTarget = point;
        moveScript.isNavigationTargetMovable = false;
      }
    }
  }
}