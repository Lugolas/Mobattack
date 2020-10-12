using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangloxSlamCollision : MonoBehaviour
{
  TriangloxSlamController slamController;

  // Start is called before the first frame update
  void Start()
  {
    slamController = GetComponentInParent<TriangloxSlamController>();
  }

  // Update is called once per frame
  void Update()
  {
  }

  void OnTriggerEnter(Collider collider)
  {
    if (collider.tag == "EnemyCharacter")
    {
      slamController.Slam(collider.gameObject);
    }
  }
}
