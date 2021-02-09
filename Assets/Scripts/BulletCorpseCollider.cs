using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCorpseCollider : MonoBehaviour
{
  public Rigidbody body;
  List<GameObject> corpsesHit = new List<GameObject>();
  public float forceRatio = 0.75f;
  void Start()
  {
    if (!body)
    {
      body = GetComponent<Rigidbody>();
    }
  }
  void OnTriggerEnter(Collider collider)
  {
    GameObject corpseHit = Tools.FindObjectOrParentWithTag(collider.gameObject, "Corpse");
    if (corpseHit && !corpsesHit.Contains(corpseHit))
    {
      corpseHit.GetComponent<Rigidbody>().AddForce(body.velocity * forceRatio, ForceMode.Impulse);
      corpsesHit.Add(corpseHit);
    }
  }
}
