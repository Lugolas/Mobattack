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
  public List<HealthSimple> targetList = new List<HealthSimple>();
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
    if (spellController.IsInBreakerUlt()) {
      speed = afroFistController.magnitude;
      Detach();
    }
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

    if (timePassed != 0) {
      trail.height = timePassed * speed;
      trail.center = new Vector3 (0, 0, -(trail.height / 2));
    }

    if (
      (!detached && !afroFistController.trail.emitting) || 
      trail.height < 0 || 
      (detached && (Time.time - detachTime) >= (trailTime))
    ) {
      Destroy(gameObject);
    }
  }
  
  public void Detach() {
    transform.SetParent(spellController.transform);
    detached = true;
    detachTime = Time.time;
  }

  void OnTriggerEnter(Collider collider)
  {
    if (collider.tag == "EnemyCharacter" || collider.tag == "EnemyBodyPart") 
    {
      HealthSimple targetHealth = Tools.GetHealth(collider.gameObject);
      if (targetHealth && !targetList.Contains(targetHealth)) {
        targetList.Add(targetHealth);
      }
    }
  }
  void OnTriggerExit(Collider collider)
  {
    if (collider.tag == "EnemyCharacter" || collider.tag == "EnemyBodyPart")
    {
      HealthSimple targetHealth = Tools.GetHealth(collider.gameObject);
      if (targetHealth && targetList.Contains(targetHealth))
      {
        targetList.Remove(targetHealth);
      }
    }
  }
}
