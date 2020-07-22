using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorpseController : MonoBehaviour
{
  public float lifeRatio = 1;
  public float digRatio = 1;
  private float minimum = 0;
  private float maximum = 1;
  private float durationBeforeDim = 1;
  private float durationDim = 2.5f;
  private float durationDig = 15;
  private float digDistance = -2.5f;
  private float startTime = -1;
  private float startDimTime = -1;
  private float startDigTime = -1;
  private float startPosition;
  private int CHARACTER_DEAD_LAYER = 15;


  void Start()
  {
    Tools.SetLayerRecursively(gameObject, CHARACTER_DEAD_LAYER);

    startTime = Time.time;
    minimum = 0;
    maximum = 1;
  }

  // Update is called once per frame
  void Update()
  {
    if (startTime != -1 && minimum != -1 && maximum != -1)
    {
      if (Time.time > startTime + durationBeforeDim)
      {
        startTime = -1;
        startDimTime = Time.time;
      }
    }
    if (startDimTime != -1 && minimum != -1 && maximum != -1)
    {
      float tDim = (Time.time - startDimTime) / durationDim;
      lifeRatio = Mathf.SmoothStep(maximum, minimum, tDim);
      if (Time.time > startDimTime + durationDim)
      {
        startPosition = transform.position.y;
        startDigTime = Time.time;
        startDimTime = -1;
      }
    }
    if (startDigTime != -1 && minimum != -1 && maximum != -1)
    {

      float tDig = (Time.time - startDigTime) / durationDig;
      digRatio = Mathf.SmoothStep(minimum, maximum, tDig);
      transform.position = new Vector3(transform.position.x, startPosition + (digDistance * digRatio), transform.position.z);
      if (Time.time > startDigTime + durationDig)
      {
        startDigTime = -1;
      }
    }
  }
}


