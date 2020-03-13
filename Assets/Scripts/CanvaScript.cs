using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CanvaScript : MonoBehaviour
{
  public GameObject MinimapFeed;
  public Camera MinimapCam;
  public GameObject MinimapCamRect;
  RectTransform miniRectTransform;
  float miniYPos;
  float miniXPos;
  float miniYCamScreenPos;
  float miniXCamScreenPos;
  GraphicRaycaster m_Raycaster;
  PointerEventData m_PointerEventData;
  EventSystem m_EventSystem;

  // Start is called before the first frame update
  void Start()
  {
    miniRectTransform = MinimapFeed.GetComponent<RectTransform>();
    //Fetch the Raycaster from the GameObject (the Canvas)
    m_Raycaster = GetComponent<GraphicRaycaster>();
    //Fetch the Event System from the Scene
    m_EventSystem = GetComponent<EventSystem>();
  }

  // Update is called once per frame
  void Update()
  {
    //Carré qui représente la caméra en fonction des coins de l'écran
    // J'ai besoin de trois positions donc trois raycasts
    Ray rayBottomLeft = Camera.main.ScreenPointToRay(new Vector2(0, 0));
    RaycastHit hitBottomLeft;
    Ray rayMiddleBottom = Camera.main.ScreenPointToRay(new Vector2(Camera.main.pixelWidth / 2, 0));
    RaycastHit hitMiddleBottom;
    Ray rayBottomRight = Camera.main.ScreenPointToRay(new Vector2(Camera.main.pixelWidth, 0));
    RaycastHit hitBottomRight;
    Ray rayMiddleRight = Camera.main.ScreenPointToRay(new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight / 2));
    RaycastHit hitMiddleRight;
    Ray rayTopRight = Camera.main.ScreenPointToRay(new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight));
    RaycastHit hitTopRight;
    Ray rayMiddleTop = Camera.main.ScreenPointToRay(new Vector2(Camera.main.pixelWidth / 2, Camera.main.pixelHeight));
    RaycastHit hitMiddleTop;
    Ray rayTopLeft = Camera.main.ScreenPointToRay(new Vector2(0, Camera.main.pixelHeight));
    RaycastHit hitTopLeft;
    Ray rayMiddleLeft = Camera.main.ScreenPointToRay(new Vector2(0, Camera.main.pixelHeight / 2));
    RaycastHit hitMiddleLeft;
    Ray rayMiddle = Camera.main.ScreenPointToRay(new Vector2(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2));
    RaycastHit hitMiddle;

    if (Physics.Raycast(rayBottomLeft, out hitBottomLeft, 5000) &&
    Physics.Raycast(rayMiddleBottom, out hitMiddleBottom, 5000) &&
    Physics.Raycast(rayBottomRight, out hitBottomRight, 5000) &&
    Physics.Raycast(rayMiddleRight, out hitMiddleRight, 5000) &&
    Physics.Raycast(rayTopRight, out hitTopRight, 5000) &&
    Physics.Raycast(rayMiddleTop, out hitMiddleTop, 5000) &&
    Physics.Raycast(rayTopLeft, out hitTopLeft, 5000) &&
    Physics.Raycast(rayMiddleLeft, out hitMiddleLeft, 5000) &&
    Physics.Raycast(rayMiddle, out hitMiddle, 5000))
    {
      float z = (hitBottomLeft.point.z +
      hitMiddleBottom.point.z +
      hitBottomRight.point.z +
      hitMiddleRight.point.z +
      hitTopRight.point.z +
      hitMiddleTop.point.z +
      hitTopLeft.point.z +
      hitMiddleLeft.point.z +
      hitMiddle.point.z) / 9;

      float x = (hitBottomLeft.point.x +
      hitMiddleBottom.point.x +
      hitBottomRight.point.x +
      hitMiddleRight.point.x +
      hitTopRight.point.x +
      hitMiddleTop.point.x +
      hitTopLeft.point.x +
      hitMiddleLeft.point.x +
      hitMiddle.point.x) / 9;

      // Moyenne des distances horizontales
      float width = (hitBottomRight.point.z - hitBottomLeft.point.z +
        hitMiddleRight.point.z - hitMiddleLeft.point.z +
        hitTopRight.point.z - hitTopLeft.point.z) / 3;
      // Moyenne des distances Verticales
      float height = (hitBottomLeft.point.x - hitTopLeft.point.x +
        hitMiddleBottom.point.x - hitMiddleTop.point.x +
        hitBottomRight.point.x - hitTopRight.point.x) / 3;

      // Debug.Log("AU DÉBUT: z " + z + " -- x " + x + " -- width " + width + " -- height " + height + " -- (z - width / 2) " + (z - width / 2) + " -- (z + width / 2) " + (z + width / 2) + " -- (x - height / 2) " + (x - height / 2) + " -- (x + height / 2) " + (x + height / 2));

      // float limitLeft = -170.0f;
      // float limitRight = 102.0f;
      // float limitTop = -105.0f;
      // float limitBottom = 105.0f;

      float endLeft = (z - width / 2);
      float endRight = (z + width / 2);
      float endTop = (x - height / 2);
      float endBottom = (x + height / 2);

      // if (z + width / 2 > 38.93f)
      // if (endLeft < limitLeft)
      // {
      //   // else
      //   // {
      //   if (z < limitLeft + 5)
      //   {
      //     z = limitLeft + 5;
      //   }
      //   width = width - (limitLeft - (z - width / 2)) * 2f;
      //   // Debug.Log("OUI");
      //   // z = z + (limitLeft - endLeft);
      //   // }
      // }
      // z = 38.93f;

      // if (z + width < 43.93f)
      //   if (z + width > -66.25f)
      //     width = width;
      //   else
      //     width = 5.0f;
      // else
      //   width = 43.93f - width - 5.0f;

      // if (x < 45.0f)
      //   if (x > -40.0f)
      //     x = x;
      //   else
      //     x = -40.0f;
      // else
      //   x = 45.0f;

      // if (x - height < 40.0f)
      //   if (x - height > -45.0f)
      //     height = height;
      //   else
      //     height = 90.0f;
      // else
      //   height = 5.0f;

      // Debug.Log("JUSTE AVANT D'ASSIGNER: z " + z + " -- x " + x + " -- width " + width + " -- height " + height + " -- (z - width / 2) " + (z - width / 2) + " -- (z + width / 2) " + (z + width / 2) + " -- (x - height / 2) " + (x - height / 2) + " -- (x + height / 2) " + (x + height / 2));

      MinimapCamRect.transform.position = new Vector3(x, MinimapCamRect.transform.position.y, z);
      SpriteRenderer MinimapCamRectSpriteRenderer = MinimapCamRect.GetComponent<SpriteRenderer>();
      MinimapCamRectSpriteRenderer.size = new Vector2(width / 200, height / 200);
    }

    //Check if the left Mouse button is clicked
    if (Input.GetKey(KeyCode.Mouse0))
    {
      //Set up the new Pointer Event
      m_PointerEventData = new PointerEventData(m_EventSystem);
      //Set the Pointer Event Position to that of the mouse position
      m_PointerEventData.position = Input.mousePosition;

      //Create a list of Raycast Results
      List<RaycastResult> results = new List<RaycastResult>();

      //Raycast using the Graphics Raycaster and mouse click position
      m_Raycaster.Raycast(m_PointerEventData, results);

      //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
      foreach (RaycastResult result in results)
      {
        if (result.gameObject == MinimapFeed)
        {
          miniXPos = m_PointerEventData.position.x - (Screen.width + (miniRectTransform.anchoredPosition.x - (miniRectTransform.sizeDelta.x / 2)));
          miniXCamScreenPos = miniXPos * MinimapCam.pixelWidth / miniRectTransform.sizeDelta.x;
          miniYPos = m_PointerEventData.position.y - (miniRectTransform.anchoredPosition.y - (miniRectTransform.sizeDelta.y / 2));
          miniYCamScreenPos = miniYPos * MinimapCam.pixelHeight / miniRectTransform.sizeDelta.y;

          // Donc là je fais le raycast depuis la seconde caméra avec ces coordonnées et je mets la caméra principale dessus 
          Ray ray = MinimapCam.ScreenPointToRay(new Vector2(miniXCamScreenPos, miniYCamScreenPos));
          RaycastHit hit;

          if (Physics.Raycast(ray, out hit, 2500))
          {
            if (hit.collider.gameObject.CompareTag("Environment"))
            {
              Camera.main.transform.position = new Vector3(hit.point.x, Camera.main.transform.position.y, hit.point.z);
            }
          }
        }
      }
    }
  }
}