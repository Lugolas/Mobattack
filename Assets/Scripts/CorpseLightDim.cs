using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorpseLightDim : MonoBehaviour
{
  private CorpseController corpseController;
  private float intensity;
  private Light lifeLight;
  void Start()
  {
    lifeLight = GetComponent<Light>();
    intensity = lifeLight.intensity;
    corpseController = GetComponentInParent<CorpseController>();
  }

  void Update()
  {
    lifeLight.intensity = corpseController.lifeRatio * intensity;
  }
}
