using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellControllerSphere : SpellController
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

  // Start is called before the first frame update
  void Start()
  {
    moneyManager = GetComponent<MoneyManager>();
    moveScript = GetComponent<BaseMoveAttacc>();
    healthScript = GetComponent<HealthDamage>();
    enemiesManager = GameObject.Find("EnemiesManager");
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
          previewTurret.transform.position = hit.point;
          break;
        }
      }
    }
  }

  override public void Spell1()
  {
    if (!createModeOn)
    {
      if (moneyManager.GetMoney() >= turret1Prefab.GetComponentInChildren<TurretStatManager>().price)
      {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, 2500, layerMaskGround);

        if (hits.Length > 0)
        {
          System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));
          foreach (RaycastHit hit in hits)
          {
            turretCreationPoint = hit.point;
            previewTurret = Instantiate(turret1Prefab, turretCreationPoint, new Quaternion());
            previewTurretPlayerLink = previewTurret.GetComponentInChildren<TurretPlayerLink>();
            previewTurretPlayerLink.InitialLink(gameObject, moneyManager);
            createModeOn = true;
            break;
          }
        }
      }
    }
  }

  override public void Spell2()
  {
    if (!createModeOn)
    {
      if (moneyManager.GetMoney() >= turret2Prefab.GetComponentInChildren<TurretStatManager>().price)
      {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, 2500, layerMaskGround);

        if (hits.Length > 0)
        {
          System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));
          foreach (RaycastHit hit in hits)
          {
            turretCreationPoint = hit.point;
            previewTurret = Instantiate(turret2Prefab, turretCreationPoint, new Quaternion());
            previewTurretPlayerLink = previewTurret.GetComponentInChildren<TurretPlayerLink>();
            previewTurretPlayerLink.InitialLink(gameObject, moneyManager);
            createModeOn = true;
            break;
          }
        }
      }
    }
  }

  void CancelCreateMode()
  {
    createModeOn = false;
    Destroy(previewTurret);
    previewTurret = null;
    previewTurretPlayerLink = null;
  }

  void EnemiesRefreshPathfinding()
  {
    EnemyController[] enemies = enemiesManager.GetComponentsInChildren<EnemyController>();
    foreach (EnemyController enemy in enemies)
    {
      enemy.refreshDestination();
    }
  }

  public override bool Fire1()
  {
    if (previewTurret && previewTurretPlayerLink.HasEnoughSpace()
      && previewTurretPlayerLink.characterWallet.GetMoney() >= previewTurret.GetComponentInChildren<TurretStatManager>().price
      && createModeOn)
    {
      createModeOn = false;

      previewTurretPlayerLink.Activate();
      previewTurret = null;

      EnemiesRefreshPathfinding();
      return true;
    }
    else
    {
      return false;
    }
  }

  public override void Fire2(bool down)
  {
    if (createModeOn)
    {
      CancelCreateMode();
    }

    if (down)
    {
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      RaycastHit[] hits = Physics.RaycastAll(ray, 2500, layerMaskMove);
      if (hits.Length > 0)
      {
        System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));
        foreach (RaycastHit hit in hits)
        {
          if (hit.collider.gameObject != gameObject)
          {
            EnemyController enemyHit = hit.collider.gameObject.GetComponent<EnemyController>();
            if (enemyHit)
            {
              attackTarget = hit.transform.gameObject;
              HealthSimple targetInfo = attackTarget.GetComponent<HealthSimple>();
              if (targetInfo)
              {
                Attack();
              }
              break;
            }
            else
            {
              moveClickDown = true;
              moveClickPosition = new Vector3(hit.point.x, hit.point.y + 0.1f, hit.point.z);
              MoveTo(hit.point);
              break;
            }
          }
        }
      }
    }
    else
    {
      if (moveClickDown)
      {
        moveClickDown = false;
        Instantiate(targetPointerPrefab, moveClickPosition, new Quaternion());
      }
    }
  }

  void Attack()
  {
    if (moveScript && healthScript)
    {
      if (!healthScript.isDead)
      {
        moveScript.hasNavigationTarget = true;
        moveScript.navigationTargetMovable = attackTarget.transform;
        moveScript.isNavigationTargetMovable = true;
      }
    }
  }

  void MoveTo(Vector3 point)
  {
    if (moveScript && healthScript)
    {
      if (!healthScript.isDead)
      {
        moveScript.hasNavigationTarget = true;
        moveScript.navigationTarget = point;
        moveScript.isNavigationTargetMovable = false;
      }
    }
  }
}



