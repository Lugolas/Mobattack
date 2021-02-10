using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour {
  protected Animator anim;
  protected NavMeshAgent navAgent;
  protected HealthSimple health;
  protected HealthSimple objectiveHealth;
  protected GameObject objectiveObject;
  protected Transform objective;
  protected GameObject enemyManager;
  protected bool isCharred = false;
  protected bool isRunner = false;
  public int maxHealthNormal = 5;
  protected float maxHealthCharredRatio = 4;
  protected int maxHealthCharred = 20;
  public GameObject headband;
  public Material bone;
  public Material boneCharred;
  public List<GameObject> lightsCharred = new List<GameObject>();
  protected bool spawning = false;
  protected bool hasSpawned = false;
  public Transform body;
  public List<Rigidbody> bodyParts = new List<Rigidbody>();
  public List<Collider> bodyPartDetailColliders = new List<Collider>();
  public bool hasDied = false;
  protected Manager managersManager;
  protected string spawnAnimationName;
  public SpawnPointController spawnPoint;
  protected GameController gameController;
  protected int lastWaveNumberKnown;

  protected void Init()
  {
    gameController = GameObject.Find("GameController").GetComponent<GameController>();
    lastWaveNumberKnown = gameController.currentWaveLooped;

    maxHealthCharred = Mathf.RoundToInt(maxHealthCharredRatio * maxHealthNormal);
    GameObject objectiveObject = GameObject.Find("EnemyObjective");
    GameObject managersManagerObject = GameObject.Find("ManagersManager");
    if (managersManagerObject)
    {
      managersManager = managersManagerObject.GetComponent<Manager>();
    }

    if (objectiveObject) {
      objective = objectiveObject.transform;
      objectiveHealth = objectiveObject.GetComponent<HealthSimple> ();
    }

    health = GetComponent<HealthSimple>();
    health.isDead = true;
    enemyManager = GameObject.Find ("EnemiesManager");
    transform.parent = enemyManager.transform;
    anim = GetComponent<Animator> ();
    if (!anim) {
      anim = GetComponentInChildren<Animator> ();
    }
    navAgent = GetComponent<NavMeshAgent> ();
    if (!navAgent) {
      navAgent = GetComponentInChildren<NavMeshAgent>();
    }

    navAgent.destination = objective.position;
    navAgent.isStopped = false;

    isRunner = (Random.Range(0, 2) == 1);
    isCharred = (Random.Range(0, 2) == 1);
    int maxHealth;
    if (isCharred)
    {
      SkinnedMeshRenderer[] meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
      foreach (SkinnedMeshRenderer meshRenderer in meshRenderers)
      {
        if (meshRenderer.material.name.Substring(0, 4) == bone.name.Substring(0, 4))
        {
          meshRenderer.material = boneCharred;
        }
      }
      foreach (GameObject lightCharred in lightsCharred)
      {
        lightCharred.SetActive(true);
      }
      maxHealth = maxHealthCharred;
    }
    else
    {
      maxHealth = maxHealthNormal;
    }
    health.maxHealth = maxHealth;
    anim.SetBool("IsRunner", isRunner);
    anim.SetBool("IsRunning", true);
    anim.SetTrigger("Spawn");
    spawnPoint.used = true;
  }

  protected void SpawnAnimationProcess()
  {
    spawning = true;
    headband.SetActive(isRunner);
  }

  protected void PostSpawnAnimationProcess()
  {
    spawning = false;
    health.isDead = false;
    hasSpawned = true;
    spawnPoint.used = false;
    managersManager.rangeSnapshotWanted = true;
  }

  protected void TouchedObjectiveProcess()
  {
    anim.SetBool ("IsRunning", false);
    hasDied = true;
    Destroy (gameObject);

    objectiveHealth.TakeDamage (health.currentHealth, gameObject);
  }

  protected bool HasTouchedObjective()
  {
    return (!health.isDead
      && !navAgent.pathPending
      && !double.IsInfinity(navAgent.remainingDistance)
      && Vector3.Distance(body.position, objective.position) <= navAgent.stoppingDistance
    );
  }

  protected void DyingProcess()
  {
    hasDied = true;
    managersManager.rangeSnapshotWanted = true;
    anim.SetBool("IsDead", true);
    anim.enabled = false;
    navAgent.isStopped = true;
    navAgent.enabled = false;
    navAgent.GetComponent<CapsuleCollider>().enabled = false;
    foreach (Rigidbody bodyPart in bodyParts)
    {
      bodyPart.transform.SetParent(transform);
      bodyPart.GetComponent<Collider>().isTrigger = false;
      bodyPart.isKinematic = false;
      bodyPart.useGravity = true;
      bodyPart.collisionDetectionMode = CollisionDetectionMode.Continuous;
      bodyPart.tag = "Corpse";
    }
    foreach (Collider bodyPartDetailCollider in bodyPartDetailColliders)
    {
      bodyPartDetailCollider.isTrigger = false;
    }
  }

  protected void Updating () {
    DestroyOnWaveChange();

    if (anim.GetCurrentAnimatorStateInfo(0).IsName(spawnAnimationName) && !hasSpawned) {
      SpawnAnimationProcess();
    } else if (spawning) {
      PostSpawnAnimationProcess();
    }

    if (HasTouchedObjective()) {
      TouchedObjectiveProcess();
    }

    if (health.isDead && !hasDied && hasSpawned) {
      DyingProcess();
    }
  }

  public void refreshDestination () {
    if (!health.isDead) {
      navAgent.SetDestination (objective.position);

      navAgent.isStopped = false;
    }
  }

  protected void DestroyOnWaveChange() {
    if (gameController.currentWaveLooped != lastWaveNumberKnown) {
      lastWaveNumberKnown = gameController.currentWaveLooped;
      Destroy(gameObject);
    }
  }
}