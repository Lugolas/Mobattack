using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LilAfroController : MonoBehaviour
{
  public GameObject target;
  public List<GameObject> fistsInRange = new List<GameObject>();
  public Transform fistPropulsionPoint;
  public bool targetUpdateWanted = false;
  TurretPlayerLink playerLink;
  Animator animator;
  string attackTriggerName = "Attack";
  string attackModeName = "AttackSelector";
  int attackAnimationMode = -1;


  private void Start()
  {
    playerLink = GetComponentInParent<TurretPlayerLink>();
    animator = GetComponent<Animator>();
  }

  void OnTriggerEnter(Collider collider)
  {
    if (playerLink.activated) {
      if (collider.tag == "Fist") {
        target = collider.gameObject;
      }
      if (target)
      {
        target.transform.position = fistPropulsionPoint.position;
        AfroFistController fistController = target.GetComponent<AfroFistController>();
        fistController.outsideDamageModifier += 0.1f;
        Rigidbody targetRigidbody = target.GetComponent<Rigidbody>();

        if (targetRigidbody) {
          float magnitude = targetRigidbody.velocity.magnitude;
          float force;
          if (magnitude < 10f) {
            force = 10f;
          } else {
            force = magnitude * 1.05f;
          }
          targetRigidbody.Sleep();
          targetRigidbody.AddForce(transform.forward * force, ForceMode.Impulse);
        }

        TriggerFireAnimation();
      }
      target = null;
    }
  }

  void TriggerFireAnimation()
  {
    attackAnimationMode = Random.Range(1, 4);
    animator.SetInteger(attackModeName, attackAnimationMode);
    animator.SetTrigger(attackTriggerName);
  }

  void LateUpdate()
  {
  }

  void UpdateTarget()
  {
    List<int> invalidEnemiesIndexes = new List<int>();

    for (int i = 0; i < fistsInRange.Count; i++)
    {
      GameObject enemy = fistsInRange[i];
      if (!enemy || enemy.GetComponent<HealthSimple>().isDead)
      {
        invalidEnemiesIndexes.Add(i);
      }
    }

    foreach (int index in invalidEnemiesIndexes)
    {
      if (index < fistsInRange.Count)
      {
        fistsInRange.RemoveAt(index);
      }
    }

    if (fistsInRange.Count > 0)
    {
      target = fistsInRange[0];
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
