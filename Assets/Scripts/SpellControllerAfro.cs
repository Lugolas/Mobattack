using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
  GameObject body;
  public GameObject armLeft;
  public GameObject armRight;
  AfroArmController[] armControllers;
  List<Rigidbody> armsRigidbodies = new List<Rigidbody>();
  bool attackTriggered = false;
  public float spell3AttackSpeedMultiplier = 1f;
  bool previewTurretNeedsOrientation = false;
  bool previewTurretPlaced = false;
  bool attacking = false;

  // Start is called before the first frame update
  void Start()
  {
    armControllers = GetComponentsInChildren<AfroArmController>();
    moneyManager = GetComponent<MoneyManager>();
    moveScript = GetComponent<BaseMoveAttacc>();
    healthScript = GetComponent<HealthDamage>();

    enemiesManager = GameObject.Find("EnemiesManager");

    Rigidbody[] armLeftRigidBodies = armLeft.GetComponentsInChildren<Rigidbody>();
    foreach (Rigidbody armLeftRigidBody in armLeftRigidBodies)
    {
      armsRigidbodies.Add(armLeftRigidBody);
    }

    Rigidbody[] armRightRigidBodies = armRight.GetComponentsInChildren<Rigidbody>();
    foreach (Rigidbody armRightRigidBody in armRightRigidBodies)
    {
      armsRigidbodies.Add(armRightRigidBody);
    }

    if (animator)
    {
      body = animator.gameObject;
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

    if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")) {
      attackTriggered = false;
    }
  }

  override public void Spell1 () {
    AttackStanceState (false);
    if (!createModeOn) {
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
            break;
          }
        }
      }
    }
  }

  override public void Spell2 () {
    AttackStanceState (false);
    if (!createModeOn) {
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
            break;
          }
        }
      }
    }
  }

  public override void Spell3()
  {
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

    foreach (AfroArmController armController in armControllers)
    {
      armController.synchronize = !state;
    }

    animator.SetBool ("AttackStance", state);
  }

  void CancelCreateMode () {
    createModeOn = false;
    Destroy (previewTurret);
    previewTurret = null;
    previewTurretPlayerLink = null;
  }

  void EnemiesRefreshPathfinding () {
    EnemyController[] enemies = enemiesManager.GetComponentsInChildren<EnemyController> ();
    foreach (EnemyController enemy in enemies) {
      enemy.refreshDestination ();
    }
  }

  public override bool Fire1 (bool down) {
    if (down) {
      if (previewTurret && previewTurretPlayerLink.HasEnoughSpace () &&
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

          EnemiesRefreshPathfinding ();
        }

        return true;
      }

      if (isInAttackStance) {
        attacking = true;
      }
      return false;
    } else {
      if (previewTurretNeedsOrientation) {
          previewTurretPlaced = false;
          previewTurretNeedsOrientation = false;
          createModeOn = false;

          previewTurretPlayerLink.Activate ();
          previewTurret = null;

          EnemiesRefreshPathfinding ();
      }
      if (isInAttackStance) {
        attacking = false;
      }
      return true;
    }
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
            EnemyController enemyHit = hit.collider.gameObject.GetComponent<EnemyController> ();
            if (enemyHit) {
              attackTarget = hit.transform.gameObject;
              HealthSimple targetInfo = attackTarget.GetComponent<HealthSimple> ();
              if (targetInfo) {
                Attack ();
              }
              break;
            } else {
              moveClickDown = true;
              moveClickPosition = new Vector3 (hit.point.x, hit.point.y + 0.1f, hit.point.z);
              MoveTo (hit.point);
              break;
            }
          }
        }
      }
    } else {
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
    spell3AttackSpeedMultiplier *= 1.01f;
  }
}