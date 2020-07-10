using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPointerEffectController : MonoBehaviour
{
  public bool theShowHasBegun = false;
  private float startTime = -1;
  private float duration = 0.5f;
  private float alphaRatio = 1f;
  private List<Material> materials = new List<Material>();
  private float initialAlphaValue = 0.33f;


  // Start is called before the first frame update
  void Start()
  {
    GetComponent<Renderer>().GetMaterials(materials);
    // initialAlphaValue = materials[0].color.a;
  }

  // Update is called once per frame
  void Update()
  {
    if (theShowHasBegun)
    {
      if (startTime == -1)
      {
        startTime = Time.time;
      }
      else
      {
        float t = (Time.time - startTime) / duration;
        alphaRatio = Mathf.SmoothStep(1f, 0f, t);
        materials[0].color = new Color(materials[0].color.r, materials[0].color.g, materials[0].color.b, initialAlphaValue * alphaRatio);
      }
    }
    else
    {
      startTime = -1;
    }
  }
}
