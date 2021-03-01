using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMoveAttacc : MonoBehaviour
{
  public float moveSpeed;
  float walkSpeed;
  float runSpeed;

  public float walkBaseDistancePerSecond = -1f;
  public float runBaseDistancePerSecond = -1f;

  public Transform targetedEnemy = null;
  private bool animatingAttack = false;
  public Animator anim;

  public Vector3 navigationTarget = Vector3.zero;
  public Transform navigationTargetMovable = null;
  public bool hasNavigationTarget = false;
  public bool isNavigationTargetMovable = false;
  private UnityEngine.AI.NavMeshAgent navAgent;
  public float shootDistance = 10f;
  public float distanceAcceptability = 1.0f;
  public Transform FireballSpawnPoint = null;
  public GameObject FireballPrefab = null;
  public HealthSimple health;
  public bool attacking = false;
  private int running = 0;
  private float nextFire;
  private float nextSpell;
  private bool disabled = false;
  private FireMomentListener fireMomentListener;
  private bool timetoFireFired = false;
  private string nameOfCharacter;
  public float timeBetweenShots = 1.15f;
  bool withinShootDistance = false;
  public GameObject attacker = null;
  public MoneyManager moneyManager;
  HealthSimple navTargetHealth;
  public LineRenderer line;
  // private int fireballDamage = 25;
  public enum BaseAttackType
  {
    TargetSolo,
    TargetGroup,
    Group,
    Projectile,
    Hitscan
  }
  public BaseAttackType baseAttackType;
  public int baseAttackDamage = 50;

  void Start()
  {
    GameObject character = Tools.FindObjectOrParentWithTag(gameObject, "Character");
    if (character)
    {
      nameOfCharacter = character.name;
    }
    fireMomentListener = GetComponentInChildren<FireMomentListener>();
    health = GetComponent<HealthSimple>();
    if (!anim)
    {
      anim = GetComponent<Animator>();
      if (!anim)
      {
        anim = GetComponentInChildren<Animator>();
      }
    }

    moveSpeed = anim.GetFloat("MoveSpeed");

    navAgent = GetComponentInChildren<UnityEngine.AI.NavMeshAgent>();
    if (!moneyManager)
      moneyManager = GetComponent<MoneyManager>();
  }

  void LateUpdate()
  {
    if (!disabled)
    {
      if (line && !navAgent.pathPending && navAgent.path.corners.Length >= 2) {
        line.positionCount = navAgent.path.corners.Length;

        line.SetPositions(navAgent.path.corners);
      }

      if (navigationTargetMovable && navigationTargetMovable.gameObject)
      {
        navTargetHealth = navigationTargetMovable.gameObject.GetComponentInParent<HealthSimple>();
        if (!navTargetHealth) {
          navTargetHealth = navigationTargetMovable.gameObject.GetComponentInChildren<HealthSimple>();
        }
      }

      if (navTargetHealth && navTargetHealth.isDead) {
        navigationTargetMovable = null;
      }
      // Debug.Log("navigationTargetMovable " + navigationTargetMovable);
      // Debug.Log("--------------------------------------------------------------------------------------------");

      // Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      // RaycastHit hit;

      // if (Input.GetButton("Fire2") && !health.isDead)
      // {
      //   rightClicked = true;
      //   if (Physics.Raycast(ray, out hit, 2500))
      //   {
      //     if (hit.collider.CompareTag("Character") && hit.collider.name != nameOfCharacter)
      //     {
      //       targetedEnemy = hit.transform;
      //       enemyClicked = true;
      //     }
      //     else
      //     {
      //       CmdSetPoint(hit.point);
      //     }
      //   }
      // }

      if (hasNavigationTarget && !isNavigationTargetMovable && !health.isDead)
      {
        navAgent.destination = navigationTarget;
        run();
        navAgent.isStopped = false;
      }

      // if (Input.GetKeyDown(KeyCode.A) && !health.isDead)
      // {
      //   navAgent.isStopped = true;
      //   running = 0;
      //   Spell();
      // }

      if (fireMomentListener && fireMomentListener.timeToFire && targetedEnemy && timetoFireFired == false && attacking)
      {
        fireMomentListener.timeToFire = false;
        timetoFireFired = true;
        switch (baseAttackType)
        {
          case BaseAttackType.TargetSolo:
            Tools.InflictDamage(targetedEnemy, baseAttackDamage, moneyManager, attacker);
            break;
          case BaseAttackType.TargetGroup:
            break;
          case BaseAttackType.Group:
            break;
          case BaseAttackType.Projectile:
            if (FireballSpawnPoint && FireballPrefab)
            {
              GameObject fireball = Instantiate(FireballPrefab, FireballSpawnPoint.position, FireballSpawnPoint.rotation) as GameObject;
              // fireball.GetComponent<Rigidbody>().velocity = fireball.transform.forward * 5;
              Fireball fireballScript = fireball.GetComponent<Fireball>();
              fireballScript.attacker = gameObject;
              fireballScript.damage = baseAttackDamage;
              fireballScript.target = targetedEnemy;
              fireballScript.characterWallet = moneyManager;
            }
            break;
          case BaseAttackType.Hitscan:
            break;
          default:
            break;
        }
      }

      if (hasNavigationTarget && isNavigationTargetMovable && !health.isDead)
      {
        StartCoroutine("MoveAndShoot");
      }
      else if (running != 0 && !health.isDead
      && (navAgent.remainingDistance <= navAgent.stoppingDistance
        || Vector3.Distance(navAgent.destination, navAgent.transform.position) <= navAgent.stoppingDistance) 
      && !double.IsInfinity(Vector3.Distance(navAgent.destination, navAgent.transform.position)))
      {
        if (running != 0)
        {
          if (navAgent.pathPending)
          {
            running = 2;
          }
          else
          {
            running = 0;
            hasNavigationTarget = false;
          }
        }
      }

      if (running == 0)
      {
        anim.SetBool("IsRunning", false);
      }
      else
      {
        anim.SetBool("IsRunning", true);
      }

      if (this.anim.GetCurrentAnimatorStateInfo(0).IsName("Attack1") 
        || this.anim.GetCurrentAnimatorStateInfo(0).IsName("Attack2") 
        || this.anim.GetCurrentAnimatorStateInfo(0).IsName("Attack3")
        || this.anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
      {
        animatingAttack = true;
      }
      else if (animatingAttack && attacking)
      {
        attacking = false;
        animatingAttack = false;
        timetoFireFired = false;
      }
      else
      {
        animatingAttack = false;
        timetoFireFired = false;
      }
    }

    walkSpeed = moveSpeed / walkBaseDistancePerSecond;
    runSpeed = moveSpeed / runBaseDistancePerSecond;

    anim.SetFloat("MoveSpeed", moveSpeed);
    anim.SetFloat("WalkSpeed", walkSpeed);
    anim.SetFloat("RunSpeed", runSpeed);
  }

  public void disable()
  {
    if (!disabled)
    {
      disabled = true;
    }
  }

  public void enable()
  {
    if (disabled)
    {
      disabled = false;
    }
  }

  IEnumerator MoveAndShoot()
  {
    if (health.isDead)
    {
      yield return null;
    }

    targetedEnemy = navigationTargetMovable;
    if (targetedEnemy && navTargetHealth && !navTargetHealth.isDead)
    {
      navAgent.destination = targetedEnemy.position;

      if (navAgent.pathPending)
      {
        yield return null;
      }

      float acceptableDistance = shootDistance;
      if (withinShootDistance)
      {
        acceptableDistance = shootDistance + distanceAcceptability;
      }

      if (Vector3.Distance(targetedEnemy.position, navAgent.transform.position) > acceptableDistance || double.IsInfinity(Vector3.Distance(targetedEnemy.position, navAgent.transform.position)))
      {
        withinShootDistance = false;
        navAgent.isStopped = false;
        run();
      }
      else
      {
        if (Vector3.Distance(targetedEnemy.position, navAgent.transform.position) <= acceptableDistance)
        {
          withinShootDistance = true;
          LookAtTargetedEnemy();
          navAgent.isStopped = true;
          running = 0;
          Fire();
          // StartCoroutine("Fire");
        }
      }

      yield return null;
    }
    else
    {
      stopMoving();
    }
  }

  public void LookAtTargetedEnemy()
  {
    navAgent.transform.LookAt(new Vector3(targetedEnemy.position.x, navAgent.transform.position.y, targetedEnemy.position.z));
  }

  public void Fire()
  {
    if (health.isDead)
    {
      return;
    }
    // if (attacking == false && (Time.time > nextFire))
    if (animatingAttack == false && attacking == false && (Time.time > nextFire))
    {
      stopMoving();
      attacking = true;
      anim.SetTrigger("Attack");
      nextFire = Time.time + timeBetweenShots;
      // NEED TO PUT IT EXACTLY WHEN THE ATTACK ANIM STARTED NOT AT THE TRIGGER
      // TRIGGERS CAN STAY ON UNTIL HEARD SO THAT MAY BE NOT PRECISE
    }
  }

  void Spell()
  {
    if (health.isDead)
    {
      return;
    }
    if (animatingAttack == false && attacking == false && (Time.time > nextSpell))
    {
      attacking = true;
      anim.SetTrigger("Spell");
    }
  }

  void run()
  {
    if (health.isDead)
    {
      return;
    }
    running = 1;
    StopCoroutine("Fire");
    attacking = false;
  }

  public void stopMoving()
  {
    running = 0;
    attacking = false;
    // StopCoroutine("Fire");
    hasNavigationTarget = false;
    navAgent.destination = transform.position;
    navAgent.isStopped = true;
  }
}