using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpellControllerAfro : SpellController {
  public MoneyManager moneyManager;
  BaseMoveAttacc moveScript;
  public HealthDamage healthScript;
  bool createModeOn = false;
  public LayerMask layerMaskGround;
  public LayerMask layerMaskMove;
  public GameObject turret1Prefab;
  public GameObject turret2Prefab;
  Vector3 turretCreationPoint;
  GameObject previewTurret;
  TurretPlayerLink previewTurretPlayerLink;
  GameObject enemiesManager;
  bool moveClickDown = false;
  public GameObject targetPointerPrefab;
  Vector3 moveClickPosition;
  GameObject attackTarget;
  bool isInAttackStance = false;
  bool isInBreakerUlt = false;
  public Animator animator;
  public NavMeshAgent navAgent;
  public GameObject armLeft;
  public GameObject armLeft2;
  public GameObject armLeft3;
  public GameObject armRight;
  public GameObject armRight2;
  public GameObject armRight3;
  TogglableRagdollController[] togglableRagdollControllers;
  List<Rigidbody> armsRigidbodies = new List<Rigidbody> ();
  bool attackTriggered = false;
  public float spell3AttackSpeedMultiplier = 1f;
  bool previewTurretNeedsOrientation = false;
  bool previewTurretPlaced = false;
  bool attacking = false;
  bool previewTurret1 = false;
  bool previewTurret2 = false;

  float walkBaseDistancePerSecond = 2.4545f;
  float runBaseDistancePerSecond = 9f;

  public bool pairPunch = false;
  public AfroHandController handL;
  public AfroHandController handL2;
  public AfroHandController handL3;
  public AfroHandController handR;
  public AfroHandController handR2;
  public AfroHandController handR3;
  public GameObject spellsUIPrefab;

  bool afroBallClickDown = false;
  GameObject afroBallClicked;
  GameObject lastAfroBallClicked;
  public GameObject arrowPrefab;
  GameObject currentArrow;
  ArrowStretchController currentArrowPoint;
  Vector3 lastArrowPoint;
  TurretStatManager afroBallClickedStats;
  Vector3 afroBallLaunchPosition;
  bool afroBallLaunchPlanned = false;

  public bool armLeft2Active = false;
  bool armLeft2ActiveCheck = true;
  public bool armLeft3Active = false;
  bool armLeft3ActiveCheck = true;
  public bool armRight2Active = false;
  bool armRight2ActiveCheck = true;
  public bool armRight3Active = false;
  bool armRight3ActiveCheck = true;
  bool spinning = false;
  bool spawning = false;
  bool spawned = false;
  bool hasDied = false;
  public float respawnDelay = 5;
  float timeOfDeath;
  public FireMomentListener armRagdollListener;
  bool armRagdollState;
  Vector3 spawnPoint;
  int manaReceivedOnKill = 10;

  protected int turret1Price;
  protected int turret2Price;

  public PhysicMaterial fistMaterial;
  public bool rageArmor = false;
  public bool rageDamage = false;
  public bool rageHealthRegenPerSecond = false;
  public bool speedAffectsFists = false;
  public bool fistBounceUp = false;
  bool fistBounceUpControl = true;
  AnimatorNavAgentRootMotion rootMotion;
  float defaultSpeed;
  float defaultAcceleration;
  float defaultAngularSpeed;
  float defaultStoppingDistance;
  bool defaultAutoBraking;

  // Start is called before the first frame update
  void Start () {
    spawnPoint = GameObject.Find("Spawn").transform.position;
    togglableRagdollControllers = GetComponentsInChildren<TogglableRagdollController> ();
    moneyManager = GetComponent<MoneyManager> ();
    moveScript = GetComponent<BaseMoveAttacc> ();
    rootMotion = GetComponentInChildren<AnimatorNavAgentRootMotion>();

    if (moveScript) {
      moveScript.walkBaseDistancePerSecond = walkBaseDistancePerSecond;
      moveScript.runBaseDistancePerSecond = runBaseDistancePerSecond;
    }

    healthScript = GetComponent<HealthDamage> ();

    enemiesManager = GameObject.Find ("EnemiesManager");

    if (armLeft) {
      Rigidbody[] armLeftRigidBodies = armLeft.GetComponentsInChildren<Rigidbody> ();
      foreach (Rigidbody armLeftRigidBody in armLeftRigidBodies) {
        armsRigidbodies.Add (armLeftRigidBody);
      }
    }

    if (armLeft2) {
      Rigidbody[] armLeft2RigidBodies = armLeft2.GetComponentsInChildren<Rigidbody> ();
      foreach (Rigidbody armLeft2RigidBody in armLeft2RigidBodies) {
        armsRigidbodies.Add (armLeft2RigidBody);
      }
    }

    if (armLeft3) {
      Rigidbody[] armLeft3RigidBodies = armLeft3.GetComponentsInChildren<Rigidbody> ();
      foreach (Rigidbody armLeft3RigidBody in armLeft3RigidBodies) {
        armsRigidbodies.Add (armLeft3RigidBody);
      }
    }

    if (armRight) {
      Rigidbody[] armRightRigidBodies = armRight.GetComponentsInChildren<Rigidbody> ();
      foreach (Rigidbody armRightRigidBody in armRightRigidBodies) {
        armsRigidbodies.Add (armRightRigidBody);
      }
    }

    if (armRight2) {
      Rigidbody[] armRight2RigidBodies = armRight2.GetComponentsInChildren<Rigidbody> ();
      foreach (Rigidbody armRight2RigidBody in armRight2RigidBodies) {
        armsRigidbodies.Add (armRight2RigidBody);
      }
    }

    if (armRight3) {
      Rigidbody[] armRight3RigidBodies = armRight3.GetComponentsInChildren<Rigidbody> ();
      foreach (Rigidbody armRight3RigidBody in armRight3RigidBodies) {
        armsRigidbodies.Add (armRight3RigidBody);
      }
    }

    if (animator) {
      body = animator.gameObject;
      animator.SetTrigger("Spawn");
      navAgent = body.GetComponent<NavMeshAgent> ();
      defaultSpeed = navAgent.speed;
      defaultAcceleration = navAgent.acceleration;
      defaultAngularSpeed = navAgent.angularSpeed;
      defaultStoppingDistance = navAgent.stoppingDistance;
      defaultAutoBraking = navAgent.autoBraking;
    }

    turret1Price = turret1Prefab.GetComponentInChildren<TurretUpgradeManager> ().baseTurret.GetComponent<TurretStatManager> ().price;
    turret2Price = turret2Prefab.GetComponentInChildren<TurretUpgradeManager> ().baseTurret.GetComponent<TurretStatManager> ().price;

    GameObject canvas = GameObject.Find ("Canvas");
    GameObject spells = canvas.transform.Find ("Spells").gameObject;
    GameObject spellsUI = Instantiate (spellsUIPrefab);
    spellsUI.transform.SetParent (spells.transform);
    RectTransform spellsUiTransform = spellsUI.GetComponent<RectTransform> ();
    spellsUiTransform.localPosition = Vector3.zero;
    SpellUIController spellsUIScript = spellsUI.GetComponent<SpellUIController> ();
    if (spellsUIScript) {
      spellsUIScript.spellController = this;
    }

    armRagdollState = !armRagdollListener.timeToFire;
  }

  // Update is called once per frame
  void Update () {
    if (fistBounceUp && !fistBounceUpControl) {
      fistBounceUpControl = true;
      fistMaterial.bounciness = 1;
      fistMaterial.staticFriction = 0;
      fistMaterial.dynamicFriction = 0;
    } else if (!fistBounceUp && fistBounceUpControl) {
      fistBounceUpControl = false;
      fistMaterial.bounciness = 0.333f;
      fistMaterial.staticFriction = 0.333f;
      fistMaterial.dynamicFriction = 0.333f;
    }
    if (rageArmor) {
      healthScript.AddArmorAddition(healthScript.currentMana, "rageArmor");
    }
    if (rageHealthRegenPerSecond) {
      healthScript.AddHealthRegenPerSecondAddition(healthScript.currentMana, "rageHealthRegenPerSecond");
    }
    if (rageDamage) {
      healthScript.AddDamageMultiplier(((float) healthScript.currentMana / healthScript.maxMana) + 1, "rageDamage");
    }
    if (healthScript.isDead) {
      if (!hasDied) {
        // DIE
        animator.SetTrigger("Die");
        AttackStanceState (false);
        CancelCreateMode();
        hasDied = true;
        timeOfDeath = Time.time;
        spawned = false;
      } else {
        // SPAWN
        if (Time.time > timeOfDeath + respawnDelay) {
          healthScript.isDead = false;
          hasDied = false;
          spawned = false;
          navAgent.Warp(spawnPoint);
          animator.SetTrigger("Spawn");
        }
      }
    } else {
      if (createModeOn && previewTurret) {
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll (ray, 2500, layerMaskGround);
        if (hits.Length > 0) {
          System.Array.Sort (hits, (x, y) => x.distance.CompareTo (y.distance));
          foreach (RaycastHit hit in hits) {
            if (!previewTurretPlaced) {
              previewTurret.transform.position = hit.point;
            } else {
              previewTurret.transform.LookAt (new Vector3 (hit.point.x, previewTurret.transform.position.y, hit.point.z));
            }
            break;
          }
        }
      }

      if (isInAttackStance && moveScript.hasNavigationTarget) {
        moveScript.stopMoving ();
      }

      if (isInAttackStance) {
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
        RaycastHit hit;
        Physics.Raycast (ray, out hit, 2500, layerMaskMove);
        body.transform.LookAt (new Vector3 (hit.point.x, body.transform.position.y, hit.point.z));
        animator.SetBool ("Attacking", attacking);
        animator.SetFloat ("Spell3Speed", spell3AttackSpeedMultiplier);
      }

      if (animator.GetCurrentAnimatorStateInfo (0).IsName ("Shoot")) {
        attackTriggered = false;
      }

      if (afroBallClickDown && currentArrowPoint) {
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
        RaycastHit hit;
        Physics.Raycast (ray, out hit, 2500, layerMaskMove);
        currentArrow.transform.LookAt (new Vector3 (hit.point.x, currentArrow.transform.position.y, hit.point.z));
        currentArrowPoint.transform.position = new Vector3 (hit.point.x, currentArrowPoint.transform.position.y, hit.point.z);
      }

      if (afroBallLaunchPlanned) {
        if (Vector3.Distance (body.transform.position, afroBallLaunchPosition) <= navAgent.stoppingDistance) {
          moveScript.targetedEnemy = lastAfroBallClicked.transform;
          // moveScript.attacking = true;
          body.transform.LookAt (lastArrowPoint);
          moveScript.Fire ();
          afroBallLaunchPlanned = false;
        }
      }
    }

    if (moneyManager.GetMoney () >= turret1Price && !spell3Active) {
      spell1Available = true;
    } else {
      spell1Available = false;
    }
    if (moneyManager.GetMoney () >= turret2Price && !spell3Active) {
      spell2Available = true;
    } else {
      spell2Available = false;
    }
    if (!spell1Active && !spell2Active) {
      spell3Available = true;
    } else {
      spell3Available = false;
    }

    if (armLeft2Active != armLeft2ActiveCheck) {
      armLeft2ActiveCheck = armLeft2Active;
      ArmSetActive(armLeft2Active, handL2);
    }
    if (armLeft3Active != armLeft3ActiveCheck) {
      armLeft3ActiveCheck = armLeft3Active;
      ArmSetActive(armLeft3Active, handL3);
    }
    if (armRight2Active != armRight2ActiveCheck) {
      armRight2ActiveCheck = armRight2Active;
      ArmSetActive(armRight2Active, handR2);
    }
    if (armRight3Active != armRight3ActiveCheck) {
      armRight3ActiveCheck = armRight3Active;
      ArmSetActive(armRight3Active, handR3);
    }
  }

  void LateUpdate () {
    if (handR.punchAttempted && !pairPunch) {
      pairPunch = true;
      animator.SetBool ("PairPunch", pairPunch);
    } else
    if (handL.punchAttempted && pairPunch) {
      pairPunch = false;
      animator.SetBool ("PairPunch", pairPunch);
    }

    if (animator.GetCurrentAnimatorStateInfo(0).IsName("Spawn")) {
      spawning = true;
    }
    if (spawning) {
      spawning = false;
      spawned = true;
      hasDied = false;
    }

    if (animator.GetCurrentAnimatorStateInfo(0).IsName("Breaker")) {
      if (!spinning) {
        spinning = true;
        BreakerLoopState(true);
      }
    } else if (spinning) {
      spinning = false;
      BreakerLoopState(false);
    }

    if (armRagdollListener.timeToFire && !armRagdollState)
    {
      ArmRagdollState(true);
    }
    if (!armRagdollListener.timeToFire && armRagdollState) {
      ArmRagdollState(false);
    }
  }

  override public void Spell1 () {
    if (!hasDied && spawned) 
    {
      AttackStanceState (false);
      if (!spell1Active) {
        if (createModeOn) {
          CancelCreateMode ();
        }
        if (spell1Available) {
          spell1Active = true;
          Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
          RaycastHit[] hits = Physics.RaycastAll (ray, 2500, layerMaskGround);

          if (hits.Length > 0) {
            System.Array.Sort (hits, (x, y) => x.distance.CompareTo (y.distance));
            foreach (RaycastHit hit in hits) {
              turretCreationPoint = hit.point;
              previewTurret = Instantiate (turret1Prefab, turretCreationPoint, new Quaternion ());
              previewTurretPlayerLink = previewTurret.GetComponentInChildren<TurretPlayerLink> ();
              previewTurretPlayerLink.InitialLink (gameObject, moneyManager);
              createModeOn = true;
              previewTurretNeedsOrientation = true;
              previewTurret1 = true;
              break;
            }
          }
        }
      } else {
        CancelCreateMode ();
      }
    }
  }

  override public void Spell2 () {
    if (!hasDied && spawned)
    {
      AttackStanceState (false);
      if (!spell2Active) {
        if (createModeOn) {
          CancelCreateMode ();
        }
        if (spell2Available) {
          spell2Active = true;
          Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
          RaycastHit[] hits = Physics.RaycastAll (ray, 2500, layerMaskGround);

          if (hits.Length > 0) {
            System.Array.Sort (hits, (x, y) => x.distance.CompareTo (y.distance));
            foreach (RaycastHit hit in hits) {
              turretCreationPoint = hit.point;
              previewTurret = Instantiate (turret2Prefab, turretCreationPoint, new Quaternion ());
              previewTurretPlayerLink = previewTurret.GetComponentInChildren<TurretPlayerLink> ();
              previewTurretPlayerLink.InitialLink (gameObject, moneyManager);
              createModeOn = true;
              previewTurretNeedsOrientation = true;
              previewTurret2 = true;
              break;
            }
          }
        }
      } else {
        CancelCreateMode ();
      }
    }
  }

  public override void Spell3 () {
    if (!hasDied && spawned) 
    {
      afroBallLaunchPlanned = false;
      if (createModeOn) {
        CancelCreateMode();
      } else {
        AttackStanceState(!isInAttackStance);
      }
    }
  }

  public override void Spell4 () {
    if (!hasDied && spawned) 
    {
      afroBallLaunchPlanned = false;
      if (createModeOn) {
        CancelCreateMode();
      } else {
        BreakerUltState(!isInBreakerUlt);
      }
    }
  }

  void AttackStanceState (bool state) {
    if (spawned)
    {
      isInAttackStance = state;
      spell3Active = state;

      // ArmRagdollState(!state);

      animator.SetBool("AttackStance", state);
    }
  }

  void BreakerUltState (bool state) {
    if (spawned)
    {
      isInBreakerUlt = state;
      spell4Active = state;

      animator.SetBool("BreakerUlt", state);
    }
  }

  public bool IsInBreakerUlt() {
    return isInBreakerUlt;
  }

  void BreakerLoopState(bool state) {
    SetHandTrail(handL, state);
    SetHandTrail(handL2, state);
    SetHandTrail(handL3, state);
    SetHandTrail(handR, state);
    SetHandTrail(handR2, state);
    SetHandTrail(handR3, state);

    rootMotion.active = !state;

    float speed;
    float acceleration;
    float angularSpeed;
    float stoppingDistance;
    bool autoBraking;

    if (state) {
      speed = 15;
      acceleration = 5;
      angularSpeed = 10;
      stoppingDistance = 0;
      autoBraking = false;
    } else {
      speed = defaultSpeed;
      acceleration = defaultAcceleration;
      angularSpeed = defaultAngularSpeed;
      stoppingDistance = defaultStoppingDistance;
      autoBraking = defaultAutoBraking;
    }

    navAgent.speed = speed;
    navAgent.acceleration = acceleration;
    navAgent.angularSpeed = angularSpeed;
    navAgent.stoppingDistance = stoppingDistance;
    navAgent.autoBraking = autoBraking;
  }

  void SetHandTrail(AfroHandController hand, bool state) {
    if (hand.fist.gameObject.activeSelf) {
      hand.fist.trail.emitting = state;
      // hand.fist.trailIN.emitting = state;
      // hand.fist.trailOUT.emitting = state;
    }
  }

  void ArmRagdollState (bool state) {
    foreach (Rigidbody armsRigidbody in armsRigidbodies) {
      if (state) {
        // RAGDOLLED
        armsRigidbody.useGravity = true;
        armsRigidbody.isKinematic = false;
      } else {
        // ANIMATED
        armsRigidbody.useGravity = false;
        armsRigidbody.isKinematic = true;
      }
    }

    foreach (TogglableRagdollController togglableRagdollController in togglableRagdollControllers) {
      togglableRagdollController.ragdoll = state;
    }

    armRagdollState = state;
  }

  void CancelCreateMode () {
    spell1Active = false;
    spell2Active = false;
    createModeOn = false;
    Destroy (previewTurret);
    previewTurret = null;
    previewTurretPlayerLink = null;
    previewTurretPlaced = false;
    previewTurret1 = false;
    previewTurret2 = false;
  }

  void EnemiesRefreshPathfinding () {
    EnemyController[] enemies = enemiesManager.GetComponentsInChildren<EnemyController> ();
    foreach (EnemyController enemy in enemies) {
      enemy.refreshDestination ();
    }
  }

  public override bool Fire1 (bool down) {
    bool returnValue = false;
    if (down && spawned) {
      if (previewTurret) {
        returnValue = true;
      }
      if (previewTurret && (previewTurretPlayerLink.HasEnoughSpace () || previewTurretNeedsOrientation) &&
        previewTurretPlayerLink.characterWallet.GetMoney () >= previewTurret.GetComponentInChildren<TurretStatManager> ().price &&
        createModeOn) {

        if (previewTurretNeedsOrientation) {
          previewTurretPlaced = true;
        } else {
          createModeOn = false;

          previewTurretPlayerLink.Activate ();
          previewTurret = null;
          previewTurret1 = false;
          previewTurret2 = false;

          EnemiesRefreshPathfinding ();
        }
      }

      if (isInAttackStance) {
        attacking = true;
        returnValue = true;
      }
    } else {
      if (
        spawned &&
        previewTurretNeedsOrientation && 
        previewTurretPlayerLink && 
        previewTurretPlayerLink.HasEnoughSpace() &&
        previewTurretPlayerLink.characterWallet.GetMoney() >= previewTurret.GetComponentInChildren<TurretStatManager>().price &&
        createModeOn
      ) {
        previewTurretPlaced = false;
        previewTurretNeedsOrientation = false;
        createModeOn = false;
        spell1Active = false;
        spell2Active = false;
        previewTurret1 = false;
        previewTurret2 = false;

        previewTurretPlayerLink.Activate ();

        TurretAfroBallController afroBallController = previewTurret.GetComponentInChildren<TurretAfroBallController> ();
        if (afroBallController) {
          afroBallController.spellController = this;
        }

        previewTurret = null;

        EnemiesRefreshPathfinding ();
      }
      if (isInAttackStance) {
        attacking = false;
      }
    }
    return returnValue;
  }

  public override void Fire2 (bool down) {
    if (createModeOn) {
      CancelCreateMode ();
    }
    AttackStanceState (false);

    if (down) {
      Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
      RaycastHit[] hits = Physics.RaycastAll (ray, 2500, layerMaskMove);
      if (hits.Length > 0) {
        System.Array.Sort (hits, (x, y) => x.distance.CompareTo (y.distance));
        foreach (RaycastHit hit in hits) {
          if (hit.collider.gameObject != gameObject) {
            EnemyController enemyHit = hit.collider.gameObject.GetComponentInParent<EnemyController> ();
            if (enemyHit) {
              attackTarget = hit.transform.gameObject;
              HealthSimple targetInfo = attackTarget.GetComponentInParent<HealthSimple> ();
              if (targetInfo && !afroBallClickDown) {
                afroBallLaunchPlanned = false;
                Attack ();
              }
              break;
            } else {
              TurretAfroBallController afroBallHit = hit.collider.gameObject.GetComponentInParent<TurretAfroBallController> ();
              if (afroBallHit && spawned) {
                if (!moveClickDown && !afroBallClickDown) {
                  TurretAfroBallController afroBallController = afroBallHit.GetComponentInChildren<TurretAfroBallController> ();
                  if (afroBallController.activated && !afroBallController.launched) {
                    afroBallClicked = afroBallController.gameObject;
                    afroBallClickedStats = afroBallClicked.GetComponentInParent<TurretStatManager> ();
                    currentArrow = Instantiate (arrowPrefab, new Vector3 (
                      afroBallClicked.transform.position.x,
                      afroBallClicked.transform.position.y + 0.1f,
                      afroBallClicked.transform.position.z
                    ), body.transform.rotation);
                    currentArrowPoint = currentArrow.GetComponentInChildren<ArrowStretchController> ();
                    afroBallClickDown = true;
                    break;
                  }
                }
              } else {
                if (!afroBallClickDown && !Tools.FindObjectOrParentWithTag(hit.collider.gameObject, "PlayerCharacter")) {
                  moveClickDown = true;
                  moveClickPosition = new Vector3 (hit.point.x, hit.point.y + 0.1f, hit.point.z);
                  afroBallLaunchPlanned = false;
                  MoveTo (hit.point);
                  break;
                }
              }
            }
          }
        }
      }
    } else {
      if (afroBallClickDown) {
        afroBallClickDown = false;
        float ballX = afroBallClicked.transform.position.x;
        float ballZ = afroBallClicked.transform.position.z;
        float spaceToPointRatio = (
          (afroBallClickedStats.space + navAgent.stoppingDistance) /
          Vector3.Distance (currentArrowPoint.transform.position, afroBallClicked.transform.position)
        );
        float targetX = currentArrowPoint.transform.position.x;
        float targetZ = currentArrowPoint.transform.position.z;
        Vector3 movePosition = new Vector3 (
          ballX + (ballX - targetX) * spaceToPointRatio,
          body.transform.position.y,
          ballZ + (ballZ - targetZ) * spaceToPointRatio
        );
        Instantiate (targetPointerPrefab, movePosition, new Quaternion ());
        MoveTo (movePosition);
        lastArrowPoint = new Vector3 (currentArrowPoint.transform.position.x, body.transform.position.y, currentArrowPoint.transform.position.z);
        lastAfroBallClicked = afroBallClicked;
        Destroy (currentArrow);
        afroBallClicked = null;
        currentArrow = null;
        currentArrowPoint = null;
        afroBallLaunchPosition = movePosition;
        afroBallLaunchPlanned = true;
      }
      if (moveClickDown) {
        moveClickDown = false;
        Instantiate (targetPointerPrefab, moveClickPosition, new Quaternion ());
      }
    }
  }

  void Attack () {
    if (moveScript && healthScript && spawned) {
      if (!healthScript.isDead) {
        moveScript.hasNavigationTarget = true;
        moveScript.navigationTargetMovable = attackTarget.transform;
        moveScript.isNavigationTargetMovable = true;
      }
    }
  }

  void MoveTo (Vector3 point) {
    if (moveScript && healthScript && spawned) {
      if (!healthScript.isDead) {
        moveScript.hasNavigationTarget = true;
        moveScript.navigationTarget = point;
        moveScript.isNavigationTargetMovable = false;
      }
    }
  }

  void ArmSetActive(bool state, AfroHandController hand) {
    GameObject armObject = null;
    GameObject tempObject = hand.gameObject;
    bool objectHeadFound = false;
    while (!objectHeadFound) 
    {
      if (tempObject.GetComponent<TogglableRagdollController>()) {
        armObject = tempObject;
        objectHeadFound = true;
      } else {
        tempObject = tempObject.transform.parent.gameObject;
      }
    }
    GameObject armRagdoll = armObject.GetComponent<TogglableRagdollController>().ragdollArm;
    if (!isInAttackStance) {
      CapsuleCollider[] capsules = armRagdoll.GetComponentsInChildren<CapsuleCollider>();
      foreach (CapsuleCollider capsule in capsules)
      {
        capsule.isTrigger = !state;
      }
      SphereCollider[] spheres = armRagdoll.GetComponentsInChildren<SphereCollider>();
      foreach (SphereCollider sphere in spheres)
      {
        sphere.isTrigger = !state;
      }
    }
    GameObject armModel = hand.armModel;
    armModel.SetActive(state);
    hand.GetComponentInChildren<AfroFistController>(true).gameObject.SetActive(state);
    hand.enabled = state;
  }

  public void SpeedUpSpell3 () {
    spell3AttackSpeedMultiplier *= 1.05f;
  }

  public void FistKilledEnemy() {
    SpeedUpSpell3();
    healthScript.ReceiveMana(manaReceivedOnKill);
  }
}