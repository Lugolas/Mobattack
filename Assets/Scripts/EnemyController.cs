using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour {
  public bool canHaveHeadband = true;
  protected Animator anim;
  protected NavMeshAgent navAgent;
  protected HealthSimple health;
  protected HealthSimple objectiveHealth;
  protected GameObject objectiveObject;
  protected Transform objective;
  protected GameObject enemyManager;
  public bool isCharred = false;
  public bool isRunner = false;
  public int expValue = 1;
  public int maxHealthNormal = 5;
  protected float maxHealthCharredRatio = 4;
  protected int maxHealthCharred = 20;
  public GameObject headband;
  public Material bone;
  public Material inBone;
  public Material boneCharred;
  public Material inBoneCharred;
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
  public SpriteRenderer minimapSprite;
  public List<SpriteRenderer> minimapCharredSprites = new List<SpriteRenderer>();

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

    int maxHealth;
    if (isCharred)
    {
      foreach (SpriteRenderer minimapCharredSprite in minimapCharredSprites)
      {
        minimapCharredSprite.enabled = true;
      }
      SkinnedMeshRenderer[] meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
      foreach (SkinnedMeshRenderer meshRenderer in meshRenderers)
      {
        for (int i = 0; i < meshRenderer.materials.Length; i++)
        {
          if (meshRenderer.materials[i].name.Substring(0, 4) == bone.name.Substring(0, 4))
          {
            meshRenderer.materials[i].SetColor("_Color", boneCharred.GetColor("_Color"));
            meshRenderer.materials[i].SetColor("_OutlineColor", boneCharred.GetColor("_OutlineColor"));
          }
          if (meshRenderer.materials[i].name.Substring(0, 4) == inBone.name.Substring(0, 4))
          {
            meshRenderer.materials[i].SetColor("_Color", inBoneCharred.GetColor("_Color"));
            meshRenderer.materials[i].SetColor("_OutlineColor", inBoneCharred.GetColor("_OutlineColor"));
          }
        }
      }
      foreach (GameObject lightCharred in lightsCharred)
      {
        // lightCharred.SetActive(true);
      }
      maxHealth = maxHealthCharred;
    }
    else
    {
      maxHealth = maxHealthNormal;
    }
    health.maxHealth = maxHealth;
    if (canHaveHeadband)
      anim.SetBool("IsRunner", isRunner);
    anim.SetBool("IsRunning", true);
    anim.SetTrigger("Spawn");
    spawnPoint.used = true;
  }

  protected void SpawnAnimationProcess()
  {
    spawning = true;
    if (canHaveHeadband)
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

    objectiveHealth.TakeDamage (health.currentHealth);
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
    if (health.fatalAttacker) {
      health.fatalAttacker.GotAKill(expValue, body.position);
    }
    managersManager.rangeSnapshotWanted = true;
    anim.SetBool("IsDead", true);
    anim.enabled = false;
    navAgent.isStopped = true;
    navAgent.enabled = false;
    navAgent.GetComponent<CapsuleCollider>().enabled = false;
    foreach (Rigidbody bodyPart in bodyParts)
    {
      bodyPart.transform.SetParent(transform);
      Collider bodypartCollider = bodyPart.GetComponent<Collider>();
      if (bodypartCollider)
        bodypartCollider.isTrigger = false;
      minimapSprite.enabled = false;
      foreach (SpriteRenderer minimapCharredSprite in minimapCharredSprites)
      {
        minimapCharredSprite.enabled = false;
      }
      bodyPart.isKinematic = false;
      bodyPart.useGravity = true;
      bodyPart.collisionDetectionMode = CollisionDetectionMode.Continuous;
      bodyPart.tag = "Corpse";
    }
    if (isCharred) {
      foreach (GameObject lightCharred in lightsCharred)
      {
        lightCharred.SetActive(false);
      }
    }
    foreach (Collider bodyPartDetailCollider in bodyPartDetailColliders)
    {
      bodyPartDetailCollider.isTrigger = false;
    }

    // Destroy after every bodypart has disappeared by itself
    Destroy(gameObject, 30);
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
      // Destroy(gameObject);
    }
  }

  public bool GetIsDead() {
    if (health) {
      return health.isDead;
    } else {
      return true;
    }
  }
}