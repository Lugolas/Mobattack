using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomLevels : MonoBehaviour
{
  public List<float> zoomValues;
  public int currentZoomIndex;

  private void Start() {
    currentZoomIndex = zoomValues.Count / 2;
    Tools.ZoomRefresh();
  }
}
