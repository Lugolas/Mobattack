using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamButtonsController : MonoBehaviour
{
  public int chosenTeam = -1;

  public void chooseTeam(int team)
  {
    chosenTeam = team;
  }

  public void visible(bool state)
  {
    chosenTeam = -1;

    Button[] buttons;
    buttons = GetComponentsInChildren<Button>();
    foreach (Button button in buttons)
    {
      button.enabled = state;
    }

    Image[] images;
    images = GetComponentsInChildren<Image>();
    foreach (Image image in images)
    {
      image.enabled = state;
    }

    Text[] texts;
    texts = GetComponentsInChildren<Text>();
    foreach (Text text in texts)
    {
      text.enabled = state;
    }
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
