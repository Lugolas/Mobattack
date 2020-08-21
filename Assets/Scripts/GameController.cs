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
  int scoreLimit = 5;
  [SyncVar]
  public bool team1ScoredLimit = false;
  [SyncVar]
  public bool team2ScoredLimit = false;
  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    if (team1Score >= scoreLimit)
    {
      team1ScoredLimit = true;
    }
    if (team2Score >= scoreLimit)
    {
      team2ScoredLimit = true;
    }
  }
}
