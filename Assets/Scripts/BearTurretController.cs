using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BearTurretController : TurretController {
  SphereCollider rangeCollider;
  SpellControllerBear spellControllerBear;
  public Transform weaponPole;
  public Transform weapon;
  public Transform weaponSight;
  public Transform weaponSightPoint;
  public Transform projectileSpawnPoint;
  public GameObject projectilePrefab;
  public bool fireTrigger = false;
  public bool fireTriggerControl = true;
  bool animatingAttack = false;
  public Animator animator;
  public float fireTime = 0;
  float maxTimeBetweenTargetUpdates = 0.5f;
  public LayerMask enemyDetectionMask;

  private void Start () {
    spellControllerBear = GetComponentInParent<SpellControllerBear> ();
    rangeCollider = GetComponent<SphereCollider> ();
    lastTargetUpdate = Time.time;
    UpdateRange ();
  }

  void Update () {
    if (Time.time > lastTargetUpdate + maxTimeBetweenTargetUpdates) {
      targetUpdateWanted = true;
    }
  }

  void LateUpdate () {
    UpdateRange ();
    UpdateEnemiesInRange ();
    if (targetUpdateWanted) {
      UpdateTarget(spellControllerBear.turretRange);
    }

    if (spellControllerBear.bearTurretActive)
    {
      TurnWeaponToTarget ();
        
      if (target && Time.time >= fireTime + spellControllerBear.delay) {
        fireTime = Time.time;
        Fire ();
      }
    }
  }

  void TriggerFireAnimation () {
    if (target && Time.time >= fireTime + spellControllerBear.delay) {
      fireTime = Time.time;
      animator.SetTrigger ("Fire");
    }
  }

  public void UpdateRange () {
    rangeCollider.radius = spellControllerBear.turretRange;
  }

  void OnTriggerEnter (Collider collider) {
    EnemyController enemy = collider.gameObject.GetComponentInParent<EnemyController> ();

    if (enemy) {
      AnimatorNavAgentRootMotion enemyBody = collider.gameObject.GetComponentInParent<AnimatorNavAgentRootMotion> ();
      if (enemyBody) {
        if (!enemiesInRange.Contains (enemyBody.gameObject)) {
          enemiesInRange.Add (enemyBody.gameObject);
          targetUpdateWanted = true;
        }
      }
    }
  }

  void OnTriggerExit (Collider collider) {
    EnemyController enemy = collider.gameObject.GetComponentInParent<EnemyController> ();

    if (enemy) {
      AnimatorNavAgentRootMotion enemyBody = collider.gameObject.GetComponentInParent<AnimatorNavAgentRootMotion> ();
      if (enemyBody) {
        enemiesInRange.Remove (enemyBody.gameObject);
        targetUpdateWanted = true;
      }
    }
  }

  void UpdateEnemiesInRange () {
    if (enemiesInRange.Count > 0 && !enemiesInRange[0]) {
      targetUpdateWanted = true;
    }
  }

  void TurnWeaponToTarget () {
    if (target) {
      weaponPole.LookAt (new Vector3 (target.transform.position.x, weaponPole.position.y, target.transform.position.z));
      weaponSight.LookAt (new Vector3 (target.transform.position.x, target.transform.position.y, target.transform.position.z));
      weapon.LookAt (weaponSightPoint);
      weapon.Rotate (new Vector3 (0, 0, 180));
    }
  }

  void Fire () {
    GameObject projectile = Instantiate (projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation) as GameObject;
    Fireball fireballScript = projectile.GetComponent<Fireball> ();
    if (fireballScript) {
      fireballScript.damage = spellControllerBear.gunDamage;
      fireballScript.emitter = this;
      fireballScript.target = target.transform;
      fireballScript.characterWallet = spellControllerBear.moneyManager;
    }
    targetUpdateWanted = true;
  }
}