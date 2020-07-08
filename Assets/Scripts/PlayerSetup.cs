using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour {
  private NetworkTransformChild characterNetworkTransform;
  [SyncVar]
  private GameObject character;
  private int characterNetworkId;
  private GameObject charactersManager;

  // Start is called before the first frame update
  void Start () {
    // charactersManager = GameObject.Find("CharactersManager");

    // if (hasAuthority)
    // {
    //   Init();
    // }
  }

  // Update is called once per frame
  void Update () {
    // if (hasAuthority)
    // {
    //   if (charactersManager)
    //   {
    //     BaseMoveAttacc[] moveScripts = charactersManager.GetComponentsInChildren<BaseMoveAttacc>();
    //     foreach (BaseMoveAttacc moveScript in moveScripts)
    //     {
    //       if (moveScript.gameObject.name != character.name && moveScript.enabled)
    //       {
    //         moveScript.enabled = false;
    //       }
    //     }
    //   }
    // }
    // else
    // {
    //   BaseMoveAttacc characterMoveScript = GetComponentInChildren<BaseMoveAttacc>();
    //   if (characterMoveScript && charactersManager)
    //   {
    //     characterMoveScript.gameObject.transform.SetParent(charactersManager.transform);
    //   }
    // }
  }

  void Init () {
    characterNetworkTransform = GetComponent<NetworkTransformChild> ();

    bool willInstantiate = true;

    CharacterManager[] managers = charactersManager.GetComponentsInChildren<CharacterManager> ();

    foreach (CharacterManager manager in managers) {
      if (manager.clientId == connectionToClient.connectionId) {
        willInstantiate = false;
        character = manager.gameObject;
      }
    }
    if (willInstantiate) {
      InstantiateCharacter ((connectionToClient.connectionId % 4) + 1);
    }

  }

  void InstantiateCharacter (int characterID) {
    GameObject characterPrefab = null;
    switch (characterID) {
      case 1:
        characterPrefab = Resources.Load<GameObject> ("Prefabs/Mage");
        break;
      case 2:
        characterPrefab = Resources.Load<GameObject> ("Prefabs/Grunt");
        break;
      case 3:
        characterPrefab = Resources.Load<GameObject> ("Prefabs/Archer");
        break;
      case 4:
        characterPrefab = Resources.Load<GameObject> ("Prefabs/Murderer");
        break;
    }
    if (characterPrefab) {
      Debug.Log ("WHAT");
      Debug.Log (characterPrefab);
      character = Instantiate (characterPrefab, transform);
      Debug.Log (character);
      Debug.Log (character.GetComponent<CharacterManager> ());
      Debug.Log (character.GetComponent<CharacterManager> ().clientId);
      character.GetComponent<CharacterManager> ().clientId = connectionToClient.connectionId;

      CmdSpawnCharacter (character);
      RpcSyncCharacter (character);
    }
  }

  [Command]
  void CmdSpawnCharacter (GameObject characterToSpawn) {
    NetworkServer.Spawn (character);
  }

  [ClientRpc]
  void RpcSyncCharacter (GameObject characterToSync) {
    character = characterToSync;
  }
}