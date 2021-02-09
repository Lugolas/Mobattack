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
  public List<RegisteredFist> registeredFists = new List<RegisteredFist>();
  float lastUpdate;

  private void Start()
  {
    lastUpdate = Time.time;
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

    if (Time.time > lastUpdate + 1) {
      targetUpdateWanted = true;
    }
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
    List<int> invalidFistsIndexes = new List<int>();

    for (int i = 0; i < registeredFists.Count; i++)
    {
      RegisteredFist fist = registeredFists[i];
      if (fist.fist)
      {
        Rigidbody fistBody = fist.fist.GetComponent<Rigidbody>();
        if (!fistBody || fistBody.isKinematic) {
          invalidFistsIndexes.Add(i);
        }
      } else {
        invalidFistsIndexes.Add(i);
        // if (Time.time > fist.arrivalTime + 1.0f) {
        //   fist.punched = false;
        // }
      }
    }

    foreach (int index in invalidFistsIndexes)
    {
      if (index < registeredFists.Count)
      {
        registeredFists.RemoveAt(index);
      }
    }

    targetUpdateWanted = false;
    lastUpdate = Time.time;
  }

  void OnTriggerEnter(Collider collider)
  {
    if (collider.tag == "Fist") {
      RegisteredFist fistToRegister = new RegisteredFist();
      fistToRegister.fist = collider.gameObject;
      fistToRegister.arrivalTime = Time.time;
      fistToRegister.punched = false;

      bool knownFist = false;
      foreach (RegisteredFist registeredFist in registeredFists)
      {
        if (registeredFist.fist == collider.gameObject) {
          knownFist = true;
          break;
        }
      }

      if (!knownFist) {
        registeredFists.Add(fistToRegister);
      }

      targetUpdateWanted = true;
    }
  }

  void OnTriggerExit(Collider collider)
  {
    if (collider.tag == "Fist")
    {
      bool knownFist = false;
      RegisteredFist exittingFist = new RegisteredFist();

      foreach (RegisteredFist registeredFist in registeredFists)
      {
        if (registeredFist.fist == collider.gameObject) {
          knownFist = true;
          exittingFist = registeredFist;
          break;
        }
      }

      if (knownFist) {
        registeredFists.Remove(exittingFist);
      }
      targetUpdateWanted = true;
    }
  }
}

public class RegisteredFist {
  public GameObject fist;
  public float arrivalTime;
  public bool punched = false;
}
