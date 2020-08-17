using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectionButtonsController : MonoBehaviour
{
  public int chosenTeam = -1;
  public int chosenCharacter = -1;
  public string chosenName = "";
  private TMP_InputField nameInput;
  public bool selectionComplete = false;
  GameObject charactersManager;

  public void endSelection()
  {
    bool isSelectionValid = true;
    chosenName = nameInput.text;

    CharacterManager[] characterManagers = charactersManager.GetComponentsInChildren<CharacterManager>();
    foreach (CharacterManager characterManager in characterManagers)
    {
      if (characterManager.name == chosenName)
      {
        isSelectionValid = false;
      }
    }

    if (chosenTeam <= 0 || chosenCharacter <= 0 || chosenName.Length < 1)
    {
      isSelectionValid = false;
    }

    if (isSelectionValid)
    {
      selectionComplete = true;
      visible(false);
    }
  }
  public void chooseTeam(int team)
  {
    chosenTeam = team;
  }
  public void chooseCharacter(int character)
  {
    chosenCharacter = character;
  }

  public void visible(bool state)
  {
    if (state)
    {
      chosenTeam = -1;
      chosenCharacter = -1;
      chosenName = "";
      selectionComplete = false;
    }

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

    TextMeshProUGUI[] tmpTexts;
    tmpTexts = GetComponentsInChildren<TextMeshProUGUI>();
    foreach (TextMeshProUGUI tmpText in tmpTexts)
    {
      tmpText.enabled = state;
    }

    TMP_InputField[] inputFields;
    inputFields = GetComponentsInChildren<TMP_InputField>();
    foreach (TMP_InputField inputField in inputFields)
    {
      inputField.enabled = state;
    }

    RectMask2D[] rectMasks;
    rectMasks = GetComponentsInChildren<RectMask2D>();
    foreach (RectMask2D rectMask in rectMasks)
    {
      rectMask.enabled = state;
    }

    Toggle[] toggles;
    toggles = GetComponentsInChildren<Toggle>();
    foreach (Toggle toggle in toggles)
    {
      toggle.enabled = state;
    }

    ToggleGroup[] toggleGroups;
    toggleGroups = GetComponentsInChildren<ToggleGroup>();
    foreach (ToggleGroup toggleGroup in toggleGroups)
    {
      toggleGroup.enabled = state;
    }
  }

  // Start is called before the first frame update
  void Start()
  {
    visible(false);
    charactersManager = GameObject.Find("CharactersManager");
    Transform nameInputTransform = transform.Find("CharacterName");
    if (nameInputTransform)
    {
      nameInput = nameInputTransform.GetComponent<TMP_InputField>();
    }
  }

  // Update is called once per frame
  void Update()
  {

  }
}