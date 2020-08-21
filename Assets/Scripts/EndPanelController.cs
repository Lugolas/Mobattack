using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPanelController : MonoBehaviour
{
  public GameObject winImage;
  public GameObject losImage;
  GameController gameController;
  int playerTeam = -1;
  // Start is called before the first frame update
  void Start()
  {
    if (winImage)
    {
      winImage.SetActive(false);
    }
    if (losImage)
    {
      losImage.SetActive(false);
    }
    gameController = Tools.getGameController();
  }

  // Update is called once per frame
  void Update()
  {
    if (!gameController)
    {
      gameController = Tools.getGameController();
    }
    if (playerTeam == -1)
    {
      GameObject charactersManager = GameObject.Find("CharactersManager");
      if (charactersManager)
      {
        CharacterManager[] characterManagers = charactersManager.GetComponentsInChildren<CharacterManager>();
        foreach (CharacterManager characterManager in characterManagers)
        {
          if (characterManager.isPlayerCharacter)
          {
            playerTeam = characterManager.team;
          }
        }
      }
    }
    if (gameController && playerTeam != -1 && gameController.winner != -1)
    {
      if (gameController.winner == playerTeam)
      {
        winImage.SetActive(true);
      }
      else
      {
        losImage.SetActive(true);
      }
    }
  }
}
