using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpellControllerAfro : SpellController
{
  MoneyManager moneyManager;
  BaseMoveAttacc moveScript;
  HealthDamage healthScript;
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
  public Animator animator;
  public NavMeshAgent navAgent;
  public GameObject armLeft;
  public GameObject armRight;
  TogglableRagdollController[] togglableRagdollControllers;
  List<Rigidbody> armsRigidbodies = new List<Rigidbody>();
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
  public AfroHandController handR;
  public AfroHandController handL;
  public GameObject testSpherePrefab;

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

  // Start is called before the first frame update
  void Start()
  {
    togglableRagdollControllers = GetComponentsInChildren<TogglableRagdollController>();
    moneyManager = GetComponent<MoneyManager>();
    moveScript = GetComponent<BaseMoveAttacc>();

    if (moveScript) {
      moveScript.walkBaseDistancePerSecond = walkBaseDistancePerSecond;
      moveScript.runBaseDistancePerSecond = runBaseDistancePerSecond;
    }

    healthScript = GetComponent<HealthDamage>();

    enemiesManager = GameObject.Find("EnemiesManager");

    if (armLeft) {
      Rigidbody[] armLeftRigidBodies = armLeft.GetComponentsInChildren<Rigidbody>();
      foreach (Rigidbody armLeftRigidBody in armLeftRigidBodies)
      {
        armsRigidbodies.Add(armLeftRigidBody);
      }
    }

    if (armRight) {
      Rigidbody[] armRightRigidBodies = armRight.GetComponentsInChildren<Rigidbody>();
      foreach (Rigidbody armRightRigidBody in armRightRigidBodies)
      {
        armsRigidbodies.Add(armRightRigidBody);
      }
    }

    if (animator)
    {
      body = animator.gameObject;
      navAgent = body.GetComponent<NavMeshAgent>();
    }
  }

  // Update is called once per frame
  void Update()
  {
    if (createModeOn && previewTurret)
    {
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      RaycastHit[] hits = Physics.RaycastAll(ray, 2500, layerMaskGround);
      if (hits.Length > 0)
      {
        System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));
        foreach (RaycastHit hit in hits)
        {
          if (!previewTurretPlaced) {
            previewTurret.transform.position = hit.point;
          } else
          {
            previewTurret.transform.LookAt(new Vector3(hit.point.x, previewTurret.transform.position.y, hit.point.z));
          }
          break;
        }
      }
    }

    if (isInAttackStance && moveScript.hasNavigationTarget)
    {
      moveScript.stopMoving();
    }

    if (isInAttackStance)
    {
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      RaycastHit hit;
      Physics.Raycast(ray, out hit, 2500, layerMaskMove);
      body.transform.LookAt(new Vector3(hit.point.x, body.transform.position.y, hit.point.z));
      animator.SetBool("Attacking", attacking);
      animator.SetFloat("Spell3Speed", spell3AttackSpeedMultiplier);
    }

    if (animator.GetCurrentAnimatorStateInfo(0).IsName("Shoot")) {
      attackTriggered = false;
    }

    if (afroBallClickDown && currentArrowPoint)
    {
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      RaycastHit hit;
      Physics.Raycast(ray, out hit, 2500, layerMaskMove);
      currentArrow.transform.LookAt(new Vector3(hit.point.x, currentArrow.transform.position.y, hit.point.z));
      currentArrowPoint.transform.position = new Vector3(hit.point.x, currentArrowPoint.transform.position.y, hit.point.z);
    }

    if (afroBallLaunchPlanned)
    {
      if (Vector3.Distance(body.transform.position, afroBallLaunchPosition) <= navAgent.stoppingDistance)
      {
        moveScript.targetedEnemy = lastAfroBallClicked.transform;
        // moveScript.attacking = true;
        body.transform.LookAt(lastArrowPoint);
        moveScript.Fire();
        afroBallLaunchPlanned = false;
      }
    }
  }

  void LateUpdate()
  {
    if (handR.punchAttempted && !pairPunch)
    {
      pairPunch = true;
      animator.SetBool("PairPunch", pairPunch);
    } else
    if (handL.punchAttempted && pairPunch)
    {
      pairPunch = false;
      animator.SetBool("PairPunch", pairPunch);
    }

  }

  override public void Spell1 () {
    AttackStanceState(false);
    if (!previewTurret1)
    {
      if (createModeOn)
      {
        CancelCreateMode();
      }
      if (moneyManager.GetMoney () >= turret1Prefab.GetComponentInChildren<TurretStatManager> ().price) {
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
    }
    else
    {
      CancelCreateMode();
    }
  }

  override public void Spell2 () {
    AttackStanceState (false);
    if (!previewTurret2)
    {
      if (createModeOn)
      {
        CancelCreateMode();
      }
      if (moneyManager.GetMoney () >= turret2Prefab.GetComponentInChildren<TurretStatManager> ().price) {
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
    }
    else
    {
      CancelCreateMode();
    }
  }

  public override void Spell3()
  {
    afroBallLaunchPlanned = false;
    if (createModeOn)
    {
      CancelCreateMode();
    }

    AttackStanceState(!isInAttackStance);
  }

  void AttackStanceState (bool state) {
    isInAttackStance = state;

    foreach (Rigidbody armsRigidbody in armsRigidbodies) {
      if (state) {
        armsRigidbody.useGravity = false;
        armsRigidbody.isKinematic = true;
      } else {
        armsRigidbody.useGravity = true;
        armsRigidbody.isKinematic = false;
      }
    }

    foreach (TogglableRagdollController togglableRagdollController in togglableRagdollControllers)
    {
      togglableRagdollController.synchronize = !state;
    }

    animator.SetBool ("AttackStance", state);
  }

  void CancelCreateMode () {
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
    if (down) {
      if (previewTurret) {
        returnValue = true;
      }
      if (previewTurret && (previewTurretPlayerLink.HasEnoughSpace () || previewTurretNeedsOrientation) &&
        previewTurretPlayerLink.characterWallet.GetMoney () >= previewTurret.GetComponentInChildren<TurretStatManager> ().price &&
        createModeOn) {

        if (previewTurretNeedsOrientation)
        {
          previewTurretPlaced = true;
        }
        else
        {
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
      if (previewTurretNeedsOrientation && previewTurretPlayerLink && previewTurretPlayerLink.HasEnoughSpace () &&
        previewTurretPlayerLink.characterWallet.GetMoney () >= previewTurret.GetComponentInChildren<TurretStatManager> ().price &&
        createModeOn) {
        previewTurretPlaced = false;
        previewTurretNeedsOrientation = false;
        createModeOn = false;
        previewTurret1 = false;
        previewTurret2 = false;

        previewTurretPlayerLink.Activate ();

        TurretAfroBallController afroBallController = previewTurret.GetComponentInChildren<TurretAfroBallController>();
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
              TurretAfroBallController afroBallHit = hit.collider.gameObject.GetComponentInParent<TurretAfroBallController>();
              if (afroBallHit)
              {
                if (!moveClickDown && !afroBallClickDown)
                {
                  TurretAfroBallController afroBallController = afroBallHit.GetComponentInChildren<TurretAfroBallController>();
                  if (afroBallController.activated && !afroBallController.launched)
                  {
                    afroBallClicked = afroBallController.gameObject;
                    afroBallClickedStats = afroBallClicked.GetComponentInParent<TurretStatManager>();
                    currentArrow = Instantiate(arrowPrefab, new Vector3(
                      afroBallClicked.transform.position.x,
                      afroBallClicked.transform.position.y + 0.1f,
                      afroBallClicked.transform.position.z
                    ), body.transform.rotation);
                    currentArrowPoint = currentArrow.GetComponentInChildren<ArrowStretchController>();
                    afroBallClickDown = true;
                    break;
                  }
                }
              }
              else
              {
                if (!afroBallClickDown)
                {
                  moveClickDown = true;
                  moveClickPosition = new Vector3 (hit.point.x, hit.point.y + 0.1f, hit.point.z);
                  afroBallLaunchPlanned = false;
                  MoveTo(hit.point);
                  break;
                }
              }
            }
          }
        }
      }
    }
    else
    {
      if (afroBallClickDown)
      {
        afroBallClickDown = false;
        float ballX = afroBallClicked.transform.position.x;
        float ballZ = afroBallClicked.transform.position.z;
        float spaceToPointRatio = (
          (afroBallClickedStats.space + navAgent.stoppingDistance) /
          Vector3.Distance(currentArrowPoint.transform.position, afroBallClicked.transform.position)
        );
        float targetX = currentArrowPoint.transform.position.x;
        float targetZ = currentArrowPoint.transform.position.z;
        Vector3 movePosition = new Vector3(
          ballX + (ballX - targetX) * spaceToPointRatio,
          body.transform.position.y,
          ballZ + (ballZ - targetZ) * spaceToPointRatio
        );
        Instantiate (targetPointerPrefab, movePosition, new Quaternion ());
        MoveTo(movePosition);
        lastArrowPoint = new Vector3(currentArrowPoint.transform.position.x, body.transform.position.y, currentArrowPoint.transform.position.z);
        lastAfroBallClicked = afroBallClicked;
        Destroy(currentArrow);
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
    if (moveScript && healthScript) {
      if (!healthScript.isDead) {
        moveScript.hasNavigationTarget = true;
        moveScript.navigationTargetMovable = attackTarget.transform;
        moveScript.isNavigationTargetMovable = true;
      }
    }
  }

  void MoveTo (Vector3 point) {
    if (moveScript && healthScript) {
      if (!healthScript.isDead) {
        moveScript.hasNavigationTarget = true;
        moveScript.navigationTarget = point;
        moveScript.isNavigationTargetMovable = false;
      }
    }
  }

  public void speedUpSpell3() {
    spell3AttackSpeedMultiplier *= 1.05f;
  }
}