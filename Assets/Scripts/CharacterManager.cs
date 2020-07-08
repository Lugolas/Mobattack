using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CharacterManager : NetworkBehaviour
{
  [SyncVar]
  public int clientId = -1;

  void Start()
  {
    GameObject charactersManager = GameObject.Find("CharactersManager");
    if (charactersManager)
    {
      transform.SetParent(charactersManager.transform);
    }
  }
}
