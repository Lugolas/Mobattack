using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorpsePartTimer : MonoBehaviour
{
  public bool isImportant = false;
  public GameObject linkedModel;
  float startTime = -1;
  float destroyDelay = 5;
  Rigidbody body;
  public GameObject headBandModel;
  public GameObject headBandParts;

  void Start()
  {
    body = GetComponent<Rigidbody>();
    if (isImportant) {
      destroyDelay = 20;
    }
  }

  void Update()
  {
    if (startTime == -1) {
      if (body.useGravity) {
        startTime = Time.time;
      }
    } else {
      if (Time.time > startTime + destroyDelay) {
        if (headBandModel && headBandParts) {
          Destroy(headBandModel);
          Destroy(headBandParts);
        }
        if (linkedModel) {
          Destroy(linkedModel);
        }
        Destroy(gameObject);
      }
    }
  }
}
