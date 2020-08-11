using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenPointer : MonoBehaviour
{
  // List<Material> materials = new List<Material>();
  // private Transform parent;
  // private string precedentParentTag = "";
  // // Start is called before the first frame update
  void Start()
  {
    gameObject.GetComponent<MeshRenderer>().enabled = true;
    gameObject.layer = 12;
    // parent = GetComponentsInParent<Transform>()[1];
  }

  // // Update is called once per frame
  // void Update()
  // {
  //   string parentTag = parent.gameObject.tag;
  //   if (parentTag == "Player" && parentTag != precedentParentTag)
  //   {
  //     precedentParentTag = parentTag;
  //     GetComponent<Renderer>().GetMaterials(materials);
  //     foreach (Material material in materials)
  //     {
  //       material.SetColor("_EmissionColor", new Color(0, 1, 35f / 255f));
  //     }
  //   }
  // }
}