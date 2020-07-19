using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerInputManager : NetworkBehaviour
{
  int characterIndex;
  // Index of the right character in the array of the children of the CharactersManager
  public GameObject targetPointer;
  [SyncVar]
  public GameObject character;
  [SyncVar]
  public GameObject target;

  [SyncVar]
  bool hasSetupCharacter = false;
  [SyncVar]
  bool hasCharacter = false;
  bool disable = false;

  bool moveClickDown = false;
  Vector3 moveClickPosition = Vector3.zero;
  List<Material> outlinedCharacterMaterials = new List<Material>();
  string outlinedCharacterName = null;

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
    Ray rayMouse = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit hitMouse;

    if (Physics.Raycast(rayMouse, out hitMouse, 2500))
    {
      if (hitMouse.collider.CompareTag("Character"))
      {
        Debug.Log("This is a character.");
        if (character && hitMouse.collider.name != character.name)
        {
          Debug.Log("That is not mine.");
          if (outlinedCharacterName == null)
          {
            Debug.Log("And I don't have a character outlined, so I get one.");
            Renderer[] targetRenderers;
            targetRenderers = hitMouse.collider.GetComponentsInChildren<Renderer>();
            foreach (Renderer targetRenderer in targetRenderers)
            {
              List<Material> targetMaterials = new List<Material>();
              targetRenderer.GetMaterials(targetMaterials);
              foreach (Material targetMaterial in targetMaterials)
              {
                targetMaterial.SetFloat("Vector1_Intensity", 1f);
                outlinedCharacterMaterials.Add(targetMaterial);
              }
              outlinedCharacterName = hitMouse.collider.name;
            }
          }
          else
          {
            Debug.Log("But I have a character outlined.");
            if (outlinedCharacterName != hitMouse.collider.name)
            {
              Debug.Log("This is a different one, so I change between them.");
              foreach (Material outlinedCharacterMaterial in outlinedCharacterMaterials)
              {
                outlinedCharacterMaterial.SetFloat("Vector1_Intensity", 0f);
              }
              Renderer[] targetRenderers;
              targetRenderers = hitMouse.collider.GetComponentsInChildren<Renderer>();
              foreach (Renderer targetRenderer in targetRenderers)
              {
                List<Material> targetMaterials = new List<Material>();
                targetRenderer.GetMaterials(targetMaterials);
                foreach (Material targetMaterial in targetMaterials)
                {
                  targetMaterial.SetFloat("Vector1_Intensity", 1f);
                  outlinedCharacterMaterials.Add(targetMaterial);
                }
                outlinedCharacterName = hitMouse.collider.name;
              }
            }
          }
        }
      }
      else
      {
        Debug.Log("This is NOT a character.");
        if (outlinedCharacterName != null)
        {
          Debug.Log("And I still have a character outlined, so I reset it.");
          foreach (var outlinedCharacterMaterial in outlinedCharacterMaterials)
          {
            outlinedCharacterMaterial.SetFloat("Vector1_Intensity", 0f);
          }
          outlinedCharacterMaterials.Clear();
          outlinedCharacterName = null;
        }
      }
    }



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
            moveClickDown = true;
            moveClickPosition = new Vector3(hit.point.x, hit.point.y + 0.1f, hit.point.z);
            CmdMoveTo(hit.point);
          }
        }
      }
    }
    else
    {
      if (moveClickDown)
      {
        moveClickDown = false;
        Instantiate(targetPointer, moveClickPosition, new Quaternion());
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
