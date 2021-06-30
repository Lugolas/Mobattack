using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfroHandController : MonoBehaviour
{
  public GameObject afroParent;
  public GameObject fistPrefab;
  public bool fire = false;
  public SphereCollider sphereCollider;
  Rigidbody rigidbodyRagdoll;
  SphereCollider sphereColliderLocal;
  public GameObject armModel;
  public MoneyManager characterWallet;
  bool fireFired = false;
  public AfroFistController fist;
  public SpellControllerAfro spellController;
  public bool punchAttempted = false;
  float fireTime;
  float disabledDelay = 0.5f;
  bool breakerUlt = false;
  public List<HealthSimple> alreadyHit = new List<HealthSimple>();
  float clearTime;
  float clearDelay = 0.1f;
  float fistSize = 1;
  float fistRadius = 0.5f;
  int damageFinalLast;
  float lauchForceLast = 10;
  int fistEnemyBouncesLast = 0;

  void Start()
  {
    spellController = afroParent.GetComponent<SpellControllerAfro>();
    if (!fist) {
      fist = GetComponentInChildren<AfroFistController>(true);
    }
    sphereColliderLocal = GetComponent<SphereCollider>();
    UpdateFist();
  }

  void ClearHitList() {
    clearTime = Time.time;
    alreadyHit.Clear();
  }

  void FixedUpdate()
  {
    if (fire && !fireFired)
    {
      fireTime = Time.time;
      fireFired = true;
      sphereCollider.enabled = false;
      fist.transform.SetParent(afroParent.transform);
      fist.Fire(fistSize);

      fist = null;
      transform.localScale = Vector3.zero;
      GameObject fistObject = Instantiate(fistPrefab, transform.position, transform.rotation);
      fist = fistObject.GetComponent<AfroFistController>();
      fist.transform.SetParent(transform);
      fist.transform.localRotation = Quaternion.Euler(-90, 0, 0);
      fist.transform.localPosition = new Vector3(
        fist.transform.localPosition.x,
        fist.transform.localPosition.y + fistRadius,
        fist.transform.localPosition.z
      );
      fist.transform.localScale = new Vector3(fistSize, fistSize, fistSize);
      fist.spellController = spellController;
      fist.characterWallet = characterWallet;
      UpdateFist();
    } else if (!fire && fireFired) {
      fireFired = false;
    }

    if (Time.time > fireTime + disabledDelay) {
      sphereCollider.enabled = true;
    }

    if (spellController.IsInBreakerUlt()) {
      if (!breakerUlt) {
        breakerUlt = true;
        fist.AddDamageMultiplier(2, "breaker");
      }
    } else if(breakerUlt) {
      breakerUlt = false;
      fist.RemoveDamageMultiplier("breaker");
    }

    if (Time.time > clearTime + clearDelay) {
      ClearHitList();
    }
  }

  void OnTriggerEnter(Collider collider)
  {
    if (armModel.activeSelf) {
      if (collider.tag == "EnemyCharacter") {
        HealthSimple targetHealth = Tools.GetHealth(collider.gameObject);
        if (targetHealth && !alreadyHit.Contains(targetHealth)) {
          alreadyHit.Add(targetHealth);
          fist.UpdateDamage();
          fist.HitEnemy(targetHealth.GetComponent<EnemyController>(), false);
          GameObject shockObject = Instantiate (fist.shock, transform.position, transform.rotation);
          shockObject.transform.localScale *= 2;
          Destroy (shockObject, 1.5f);
        }
      }
    }
  }

  void Update()
  {
    if (spellController.healthScript.damageFinal != damageFinalLast) {
      damageFinalLast = spellController.healthScript.damageFinal;
      UpdateFistSize();
    }
    if (spellController.fistLaunchForce != lauchForceLast) {
      lauchForceLast = spellController.fistLaunchForce;
      UpdateFistLaunchForce();
    }
    if (spellController.fistEnemyBounces != fistEnemyBouncesLast) {
      fistEnemyBouncesLast = spellController.fistEnemyBounces;
      UpdateFistBounces();
    }
  }

  void UpdateFistSize() {
    float fistAugmentation = (float)spellController.healthScript.damageFinal / spellController.healthScript.damageBase;
    float fistAugmentationHalved = ((fistAugmentation - 1) / 2) + 1;
    fistSize = fistAugmentationHalved;
    fistRadius = fistSize / 2;
    sphereCollider.radius = fistRadius;
    sphereCollider.center = new Vector3(0, fistRadius, 0);
    sphereColliderLocal.radius = fistRadius;
    sphereColliderLocal.center = new Vector3(0, fistRadius, 0);

    float weight = fistAugmentationHalved * spellController.fistWeightInitial;
    if (!rigidbodyRagdoll) {
      rigidbodyRagdoll = sphereCollider.GetComponent<Rigidbody>();
    }
    rigidbodyRagdoll.mass = weight;
    fist.UpdateSize(false);
  }
  void UpdateFistLaunchForce() {
    fist.SetLaunchForce(spellController.fistLaunchForce);
  }
  void UpdateFist() {
    UpdateFistSize();
    UpdateFistLaunchForce();
    UpdateFistBounces();
  }
  void UpdateFistBounces() {
    fist.SetEnemyBounceMax(spellController.fistEnemyBounces);
  }
}
