using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorpseEmissiveDim : MonoBehaviour
{
  List<Material> materials = new List<Material>();
  List<Vector3> colorsHSV = new List<Vector3>();
  List<float> values = new List<float>();
  CorpseController corpseController;

  void Start()
  {
    corpseController = GetComponentInParent<CorpseController>();
    GetComponent<Renderer>().GetMaterials(materials);
    foreach (Material material in materials)
    {
      float h, s, v;
      Color.RGBToHSV(material.GetColor("EmissionColor"), out h, out s, out v);
      colorsHSV.Add(new Vector3(h, s, v));
      values.Add(v);
    }
  }

  void Update()
  {
    if (corpseController)
    {
      for (int i = 0; i < materials.Count; i++)
      {
        materials[i].SetColor("EmissionColor", Color.HSVToRGB(colorsHSV[i].x, colorsHSV[i].y, corpseController.lifeRatio * values[i]));
      }
    }
  }
}