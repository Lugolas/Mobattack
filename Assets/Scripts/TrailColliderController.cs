using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailColliderController : MonoBehaviour
{
  Vector3 initialPosition;
  float initialTime;
  AfroFistController afroFistController;
  float trailTime;
  float speed;
  CapsuleCollider trail;
  bool detached = false;
  float detachTime;

  void Start()
  {
    initialPosition = transform.position;
    initialTime = Time.time;
    afroFistController = GetComponentInParent<AfroFistController>();
    trailTime = afroFistController.trailTime;
    speed = afroFistController.velocity.magnitude;
    trail = GetComponent<CapsuleCollider>();
  }

  void Update() {
    float time = (Time.time - initialTime);

    if (detached) {
      time = trailTime - (Time.time - detachTime);
    } else if (time > trailTime) {
      time = trailTime;
    }

    trail.height = time * speed;
    trail.center = new Vector3 (0, 0, -(trail.height / 2) + 0.25f);
    trail.height += 0.5f;

    if (trail.height <= 0.5f) {
      Destroy(gameObject);
    }
  }
  
  public void Detach() {
    transform.SetParent(afroFistController.transform.parent);
    detached = true;
    detachTime = Time.time;
  }
}
