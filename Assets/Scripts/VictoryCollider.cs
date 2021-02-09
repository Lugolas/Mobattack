using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryCollider : MonoBehaviour
{
  public static bool hasTouched = false;
  GameController gameController;
  int team;
  void Start()
  {
    team = int.Parse(gameObject.name.Substring(gameObject.name.Length - 1));
    gameController = Tools.getGameController();
  }

  void Update()
  {
    if (!gameController)
    {
      gameController = Tools.getGameController();
    }
  }
  void OnTriggerEnter(Collider collider)
  {
    // CharacterManager character = collider.GetComponent<CharacterManager>();

    // if (character && gameController && hasTouched == false)
    // {
    //   if (team == 1 && gameController.team2ScoredLimit && character.team == 2)
    //   {
    //     hasTouched = true;
    //     gameController.winner = 2;
    //   }
    //   if (team == 2 && gameController.team1ScoredLimit && character.team == 1)
    //   {
    //     hasTouched = true;
    //     gameController.winner = 1;
    //   }
    // }
  }
}
