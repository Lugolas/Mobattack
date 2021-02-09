using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorpsePartTimer : MonoBehaviour
{
  // Rigidbody body;
  // Collider colliderComponent;
  // float startTime;
  // float sleepTime;
  // bool sleeping = false;
  // float sleepDuration = 20f;
  // float lifeDuration = 120f;
  // void Start()
  // {
  //   body = GetComponent<Rigidbody>();
  //   colliderComponent = GetComponent<Collider>();
  //   startTime = Time.time;
  // }

  // void Update()
  // {
  //   if (Time.time > startTime + lifeDuration)
  //   {
  //     deactivatePart();
  //   }

  //   if (body.IsSleeping() && !sleeping)
  //   {
  //     sleepTime = Time.time;
  //     sleeping = true;
  //   }
  //   if (!body.IsSleeping() && sleeping)
  //   {
  //     sleeping = false;
  //   }

  //   if (Time.time > sleepTime + sleepDuration)
  //   {
  //     deactivatePart();
  //   }
  // }

  // void deactivatePart()
  // {
  //   body.isKinematic = true;
  //   body.useGravity = false;
  //   colliderComponent.enabled = false;
  //   Destroy(gameObject);
  // }
}
