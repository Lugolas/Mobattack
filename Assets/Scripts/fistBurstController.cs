using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fistBurstController : MonoBehaviour
{
  List<GameObject> alreadyActedOnObjects = new List<GameObject>();

  void OnTriggerEnter(Collider collider)
  {
    GameObject corpseHit = Tools.FindObjectOrParentWithTag(collider.gameObject, "Corpse");
    if (corpseHit)
    {
      if (!alreadyActedOnObjects.Contains(corpseHit))
      {
        alreadyActedOnObjects.Add(corpseHit);
        corpseHit.GetComponent<Rigidbody>().AddExplosionForce(25f, transform.position, 5, 0, ForceMode.Impulse);
      }
    }
  }
}
