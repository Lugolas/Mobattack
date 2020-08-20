using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TuratePartController : MonoBehaviour
{
  float startTime;
  // Start is called before the first frame update
  void Start()
  {
    startTime = Time.time;
  }

  // Update is called once per frame
  void Update()
  {
    if (transform.position.y < -300)
    {
      Destroy(gameObject);
    }
  }
}
