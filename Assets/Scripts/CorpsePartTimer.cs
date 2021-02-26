using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorpsePartTimer : MonoBehaviour
{
  public bool isImportant = false;
  public GameObject linkedModel;
  float startTime = -1;
  Rigidbody body;

  void Start()
  {
    body = GetComponent<Rigidbody>();
  }

  void Update()
  {
    if (startTime == -1) {
      if (body.useGravity) {
        startTime = Time.time;
      }
    } else {
      if (!isImportant && Time.time > startTime + 5) {
        if (linkedModel) {
          Destroy(linkedModel);
        }
        Destroy(gameObject);
      }
    }
  }
}
