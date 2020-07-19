using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapToken : MonoBehaviour
{
  List<Material> materials = new List<Material>();
  private string portraitName = "";
  private Texture portrait;
  // Start is called before the first frame update
  void Start()
  {
    portraitName = GetComponentsInParent<Transform>()[1].gameObject.name;
    if (portraitName.Length > 7 && portraitName.Substring(portraitName.Length - 7) == "(Clone)")
    {
      portraitName = portraitName.Substring(0, portraitName.Length - 7);
    }
    if (portraitName != "" && portraitName != null)
    {
      portrait = (Texture)Resources.Load("Tokens/" + portraitName + "Token");
      GetComponent<Renderer>().GetMaterials(materials);
      foreach (Material material in materials)
      {
        material.SetTexture("_BaseMap", portrait);
        material.SetTexture("_EmissionMap", portrait);
      }
    }
    gameObject.GetComponent<MeshRenderer>().enabled = true;
  }

  // Update is called once per frame
  void Update()
  {
    transform.rotation = Quaternion.Euler(90, -90, 0);
  }
}