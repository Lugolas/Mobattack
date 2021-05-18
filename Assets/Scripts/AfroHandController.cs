using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfroHandController : MonoBehaviour
{
  public GameObject afroParent;
  public GameObject fistPrefab;
  public bool fire = false;
  public SphereCollider sphereCollider;
  public GameObject armModel;
  public MoneyManager characterWallet;
  bool fireFired = false;
  public AfroFistController fist;
  public SpellControllerAfro spellController;
  public bool punchAttempted = false;
  float fireTime;
  float disabledDelay = 0.2f;
  bool breakerUlt = false;
  public List<HealthSimple> alreadyHit = new List<HealthSimple>();
  float clearTime;
  float clearDelay = 0.1f;

  void Start()
  {
    spellController = afroParent.GetComponent<SpellControllerAfro>();
    if (!fist) {
      fist = GetComponentInChildren<AfroFistController>(true);
    }
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
      fist.Fire();

      fist = null;
      transform.localScale = Vector3.zero;
      GameObject fistObject = Instantiate(fistPrefab, transform.position, transform.rotation);
      fist = fistObject.GetComponent<AfroFistController>();
      fist.transform.SetParent(transform);
      fist.transform.localRotation = Quaternion.Euler(-90, 0, 0);
      fist.transform.localPosition = new Vector3(
        fist.transform.localPosition.x,
        fist.transform.localPosition.y + 0.5f,
        fist.transform.localPosition.z
      );
      fist.transform.localScale = Vector3.one;
      fist.spellController = spellController;
      fist.characterWallet = characterWallet;
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
          Tools.InflictDamage(targetHealth.transform, fist.damageFinal, spellController.moneyManager, spellController);
        }
      }
    }
  }
}
