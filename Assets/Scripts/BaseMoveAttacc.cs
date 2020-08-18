using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BaseMoveAttacc : NetworkBehaviour
{
  [SyncVar]
  public Transform targetedEnemy = null;
  private bool enemyClicked;
  private bool animatingAttack = false;
  private bool animatingRapidFire = false;
  private Animator anim;

  [SyncVar]
  public Vector3 navigationTarget = Vector3.zero;
  [SyncVar]
  public Transform navigationTargetMovable = null;
  [SyncVar]
  public bool hasNavigationTarget = false;
  [SyncVar]
  public bool isNavigationTargetMovable = false;
  private UnityEngine.AI.NavMeshAgent navAgent;
  public float shootDistance = 10f;
  public Transform FireballSpawnPoint = null;
  public GameObject FireballPrefab = null;
  public GameObject FireballRapidPrefab = null;
  public HealthDamage healthDamage;
  private bool attacking = false;
  private int running = 0;
  public int rapidFireballAmount = 2;
  private float nextFire;
  private float nextSpell;
  private bool disabled = false;
  private FireMomentListener fireMomentListener;
  private MageRapidFire mageRapidFire;
  private bool timetoFireFired = false;
  private bool firstFireballFired = false;
  private bool secondFireballFired = false;
  private string nameOfCharacter;
  private float timeBetweenShots = 1.15f;
  private float timeBetweenSpells = 1.15f;
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
    mageRapidFire = GetComponentInChildren<MageRapidFire>();
    healthDamage = GetComponent<HealthDamage>();
    anim = GetComponent<Animator>();
    if (!anim)
    {
      anim = GetComponentInChildren<Animator>();
    }
    navAgent = GetComponentInChildren<UnityEngine.AI.NavMeshAgent>();
  }

  [Command]
  void CmdSetPoint(Vector3 point)
  {
    hasNavigationTarget = true;
    navigationTarget = point;
    isNavigationTargetMovable = false;
  }

  // [Command]
  // void CmdSetTarget(Transform target)
  // {
  //   hasNavigationTarget = true;
  //   navigationTargetMovable = target;
  //   isNavigationTargetMovable = true;
  // }
  void LateUpdate()
  {
    if (!disabled)
    {
      Debug.Log("--------------------------------------------------------------------------------------------");
      Debug.Log("navigationTargetMovable " + navigationTargetMovable);
      if (navigationTargetMovable)
      {
        Debug.Log("navigationTargetMovable.gameObject " + navigationTargetMovable.gameObject);
        if (navigationTargetMovable.gameObject)
        {
          Debug.Log("navigationTargetMovable.gameObject.GetComponent<HealthDamage>() " + navigationTargetMovable.gameObject.GetComponent<HealthDamage>());
          if (navigationTargetMovable.gameObject.GetComponent<HealthDamage>())
          {
            Debug.Log("navigationTargetMovable.gameObject.GetComponent<HealthDamage>().isDead " + navigationTargetMovable.gameObject.GetComponent<HealthDamage>().isDead);
          }
        }
      }
      if (navigationTargetMovable && navigationTargetMovable.gameObject.GetComponent<HealthDamage>().isDead)
      {
        navigationTargetMovable = null;
      }
      Debug.Log("navigationTargetMovable " + navigationTargetMovable);
      Debug.Log("--------------------------------------------------------------------------------------------");

      // Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      // RaycastHit hit;

      // if (Input.GetButton("Fire2") && !healthDamage.isDead)
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

      if (hasNavigationTarget && !isNavigationTargetMovable && !healthDamage.isDead)
      {
        enemyClicked = false;
        navAgent.destination = navigationTarget;
        run();
        navAgent.isStopped = false;
      }

      // if (Input.GetKeyDown(KeyCode.A) && !healthDamage.isDead)
      // {
      //   navAgent.isStopped = true;
      //   running = 0;
      //   Spell();
      // }

      // Debug.Log(mageRapidFire.firstFireballFire);
      // Debug.Log(firstFireballFired);
      if (mageRapidFire && mageRapidFire.firstFireballFire && !mageRapidFire.secondFireballFire && firstFireballFired == false && attacking)
      {
        mageRapidFire.firstFireballFire = false;
        firstFireballFired = true;
        secondFireballFired = mageRapidFire.secondFireballFire;
        nextSpell = Time.time + timeBetweenSpells;

        if (FireballSpawnPoint && FireballRapidPrefab)
        {
          for (int i = 1; i <= rapidFireballAmount; i++)
          {
            Quaternion rotation = FireballSpawnPoint.rotation;
            rotation = Quaternion.Euler(rotation.eulerAngles.x, rotation.eulerAngles.y + Random.Range(-25, 25), rotation.eulerAngles.z);
            GameObject fireball = Instantiate(FireballRapidPrefab, FireballSpawnPoint.position, rotation) as GameObject;
            // fireball.GetComponent<Rigidbody>().velocity = fireball.transform.forward * 5;
            fireball.GetComponent<Fireball>().attacker = gameObject;
            if (targetedEnemy)
            {
              fireball.GetComponent<Fireball>().target = targetedEnemy;
            }
            else
            {
              fireball.GetComponent<Fireball>().target = transform;
            }
          }
        }

      }

      if (mageRapidFire && mageRapidFire.secondFireballFire && secondFireballFired == false && attacking)
      {
        mageRapidFire.secondFireballFire = false;
        firstFireballFired = mageRapidFire.firstFireballFire;
        secondFireballFired = true;

        if (FireballSpawnPoint && FireballRapidPrefab)
        {
          for (int i = 1; i <= rapidFireballAmount; i++)
          {
            Quaternion rotation = FireballSpawnPoint.rotation;
            rotation = Quaternion.Euler(rotation.eulerAngles.x, rotation.eulerAngles.y + Random.Range(-25, 25), rotation.eulerAngles.z);
            GameObject fireball = Instantiate(FireballRapidPrefab, FireballSpawnPoint.position, rotation) as GameObject;
            // fireball.GetComponent<Rigidbody>().velocity = fireball.transform.forward * 5;
            fireball.GetComponent<Fireball>().attacker = gameObject;
            if (targetedEnemy)
            {
              fireball.GetComponent<Fireball>().target = targetedEnemy;
            }
            else
            {
              fireball.GetComponent<Fireball>().target = transform;
            }
          }
        }

      }

      if (fireMomentListener && fireMomentListener.timeToFire && targetedEnemy && timetoFireFired == false && attacking)
      {
        fireMomentListener.timeToFire = false;
        timetoFireFired = true;
        nextFire = Time.time + timeBetweenShots;

        switch (baseAttackType)
        {
          case BaseAttackType.TargetSolo:
            inflictDamage(targetedEnemy, baseAttackDamage);
            break;
          case BaseAttackType.TargetGroup:
            break;
          case BaseAttackType.Group:
            break;
          case BaseAttackType.Projectile:
            if (FireballSpawnPoint && FireballPrefab)
            {
              GameObject fireball = Instantiate(FireballPrefab, FireballSpawnPoint.position, transform.rotation) as GameObject;
              // fireball.GetComponent<Rigidbody>().velocity = fireball.transform.forward * 5;
              fireball.GetComponent<Fireball>().attacker = gameObject;
              fireball.GetComponent<Fireball>().target = targetedEnemy;
            }
            break;
          case BaseAttackType.Hitscan:
            break;
          default:
            break;
        }
      }

      if (hasNavigationTarget && isNavigationTargetMovable && !healthDamage.isDead)
      {
        StartCoroutine("MoveAndShoot");
      }
      else if (running != 0 && navAgent.remainingDistance <= navAgent.stoppingDistance && !double.IsInfinity(navAgent.remainingDistance))
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

      if (this.anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
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

      if (this.anim.GetCurrentAnimatorStateInfo(0).IsName("RapidFire"))
      {
        animatingRapidFire = true;
      }
      else if (animatingRapidFire && attacking)
      {
        attacking = false;
        animatingRapidFire = false;
        firstFireballFired = false;
        secondFireballFired = false;
      }
      else
      {
        animatingRapidFire = false;
        firstFireballFired = false;
        secondFireballFired = false;
      }
    }
  }

  private void inflictDamage(Transform targetedEnemy, int damageAmount)
  {
    HealthDamage hd = targetedEnemy.GetComponent<HealthDamage>();
    if (hd)
    {
      hd.TakeDamage(damageAmount);
    }
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
    if (healthDamage.isDead)
    {
      yield return null;
    }

    targetedEnemy = navigationTargetMovable;
    // Debug.Log("ça avance");
    // Debug.Log("navigationTargetMovable " + navigationTargetMovable);
    // Debug.Log("targetedEnemy " + targetedEnemy);
    // Debug.Log("targetedEnemy.gameObject " + targetedEnemy.gameObject);
    // Debug.Log("targetedEnemy.GetComponent<HealthDamage>() " + targetedEnemy.gameObject.GetComponent<HealthDamage>());
    // Debug.Log("targetedEnemy.GetComponent<HealthDamage>().isDead " + targetedEnemy.gameObject.GetComponent<HealthDamage>().isDead);
    if (targetedEnemy && !targetedEnemy.gameObject.GetComponent<HealthDamage>().isDead)
    {
      // Debug.Log("On en est là");
      navAgent.destination = targetedEnemy.position;

      if (navAgent.pathPending)
      {
        yield return null;
      }

      if (navAgent.remainingDistance > shootDistance || double.IsInfinity(navAgent.remainingDistance))
      {
        // Debug.Log("PIYAYOYIDOLIPEEEEEEEEEEEEEEEEEEEE");
        navAgent.isStopped = false;
        run();
      }
      else
      {
        // Debug.Log("HEYOOOOOOOOOOOOOOOOOOOOOOOO");
        transform.LookAt(targetedEnemy);
        navAgent.isStopped = true;
        running = 0;
        Fire();
        // StartCoroutine("Fire");
      }

      yield return null;
    }
    else
    {
      stopMoving();
    }
  }

  void Fire()
  {
    if (healthDamage.isDead)
    {
      return;
    }
    // if (attacking == false && (Time.time > nextFire))
    if (animatingAttack == false && attacking == false && (Time.time > nextFire))
    {
      attacking = true;
      anim.SetTrigger("Attack");
    }
  }

  void Spell()
  {
    if (healthDamage.isDead)
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
    if (healthDamage.isDead)
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