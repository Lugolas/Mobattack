using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
  public List<Light> LightsToDisplayOnlyHere;

  void OnPreCull()
  {
    foreach (Light light in LightsToDisplayOnlyHere)
    {
      light.enabled = true;
    }
  }
  void OnPreRender()
  {
    foreach (Light light in LightsToDisplayOnlyHere)
    {
      light.enabled = true;
    }
  }

  void OnPostRender()
  {
    foreach (Light light in LightsToDisplayOnlyHere)
    {
      light.enabled = false;
    }
  }
}
