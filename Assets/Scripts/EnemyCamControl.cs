using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCamControl : MonoBehaviour
{
  public Camera enemycam;
  private RawImage rawImage;
  public RenderTexture nothing;
  private RenderTexture activeTexture;

  // Start is called before the first frame update
  void Start()
  {
    rawImage = GetComponent<RawImage>();
    activeTexture = rawImage.texture as RenderTexture;
    rawImage.texture = nothing;
  }

  // Update is called once per frame
  void Update()
  {
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit hit;

    if (Input.GetButtonDown("Fire1"))
    {
      if (Physics.Raycast(ray, out hit, 100))
      {
        if (hit.collider.CompareTag("Character"))
        {
          rawImage.texture = activeTexture;

          enemycam = hit.transform.gameObject.GetComponentInChildren(typeof(Camera)) as Camera;
          RenderTexture renderTexture = enemycam.activeTexture;
          enemycam.targetTexture = null;
          enemycam.targetTexture = renderTexture;
        }
        else
        {
          rawImage.texture = nothing;
        }
      }
    }
  }
}