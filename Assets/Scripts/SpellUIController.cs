using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpellUIController : MonoBehaviour {
  public SpellController spellController;
  public GameObject hover1;
  public GameObject hover2;
  public GameObject hover3;
  public Button button1;
  public Button button2;
  public Button button3;
  public Image image1;
  public Image image2;
  public Image image3;
  public Image frame1;
  public Image frame2;
  public Image frame3;
  public RectTransform frameLights1;
  public RectTransform frameLights2;
  public RectTransform frameLights3;
  float frameLights1Rotation = 0f;
  float frameLights2Rotation = 0f;
  float frameLights3Rotation = 0f;
  bool spell1Active = false;
  bool spell2Active = false;
  bool spell3Active = false;
  bool spell1Available = true;
  bool spell2Available = true;
  bool spell3Available = true;
  List<float> imagesAlphaValues = new List<float>();
  Image[] images;
  Button[] buttons;
  TMP_Text[] keyTexts;

  bool hideOnSpell3 = true;

  public void SetHideOnSpell3(bool value) {
    hideOnSpell3 = value;
  }

  void Start()
  {
    images = GetComponentsInChildren<Image>(true);
    buttons = GetComponentsInChildren<Button>(true);
    keyTexts = GetComponentsInChildren<TMP_Text>(true);
    foreach (Image image in images)
    {
      imagesAlphaValues.Add(image.color.a);
    }
    button1.onClick.AddListener(delegate { spellController.Spell1(); });
    button2.onClick.AddListener(delegate { spellController.Spell2(); });
    button3.onClick.AddListener(delegate { spellController.Spell3(); });
  }
  
  void Update() {
    buttonStateManager(
      ref spell1Active,
      ref spell1Available,
      ref frameLights1Rotation,
      frameLights1,
      image1,
      frame1,
      spellController.GetSpell1Active(),
      spellController.GetSpell1Available()
    );
    buttonStateManager(
      ref spell2Active,
      ref spell2Available,
      ref frameLights2Rotation,
      frameLights2,
      image2,
      frame2,
      spellController.GetSpell2Active(),
      spellController.GetSpell2Available()
    );
    buttonStateManager(
      ref spell3Active,
      ref spell3Available,
      ref frameLights3Rotation,
      frameLights3,
      image3,
      frame3,
      spellController.GetSpell3Active(),
      spellController.GetSpell3Available()
    );
  }

  void FixedUpdate()
  {
    if (spell1Active) {
      frameLights1Rotation -= 1f;
      frameLights1.rotation = Quaternion.Euler(0, 0, frameLights1Rotation);
    }

    if (spell2Active) {
      frameLights2Rotation -= 1f;
      frameLights2.rotation = Quaternion.Euler(0, 0, frameLights2Rotation);
    }

    if (spell3Active) {
      frameLights3Rotation -= 1f;
      frameLights3.rotation = Quaternion.Euler(0, 0, frameLights3Rotation);
    }
  }

  public void test() {
    Debug.Log("yes + " + Time.time);
  }

  public void HoverIn(int buttonID) {
    if (!(spell1Active || spell2Active || (spell3Active && hideOnSpell3))) {
      HoverSetActive(buttonID, true);
    }
  }

  public void HoverOut(int buttonID) {
    HoverSetActive(buttonID, false);
  }

  void HoverSetActive(int buttonID, bool value) {
    switch (buttonID)
    {
      case 1:
        if (spell1Available)
          hover1.SetActive(value);
        break;
      case 2:
        if (spell2Available)
          hover2.SetActive(value);
        break;
      case 3:
        if (spell3Available)
          hover3.SetActive(value);
        break;
      default:
        break;
    }
  }

  void buttonStateManager(
    ref bool spellActive, 
    ref bool spellAvailable, 
    ref float frameLightsRotation,
    RectTransform frameLights, 
    Image image, 
    Image frame, 
    bool spellControllerSpellActive, 
    bool spellControllerSpellAvailable
  ) 
  {
    if (spellController) 
    {
      if (spellActive == false) 
      {
        if (spellControllerSpellActive) 
        {
          spellActive = true;
          frameLights.gameObject.SetActive(true);
        } else 
        {
          if (spellAvailable == false) 
          {
            if (spellControllerSpellAvailable) 
            {
              spellAvailable = true;
              image.color = Color.white;
              frame.color = Color.white;
            }
          } else 
          {
            if (!spellControllerSpellAvailable)
            {
              spellAvailable = false;
              image.color = Tools.GetDisabledColor();
              frame.color = Tools.GetDisabledColor();
            }
          }
        }
      } else 
      {
        if (!spellControllerSpellActive) 
        {
          spellActive = false;
          frameLights.rotation = Quaternion.Euler(0, 0, 0);
          frameLightsRotation = 0;
          frameLights.gameObject.SetActive(false);
        }
      }
    }
  }

  public void hideIfActive() {
    if (spell1Active || spell2Active || (spell3Active && hideOnSpell3)) {
      foreach (Image image in images)
      {
        Color color = image.color;
        color.a = color.a * 0.25f;
        image.color = color;
      }
      foreach (Button button in buttons)
      {
        button.interactable = false;
      }
      foreach (TMP_Text keyText in keyTexts)
      {
        keyText.alpha = 0.25f;
      }
    }
  }
  public void showIfHidden() {
    for (var i = 0; i < images.Length; i++)
    {
      Color color = images[i].color;
      color.a = imagesAlphaValues[i];
      images[i].color = color;
    }
    foreach (Button button in buttons)
    {
      button.interactable = true;
    }
    foreach (TMP_Text keyText in keyTexts)
    {
      keyText.alpha = 1f;
    }
  }
}