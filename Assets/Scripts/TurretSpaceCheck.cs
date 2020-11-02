using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretSpaceCheck : MonoBehaviour
{
  public List<SpaceCheck> spaceChecks = new List<SpaceCheck>();
  TurretPlayerLink turretPlayerLink;
  public bool enoughSpace = true;
  public GameObject visualSpace;
  private bool activated = false;

  // Start is called before the first frame update
  void Start()
  {
    turretPlayerLink = GetComponent<TurretPlayerLink>();
  }

  // Update is called once per frame
  void Update()
  {
    if (!turretPlayerLink.activated)
    {
      enoughSpace = true;
      foreach (SpaceCheck spaceCheck in spaceChecks)
      {
        if (!spaceCheck.enoughSpace)
        {
          enoughSpace = false;
          break;
        }
      }

      Color color;
      if (enoughSpace)
      {
        color = Tools.GetGreen();
      }
      else
      {
        color = Tools.GetRed();
      }
      Renderer[] renderers;
      renderers = visualSpace.GetComponentsInChildren<Renderer>();
      foreach (Renderer renderer in renderers)
      {
        List<Material> materials = new List<Material>();
        renderer.GetMaterials(materials);
        foreach (Material material in materials)
        {
          material.SetColor("_BaseColor", color);
        }
      }
    } else if (!activated) {
      activated = true;

      Renderer[] renderers;
      renderers = visualSpace.GetComponentsInChildren<Renderer>();
      foreach (Renderer renderer in renderers)
      {
        List<Material> materials = new List<Material>();
        renderer.GetMaterials(materials);
        foreach (Material material in materials)
        {
          material.SetColor("_BaseColor", Tools.GetWhite());
        }
      }
    }
  }
}
