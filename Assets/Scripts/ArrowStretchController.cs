using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowStretchController : MonoBehaviour
{
  float nominalPointDistance = 4.96f;
  float nominalBodyDistance = -2.48f;
  float nominalBodyScale = 1f;
  public GameObject arrowBody;

  // Update is called once per frame
  void Update()
  {
    arrowBody.transform.localPosition = new Vector3(
      arrowBody.transform.localPosition.x,
      (transform.localPosition.z / nominalPointDistance) * nominalBodyDistance,
      arrowBody.transform.localPosition.z
    );

    arrowBody.transform.localScale = new Vector3(
      arrowBody.transform.localScale.x,
      (transform.localPosition.z / nominalPointDistance) * nominalBodyScale,
      arrowBody.transform.localScale.z
    );
  }
}
