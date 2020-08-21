using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScorePanelController : MonoBehaviour
{
  TMP_Text myTeam;
  TMP_Text otherTeam;
  GameObject charactersManager;
  GameController gameController;
  int playerTeam = -1;

  // Start is called before the first frame update
  void Start()
  {
    TMP_Text[] texts = GetComponentsInChildren<TMP_Text>();

    foreach (TMP_Text text in texts)
    {
      if (text.name == "MyTeam")
      {
        myTeam = text;
      }
      else if (text.name == "OtherTeam")
      {
        otherTeam = text;
      }
    }

    charactersManager = GameObject.Find("CharactersManager");

    gameController = Tools.getGameController();
  }

  // Update is called once per frame
  void Update()
  {
    if (playerTeam == -1)
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

    if (!gameController)
    {
      gameController = Tools.getGameController();
    }

    if (gameController && playerTeam != -1)
      if (playerTeam == 1)
      {
        myTeam.text = gameController.team1Score.ToString();
        otherTeam.text = gameController.team2Score.ToString();
      }
      else
      {
        myTeam.text = gameController.team2Score.ToString();
        otherTeam.text = gameController.team1Score.ToString();
      }
  }
}
