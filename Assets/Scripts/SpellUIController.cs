using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
  float frameLightsRotation = 0f;
  bool spell1Active = false;
  bool spell2Active = false;
  bool spell3Active = false;
  bool spell1Available = true;
  bool spell2Available = true;
  bool spell3Available = true;

  void Start()
  {
    button1.onClick.AddListener(delegate { spellController.Spell1(); });
    button2.onClick.AddListener(delegate { spellController.Spell2(); });
    button3.onClick.AddListener(delegate { spellController.Spell3(); });
  }
  
  void Update() {
    buttonStateManager(
      ref spell1Active,
      ref spell1Available,
      frameLights1,
      image1,
      frame1,
      spellController.GetSpell1Active(),
      spellController.GetSpell1Available()
    );
    buttonStateManager(
      ref spell2Active,
      ref spell2Available,
      frameLights2,
      image2,
      frame2,
      spellController.GetSpell2Active(),
      spellController.GetSpell2Available()
    );
    buttonStateManager(
      ref spell3Active,
      ref spell3Available,
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
      frameLightsRotation -= 1f;
      frameLights1.rotation = Quaternion.Euler(0, 0, frameLightsRotation);
    }

    if (spell2Active) {
      frameLightsRotation -= 1f;
      frameLights2.rotation = Quaternion.Euler(0, 0, frameLightsRotation);
    }

    if (spell3Active) {
      frameLightsRotation -= 1f;
      frameLights3.rotation = Quaternion.Euler(0, 0, frameLightsRotation);
    }
  }

  public void test() {
    Debug.Log("yes + " + Time.time);
  }

  public void HoverIn(int buttonID) {
    HoverSetActive(buttonID, true);
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
}