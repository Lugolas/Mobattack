using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretWallController : TurretController
{
  public Transform projectileSpawnPoint;
  public GameObject projectilePrefab;
  private bool hasExtortedCharacter = false;
  private float fireTime;
  TurretStatManager statManager;
  float oldDelay;
  public bool fireTrigger = false;
  public bool fireTriggerControl = true;
  bool animatingAttack = false;
  float attackAnimationLength;
  public float attackAnimationSpeed = 1f;
  string attackAnimationName = "AttackSpeed";
  public List<RegisteredFist> registeredFists = new List<RegisteredFist>();
  float lastUpdate;
  float delay = 1;
  public SpellControllerAfro afrOwner;
  public GameObject targetPoint;
  public bool power1 = false;
  public bool targeting1 = false;
  public bool targeting2 = false;
  public bool targeting3 = false;
  public bool targeting4 = false;
  public bool targeting5 = false;
  bool targeting5Control = false;
  public bool amount1 = false;
  bool amount1Control = false;
  public GameObject fistLilPrefab;

  private void Start()
  {
    lastUpdate = Time.time;
    statManager = GetComponentInParent<TurretStatManager>();
    afrOwner = owner.GetComponent<SpellControllerAfro>();

    fireTime = Time.time;
  }

  void Update()
  {
    if (activated && !hasExtortedCharacter)
    {
      afrOwner.moneyManager.SubstractMoney(statManager.price);
      hasExtortedCharacter = true;
    }

    if (Time.time > lastUpdate + 1) {
      targetUpdateWanted = true;
    }

    if (amount1 && !amount1Control) {
      amount1Control = amount1;
      delay *= 0.5f;
    } else if (!amount1 && amount1Control) {
      amount1Control = amount1;
      delay *= 2;
    }
    if (targeting5 && !targeting5Control) {
      targeting5Control = targeting5;
      delay *= 0.1f;
    } else if (!targeting5 && targeting5Control) {
      targeting5Control = targeting5;
      delay *= 10;
    }
    if (targeting4 && afrOwner.GetSpell3Active()) {
      targetPoint.transform.position = afrOwner.spell3TargetPoint;
    }
  }

  void LateUpdate()
  {
    if (targetUpdateWanted)
    {
      UpdateTarget();
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

  public float GetDelay() {
    return delay;
  }
}

public class RegisteredFist {
  public GameObject fist;
  public float arrivalTime;
  public bool punched = false;
}
