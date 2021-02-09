using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TurretWolfDenController : TurretController
{
  public Transform wolfSpawnPoint;
  public GameObject wolfPrefab;
  private bool hasExtortedCharacter = false;
  private float wolfSpawnTime;
  public float wolfSpawnDelay = 5f;
  TurretStatManager statManager;
  TurretPlayerLink playerLink;
  List<WolfController> wolves = new List<WolfController>();
  List<Vector3> wolfWaitingPoints = new List<Vector3>();
  public int wolfAmountLimit = 5;
  NavMeshObstacle[] obstacles;
  bool activated = false;
  public float alphaWolfFireTime;

  private void Start()
  {
    playerLink = GetComponentInParent<TurretPlayerLink>();
    statManager = GetComponentInParent<TurretStatManager>();
    obstacles = GetComponentsInChildren<NavMeshObstacle>(true);

    wolfSpawnTime = Time.time;
  }

  void Update()
  {
    if (!activated && playerLink.activated) {
      foreach (NavMeshObstacle obstacle in obstacles)
      {
        obstacle.enabled = true;
      }
      activated = true;
    }

    if (playerLink.activated && !hasExtortedCharacter)
    {
      playerLink.characterWallet.SubstractMoney(statManager.price);
      hasExtortedCharacter = true;
    }

    SpawnWolfIfPossible();
  }

  void SpawnWolfIfPossible()
  {
    if (playerLink.activated && Time.time >= wolfSpawnTime + wolfSpawnDelay)
    {
      wolfSpawnTime = Time.time;
      SpawnWolf();
    }
  }

  void LateUpdate()
  {
    if (enemiesInRange.Count > 0 && !target) {
      targetUpdateWanted = true;
    }

    if (targetUpdateWanted)
    {
      UpdateTarget(statManager.range);
    }
  }

  void SpawnWolf()
  {
    if (wolves.Count < wolfAmountLimit)
    {
      Vector3 waitpoint;
      bool waitPointNotFound = true;
      do
      {
        waitpoint = new Vector3(Random.Range(-6.5f, 6.5f), 0, Random.Range(-6.5f, 6.5f));
        if (Vector3.Distance(Vector3.zero, waitpoint) >= 5) {
          bool positionIsTaken = false;
          foreach (Vector3 wolfWaitingPoint in wolfWaitingPoints)
          {
            if (Vector3.Distance(wolfWaitingPoint, waitpoint) < 4.5) {
              positionIsTaken = true;
            }
          }
          if (!positionIsTaken) {
            waitPointNotFound = false;
            wolfWaitingPoints.Add(waitpoint);
            GameObject wolf = Instantiate(wolfPrefab, wolfSpawnPoint.position, wolfSpawnPoint.rotation) as GameObject;
            WolfController wolfController = wolf.GetComponent<WolfController>();
            wolves.Add(wolfController);
            wolfController.waitPoint = transform.position + waitpoint;
            wolfController.den = this;
            wolfController.moneyManager = playerLink.characterWallet;
            wolfController.delay = statManager.delay;
            wolfController.gunDamage = statManager.damage;
            wolfController.statManager = statManager;
            wolfController.id = wolves.Count;
          }
        }
      } while (waitPointNotFound);

    }
  }
}
