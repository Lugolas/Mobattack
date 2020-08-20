using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class GameController : NetworkBehaviour
{
  [SyncVar]
  public int team1Score = 0;
  [SyncVar]
  public int team2Score = 0;
  [SyncVar]
  int scoreLimit = 1;
  [SyncVar]
  public int scorer = -1;
  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    if (scorer == -1)
    {
      if (team1Score >= scoreLimit)
      {
        scorer = 1;
      }
      else if (team2Score >= scoreLimit)
      {
        scorer = 2;
      }
    }
  }
}
