using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurateShootController : MonoBehaviour
{
  TurateShootPoint[] shootPoints;
  public GameObject fireballPrefab;
  public GameObject target;
  // Start is called before the first frame update
  void Start()
  {
    shootPoints = GetComponentsInChildren<TurateShootPoint>();
  }

  // Update is called once per frame
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.P))
    {
    }
  }
  void OnTriggerEnter(Collider collider)
  {
    target = collider.gameObject;

    foreach (TurateShootPoint shootPoint in shootPoints)
    {
      shootPoint.target = target;
      shootPoint.fire();
    }
  }
}
