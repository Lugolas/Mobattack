using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Cam : MonoBehaviour
{
  public float speedReduction = 100;
  CinemachineVirtualCamera virtualCamera;
  // Start is called before the first frame update
  void Start()
  {
    virtualCamera = GetComponent<CinemachineVirtualCamera>();
  }

  // Update is called once per frame
  void Update()
  {
    float zAxis = -(Input.GetAxis("Horizontal") / speedReduction);
    float xAxis = Input.GetAxis("Vertical") / speedReduction;


    if (virtualCamera)
    {
      CinemachineComponentBase componentBase = virtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
      if (componentBase is CinemachineFramingTransposer)
      {
        float tempScreenX, tempScreenY;

        tempScreenX = (componentBase as CinemachineFramingTransposer).m_ScreenX;
        tempScreenY = (componentBase as CinemachineFramingTransposer).m_ScreenY;

        tempScreenX += zAxis;
        tempScreenY += xAxis;

        tempScreenX = Mathf.Clamp01(tempScreenX);
        tempScreenY = Mathf.Clamp01(tempScreenY);

        (componentBase as CinemachineFramingTransposer).m_ScreenX = tempScreenX;
        (componentBase as CinemachineFramingTransposer).m_ScreenY = tempScreenY;
      }
    }
  }
}
