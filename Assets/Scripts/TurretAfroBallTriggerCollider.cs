using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretAfroBallTriggerCollider : MonoBehaviour
{
  TurretAfroBallController ballController;
  public Transform ball;

  private void Start()
  {
    ballController = GetComponentInParent<TurretAfroBallController>();
  }

  void Update()
  {
    transform.position = ball.position;
  }

  void OnTriggerEnter(Collider collider)
  {
    if (collider.gameObject.layer != LayerMask.NameToLayer("BigEnemy")) {
      ballController.Collision(collider);
    }
  }
}
