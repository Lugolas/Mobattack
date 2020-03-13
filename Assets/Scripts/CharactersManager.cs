using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactersManager : MonoBehaviour
{
  public int currentCharacter = 0;
  public int previousCharacter = -1;
  private List<GameObject> characters = new List<GameObject>();

  // Start is called before the first frame update
  void Start()
  {
    GameObject grunt = transform.Find("Grunt").gameObject;
    GameObject mage = transform.Find("Mage").gameObject;
    GameObject archer = transform.Find("Archer").gameObject;
    GameObject murderer = transform.Find("Murderer").gameObject;
    characters.Add(grunt);
    characters.Add(mage);
    characters.Add(archer);
    characters.Add(murderer);
  }

  // Update is called once per frame
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.Keypad0))
    {
      currentCharacter = 0;
    }
    if (Input.GetKeyDown(KeyCode.Keypad1))
    {
      currentCharacter = 1;
    }
    if (Input.GetKeyDown(KeyCode.Keypad2))
    {
      currentCharacter = 2;
    }
    if (Input.GetKeyDown(KeyCode.Keypad3))
    {
      currentCharacter = 3;
    }


    if (currentCharacter != previousCharacter)
    {
      previousCharacter = currentCharacter;
      foreach (GameObject character in characters)
      {
        character.GetComponent<BaseMoveAttacc>().disable();
      }
      characters[currentCharacter].GetComponent<BaseMoveAttacc>().enable();
    }
  }
}