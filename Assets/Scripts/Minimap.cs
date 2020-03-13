using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
  public List<Light> LightsToIgnore;
  public List<Light> LightsToDisplayOnlyHere;

  void OnPreCull()
  {
    foreach (Light light in LightsToDisplayOnlyHere)
    {
      light.enabled = true;
    }
    foreach (Light light in LightsToIgnore)
    {
      light.enabled = false;
    }
  }
  void OnPreRender()
  {
    foreach (Light light in LightsToDisplayOnlyHere)
    {
      light.enabled = true;
    }
    foreach (Light light in LightsToIgnore)
    {
      light.enabled = false;
    }
  }

  void OnPostRender()
  {
    foreach (Light light in LightsToDisplayOnlyHere)
    {
      light.enabled = false;
    }
    foreach (Light light in LightsToIgnore)
    {
      light.enabled = true;
    }
  }
}
