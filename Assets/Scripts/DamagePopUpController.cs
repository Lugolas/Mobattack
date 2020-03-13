using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamagePopUpController : MonoBehaviour
{
  private static DamagePopUp damagePopUp;
  private static GameObject canvas;
  public static float r = 255;
  public static float g = 85;
  public static float b = 50;
  public static void Initialize()
  {
    canvas = GameObject.Find("Canvas");
    if (!damagePopUp)
      damagePopUp = Resources.Load<DamagePopUp>("Prefabs/PopUpTextParent");
  }
  public static void CreateDamagePopUp(string text, Transform location, string color = "red")
  {
    DamagePopUp instance = Instantiate(damagePopUp);
    Vector3 positionHigher = new Vector3(location.position.x + Random.Range(-0.25f, 0.25f), location.position.y + 1 + Random.Range(-0.25f, 0.25f), location.position.z);
    Vector2 screenPosition = Camera.main.WorldToScreenPoint(positionHigher);
    instance.transform.SetParent(canvas.transform, false);
    instance.transform.position = screenPosition;
    switch (color)
    {
      case "red":
        instance.GetComponentInChildren<Text>().color = new Color(r / 255, g / 255, b / 255);
        break;
      case "green":
        instance.GetComponentInChildren<Text>().color = new Color(b / 255, r / 255, g / 255);
        break;
      case "blue":
        instance.GetComponentInChildren<Text>().color = new Color(g / 255, b / 255, r / 255);
        break;
      default:
        break;
    }
    instance.SetText(text);
  }

  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }
}
