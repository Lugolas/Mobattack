using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class AutoFocus : MonoBehaviour
{
  public PostProcessProfile profile;
  private DepthOfField dof;
  // Start is called before the first frame update
  void Start()
  {
    profile.TryGetSettings<DepthOfField>(out dof);
  }

  // Update is called once per frame
  void Update()
  {
    Ray rayMiddle = Camera.main.ScreenPointToRay(new Vector2(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2));
    RaycastHit hitMiddle;

    if (Physics.Raycast(rayMiddle, out hitMiddle, 5000))
    {
      if (dof != null)
      {
        dof.focusDistance.value = Vector3.Distance(hitMiddle.point, Camera.main.transform.position);
        dof.enabled.value = true;
      }
    }
  }
}
