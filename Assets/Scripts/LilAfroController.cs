using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LilAfroController : TurretController
{
  public Transform fistPropulsionPoint;
  Animator animator;
  string attackTriggerName = "Attack";
  string attackModeName = "AttackSelector";
  TurretWallController wallController;
  int attackAnimationMode = -1;
  List<GameObject> fistsInRange = new List<GameObject>();
  int damageBase = 1;
  int damageFinal = 1;
  float punchTime = 0;
  float fireTime = 0;
  float personnalDelay;

  private void Start()
  {
    personnalDelay = Random.Range(0.8f, 1.2f);
    animator = GetComponent<Animator>();
    wallController = GetComponentInParent<TurretWallController>();
    subscribeToRangeControllers();
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
    if (wallController.activated) {
      if (targetUpdateWanted) {
        UpdateTarget();
      }
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
      if (enemiesInRange.Count > 0) {
        if (Time.time > punchTime + wallController.GetDelay()) {
          foreach (GameObject enemy in enemiesInRange)
          {
            Tools.InflictDamage(enemy.transform, wallController.statManager.health.damageFinal, wallController.owner.moneyManager, wallController.owner);
            targetUpdateWanted = true;
          }
          punchTime = Time.time;
          TriggerFireAnimation();
        }
      }
      if (target && Time.time > punchTime + wallController.GetDelay())
      {
        Rigidbody targetRigidbody = target.GetComponent<Rigidbody>();
        AfroFistController targetController = target.GetComponent<AfroFistController>();
        if (targetController) {
          if (!targetController.rigidbodyFist.isKinematic) {
            Fire(targetController);
            punchTime = Time.time;
          }
        }

      } else if (wallController.targeting3 && Time.time > fireTime + ((wallController.GetDelay() * personnalDelay) * 5f)) {
        GameObject fistLil = Instantiate(wallController.fistLilPrefab, fistPropulsionPoint.position, fistPropulsionPoint.rotation);
        AfroFistController fistLilController = fistLil.GetComponent<AfroFistController>();
        fistLilController.spellController = wallController.afrOwner;
        fistLilController.characterWallet = wallController.afrOwner.moneyManager;
        fistLilController.freeBall = true;
        // fistLilController.fireWanted = true;
        // fistLilController.fireWantedSize = 0.48f;
        // fistLilController.Fire(0.48f, true);
        Fire(fistLilController);
        fireTime = Time.time;
      }
      target = null;
    }
  }

  void Fire(AfroFistController fist)
  {
    fist.transform.position = new Vector3(fistPropulsionPoint.position.x, fist.transform.position.y, fistPropulsionPoint.position.z);

    float magnitude = fist.rigidbodyFist.velocity.magnitude;
    float speedBoost = 1;
    if (fist) {
      if (wallController.power1) {
        fist.AddDamageAddition(damageFinal);
      }
      if (wallController.targeting1) {
        speedBoost = 1.05f;
      }
    }
    float baseForce = (magnitude * fist.GetFistMass());
    if (baseForce < fist.GetLaunchForce()) {
      baseForce = fist.GetLaunchForce();
    }

    float force = (baseForce + damageFinal) * speedBoost;

    fist.rigidbodyFist.Sleep();
    fist.rigidbodyFist.AddForce(transform.forward * force, ForceMode.Impulse);
    
    TriggerFireAnimation();
  }
}
