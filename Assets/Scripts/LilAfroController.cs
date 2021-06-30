using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LilAfroController : MonoBehaviour
{
  public GameObject target;
  public Transform fistPropulsionPoint;
  public bool targetUpdateWanted = false;
  TurretPlayerLink playerLink;
  Animator animator;
  string attackTriggerName = "Attack";
  string attackModeName = "AttackSelector";
  TurretWallController wallController;
  int attackAnimationMode = -1;
  List<GameObject> fistsInRange = new List<GameObject>();
  int damageBase = 1;
  int damageFinal = 1;
  float punchTime = 0;

  private void Start()
  {
    playerLink = GetComponentInParent<TurretPlayerLink>();
    animator = GetComponent<Animator>();
    wallController = GetComponentInParent<TurretWallController>();
  }

  void OnTriggerExit(Collider collider)
  {
    if (collider.tag == "Fist") {
      if (fistsInRange.Contains(collider.gameObject)) {
        fistsInRange.Remove(collider.gameObject);
      }
    }
  }
  void OnTriggerEnter(Collider collider)
  {
    if (collider.tag == "Fist") {
      fistsInRange.Add(collider.gameObject);
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
    if (playerLink.activated) {
      if (wallController.targeting2) {
        Vector3 point = wallController.targetPoint.transform.position;
        transform.LookAt(new Vector3(point.x, transform.position.y, point.z));
      }
      if (fistsInRange.Count > 0) {
        bool invalidRigidbodyDetected = false;
        Rigidbody invalidRigidbody = new Rigidbody();
        foreach (GameObject fistInRange in fistsInRange)
        {
          foreach (RegisteredFist registeredFist in wallController.registeredFists)
          {
            if (registeredFist.fist == fistInRange && registeredFist.punched == false) {
              Rigidbody fistBody = fistInRange.GetComponent<Rigidbody>();
              if (fistBody && !fistBody.isKinematic) {
                target = fistInRange;
                registeredFist.punched = true;
                wallController.targetUpdateWanted = true;
              } else {
                invalidRigidbodyDetected = true;
                invalidRigidbody = fistBody;
              }
            }
          }
        }
        if (invalidRigidbodyDetected) {
          fistsInRange.Remove(invalidRigidbody.gameObject);
        }
      }
      if (target && Time.time > punchTime + wallController.GetDelay())
      {
        Rigidbody targetRigidbody = target.GetComponent<Rigidbody>();
        AfroFistController targetController = target.GetComponent<AfroFistController>();
        if (targetRigidbody) {
          if (!targetRigidbody.isKinematic) {
            target.transform.position = new Vector3(fistPropulsionPoint.position.x, target.transform.position.y, fistPropulsionPoint.position.z);

            float magnitude = targetRigidbody.velocity.magnitude;
            float speedBoost = 1;
            if (targetController) {
              if (wallController.power1) {
                targetController.AddDamageAddition(damageFinal);
              }
              if (wallController.targeting1) {
                speedBoost = 1.05f;
              }
            }
            float baseForce = (magnitude * targetController.GetFistMass());
            if (baseForce < targetController.GetLaunchForce()) {
              baseForce = targetController.GetLaunchForce();
            }

            float force = (baseForce + damageFinal) * speedBoost;

            targetRigidbody.Sleep();
            targetRigidbody.AddForce(transform.forward * force, ForceMode.Impulse);
          }
        }

        TriggerFireAnimation();

        punchTime = Time.time;
      }
      target = null;
    }
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
