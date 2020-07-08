using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerInputManager : NetworkBehaviour
{
  int characterIndex;
  // Index of the right character in the array of the children of the CharactersManager
  [SyncVar]
  public GameObject character;
  [SyncVar]
  public GameObject target;

  [SyncVar]
  bool hasSetupCharacter = false;
  [SyncVar]
  bool hasCharacter = false;
  bool disable = false;

  // click -> send to server position and index
  // Start is called before the first frame update
  void Start()
  {
    if (!isLocalPlayer)
    {
      disable = true;
    }
  }

  [Command]
  void CmdSetupCharacter()
  {
    RpcSetupCharacter(connectionToClient.connectionId);
  }
  [ClientRpc]
  void RpcSetupCharacter(int clientId)
  {
    BaseMoveAttacc[] characs = GameObject.Find("CharactersManager").GetComponentsInChildren<BaseMoveAttacc>();
    foreach (BaseMoveAttacc charac in characs)
    {
      if (!hasCharacter && charac.gameObject.GetComponent<CharacterManager>().clientId == -1)
      {
        character = charac.gameObject;
        charac.gameObject.GetComponent<CharacterManager>().clientId = clientId;
        hasCharacter = true;
      }
    }
    hasSetupCharacter = true;
  }

  // Update is called once per frame
  void Update()
  {
    if (disable)
      return;
    if (!hasSetupCharacter)
    {
      CmdSetupCharacter();
    }
    // if (!isLocalPlayer)
    // {
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit hit;

    if (Input.GetButton("Fire2"))
    // if (Input.GetButton("Fire2") && !healthDamage.isDead)
    {
      // rightClicked = true;
      if (Physics.Raycast(ray, out hit, 2500))
      {
        if (character)
        {
          if (hit.collider.CompareTag("Character") && hit.collider.name != character.name)
          {
            target = hit.transform.gameObject;
            // targetedEnemy = hit.transform;
            // enemyClicked = true;
            CmdAttack(target.name);
          }
          else
          {
            CmdMoveTo(hit.point);
          }
        }
      }
    }
    // }
  }

  [Command]
  void CmdAttack(string name)
  {
    RpcAttack(name);
  }

  [ClientRpc]
  void RpcAttack(string name)
  {
    if (character)
    {
      BaseMoveAttacc moveScript = character.GetComponent<BaseMoveAttacc>();
      HealthDamage healthScript = character.GetComponent<HealthDamage>();
      if (moveScript && healthScript)
      {
        if (!healthScript.isDead)
        {
          // if (!target)
          // {
          target = GameObject.Find("CharactersManager").transform.Find(name).gameObject;
          // }
          moveScript.hasNavigationTarget = true;
          moveScript.navigationTargetMovable = target.transform;
          moveScript.isNavigationTargetMovable = true;
        }
      }
    }
  }
  [Command]
  void CmdMoveTo(Vector3 point)
  {
    RpcMoveTo(point);
  }

  [ClientRpc]
  void RpcMoveTo(Vector3 point)
  {
    if (character)
    {
      BaseMoveAttacc moveScript = character.GetComponent<BaseMoveAttacc>();
      HealthDamage healthScript = character.GetComponent<HealthDamage>();
      if (moveScript && healthScript)
      {
        if (!healthScript.isDead)
        {
          moveScript.hasNavigationTarget = true;
          moveScript.navigationTarget = point;
          moveScript.isNavigationTargetMovable = false;
        }
      }
    }
  }
}
