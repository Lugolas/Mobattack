using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellControllerBear : SpellController
{
  public MoneyManager moneyManager;
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
  TogglableRagdollController[] togglableRagdollControllers;
  bool attackTriggered = false;
  bool previewTurretNeedsOrientation = false;
  bool previewTurretPlaced = false;
  bool attacking = false;
  int attackSelection = 1;
  float attackSelectionTime;
  float walkBaseDistancePerSecond = 4.5f;
  float runBaseDistancePerSecond = 8.8f;
  public float turretRange = 10f;
  public int gunDamage = 10;
  public float delay = 1f;
  public bool bearTurretActive = true;
  bool previewTurret1 = false;
  bool previewTurret2 = false;

  // Start is called before the first frame update
  void Start()
  {
    attackSelectionTime = Time.time;
    togglableRagdollControllers = GetComponentsInChildren<TogglableRagdollController>();
    moneyManager = GetComponent<MoneyManager>();
    moveScript = GetComponent<BaseMoveAttacc>();

    if (moveScript) {
      moveScript.walkBaseDistancePerSecond = walkBaseDistancePerSecond;
      moveScript.runBaseDistancePerSecond = runBaseDistancePerSecond;
    }

    healthScript = GetComponent<HealthDamage>();

    enemiesManager = GameObject.Find("EnemiesManager");

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
    }

    if (animator.GetCurrentAnimatorStateInfo(0).IsName("Shoot")) {
      attackTriggered = false;
    }

    if (Time.time >= attackSelectionTime + .5f)
    {
      attackSelectionTime = Time.time;
      attackSelection = Random.Range(1, 4);
      animator.SetInteger("AttackSelection", attackSelection);
    }
  }

  void EnemiesRefreshPathfinding () {
    EnemyController[] enemies = enemiesManager.GetComponentsInChildren<EnemyController> ();
    foreach (EnemyController enemy in enemies) {
      enemy.refreshDestination ();
    }
  }

  override public void Spell1 () {
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

  override public void Spell3 () {
    bearTurretActive = !bearTurretActive;
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
        previewTurret1 = false;
        previewTurret2 = false;
        createModeOn = false;

        previewTurretPlayerLink.Activate ();

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
      CancelCreateMode();
    }
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
              if (targetInfo) {
                Attack ();
              }
              break;
            } else {
              if (hit.collider.name != "bear") {
                moveClickDown = true;
                moveClickPosition = new Vector3 (hit.point.x, hit.point.y + 0.1f, hit.point.z);
                MoveTo (hit.point);
                break;
              }
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

  void CancelCreateMode () {
    createModeOn = false;
    Destroy (previewTurret);
    previewTurret = null;
    previewTurretPlayerLink = null;
    previewTurretPlaced = false;
    previewTurret1 = false;
    previewTurret2 = false;
  }
}