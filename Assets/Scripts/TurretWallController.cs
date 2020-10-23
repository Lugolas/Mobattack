using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretWallController : MonoBehaviour
{
  public GameObject target;
  public List<GameObject> enemiesInRange = new List<GameObject>();
  public Transform projectileSpawnPoint;
  public GameObject projectilePrefab;
  public bool targetUpdateWanted = false;
  private bool hasExtortedCharacter = false;
  private float fireTime;
  TurretStatManager statManager;
  float oldDelay;
  TurretPlayerLink playerLink;
  Animator animator;
  public bool fireTrigger = false;
  public bool fireTriggerControl = true;
  bool animatingAttack = false;
  float attackAnimationLength;
  public float attackAnimationSpeed = 1f;
  string attackAnimationName = "AttackSpeed";


  private void Start()
  {
    playerLink = GetComponentInParent<TurretPlayerLink>();
    statManager = GetComponentInParent<TurretStatManager>();
    animator = GetComponent<Animator>();

    AnimationClip[] animationClips = animator.runtimeAnimatorController.animationClips;
    foreach (AnimationClip animationClip in animationClips)
    {
      if (animationClip.name == "Attack")
      {
        attackAnimationLength = animationClip.length;
      }
    }
    attackAnimationSpeed = animator.GetFloat(attackAnimationName);

    fireTime = Time.time;
  }

  void Update()
  {
    if (statManager.delay != oldDelay)
    {
      oldDelay = statManager.delay;
      if (attackAnimationLength > statManager.delay)
      {
        attackAnimationSpeed = (attackAnimationLength / statManager.delay);
        animator.SetFloat(attackAnimationName, attackAnimationSpeed);
      }
    }

    if (playerLink.activated && !hasExtortedCharacter)
    {
      playerLink.characterWallet.SubstractMoney(statManager.price);
      hasExtortedCharacter = true;
    }

    TriggerFireAnimation();
  }

  void TriggerFireAnimation()
  {
    if (playerLink.activated && target && Time.time >= fireTime + statManager.delay)
    {
      fireTime = Time.time;
      animator.SetTrigger("Fire");
    }
  }

  void LateUpdate()
  {
    if (targetUpdateWanted)
    {
      UpdateTarget();
    }
    if (fireTrigger && fireTriggerControl && target)
    {
      fireTrigger = false;
      fireTriggerControl = false;
      Fire();
    }


    if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
    {
      animatingAttack = true;
    }
    else if (animatingAttack)
    {
      fireTriggerControl = true;
      TriggerFireAnimation();
    }
  }

  void UpdateTarget()
  {
    List<int> invalidEnemiesIndexes = new List<int>();

    for (int i = 0; i < enemiesInRange.Count; i++)
    {
      GameObject enemy = enemiesInRange[i];
      if (!enemy || enemy.GetComponent<HealthSimple>().isDead)
      {
        invalidEnemiesIndexes.Add(i);
      }
    }

    foreach (int index in invalidEnemiesIndexes)
    {
      if (index < enemiesInRange.Count)
      {
        enemiesInRange.RemoveAt(index);
      }
    }

    if (enemiesInRange.Count > 0)
    {
      target = enemiesInRange[0];
    }
    else
    {
      target = null;
    }
    targetUpdateWanted = false;
  }

  void Fire()
  {
    // GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation) as GameObject;
    // Fireball fireballScript = projectile.GetComponent<Fireball>();
    // if (fireballScript)
    // {
    //   fireballScript.damage = statManager.damage;
    //   fireballScript.emitter = this;
    //   fireballScript.target = target.transform;
    //   fireballScript.characterWallet = playerLink.characterWallet;
    // }
    // else
    // {
    //   TriangloxSlamController slamController = projectile.GetComponent<TriangloxSlamController>();
    //   if (slamController)
    //   {
    //     slamController.damage = statManager.damage;
    //     slamController.characterWallet = playerLink.characterWallet;
    //   }
    // }
  }
}
