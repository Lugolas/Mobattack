using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailColliderController : MonoBehaviour
{
  Vector3 initialPosition;
  float initialTime;
  AfroFistController afroFistController;
  SpellControllerAfro spellController;
  float trailTime;
  float speed;
  CapsuleCollider trail;
  bool detached = false;
  float detachTime;
  List<HealthSimple> targetList = new List<HealthSimple>();
  int damage = 0;
  float lastHit = 0;
  float hitDelay = 0.25f;
  float timePassed = 0;

  void Start()
  {
    initialPosition = transform.position;
    initialTime = Time.time;
    afroFistController = GetComponentInParent<AfroFistController>();
    spellController = afroFistController.spellController;
    trailTime = afroFistController.trailTime * 0.666f;
    speed = afroFistController.velocity.magnitude;
    trail = GetComponent<CapsuleCollider>();
  }

  void Update() {
    float time = (Time.time - initialTime);
    if (detached) {
      if (time >= trailTime) {
        timePassed = trailTime - (Time.time - detachTime);
      }
    } else
    {
      if (time > trailTime)
      {
        timePassed = trailTime;
      } else {
        timePassed = time;
      }
    }

    trail.height = timePassed * speed;
    trail.center = new Vector3 (0, 0, -(trail.height / 2) + 0.25f);
    trail.height += 0.5f;

    if ((!detached && !afroFistController.trail.emitting) || trail.height <= 0.5f) {
    // if ((!detached && !afroFistController.trailIN.emitting) || trail.height <= 0.5f) {
      Destroy(gameObject);
    }
  }

  void FixedUpdate()
  {
    if (Time.time > lastHit + hitDelay) {
      damage = Mathf.RoundToInt(afroFistController.damageFinal / 2f);
      lastHit = Time.time;
      foreach (HealthSimple target in targetList)
      {
        Tools.InflictDamage(
          target.transform,
          Mathf.RoundToInt(damage * hitDelay),
          afroFistController.characterWallet,
          afroFistController.gameObject
        );
      }
    }
  }
  
  public void Detach() {
    transform.SetParent(afroFistController.transform.parent);
    detached = true;
    detachTime = Time.time;
  }

  void OnTriggerEnter(Collider collider)
  {
    if (collider.tag == "EnemyCharacter") 
    {
      HealthSimple targetHealth = Tools.GetHealth(collider.gameObject);
      if (targetHealth && !targetList.Contains(targetHealth)) {
      // if (targetHealth && !targetList.Contains(targetHealth)) {
        targetList.Add(targetHealth);
      }
    }
  }
  void OnTriggerExit(Collider collider)
  {
    if (collider.tag == "EnemyCharacter")
    {
      HealthSimple targetHealth = Tools.GetHealth(collider.gameObject);
      if (targetHealth && targetList.Contains(targetHealth))
      {
        targetList.Remove(targetHealth);
      }
    }
  }
}
