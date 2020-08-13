using System.Collections;
using System.Collections.Generic;
using Cinemachine;
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
  int currentZoomIndex;
  float[] zoomLevels = { 5, 10, 15, 20, 25 };
  public CinemachineVirtualCamera virtualCamera;
  private bool isCameraOnCharacter = false;
  public Transform waitingForRespawnPoint;
  GameObject teamPanel;
  SelectionButtonsController teamButtons;

  // click -> send to server position and index
  // Start is called before the first frame update
  void Start()
  {
    if (!isLocalPlayer)
    {
      disable = true;
    }
    else
    {
      teamPanel = GameObject.Find("TeamPanel");

      if (!waitingForRespawnPoint)
      {
        GameObject spawnPoint = GameObject.Find("waitingForRespawnPoint");
        if (spawnPoint)
        {
          waitingForRespawnPoint = spawnPoint.transform;
        }
      }

      if (teamPanel)
      {
        teamButtons = teamPanel.GetComponent<SelectionButtonsController>();
        if (teamButtons)
        {
          teamButtons.visible(true);
        }
      }
    }
    currentZoomIndex = zoomLevels.Length - 2;
  }

  [Command]
  void CmdSetupCharacter()
  {
    GameObject characterPrefab = null;
    switch (teamButtons.chosenCharacter)
    {
      default:
      case 1:
        characterPrefab = Resources.Load<GameObject>("Prefabs/Mage");
        break;
      case 2:
        characterPrefab = Resources.Load<GameObject>("Prefabs/Grunt");
        break;
      case 3:
        characterPrefab = Resources.Load<GameObject>("Prefabs/Archer");
        break;
      case 4:
        characterPrefab = Resources.Load<GameObject>("Prefabs/Murderer");
        break;
    }
    if (characterPrefab)
    {
      GameObject currentCharacter = Instantiate(characterPrefab, waitingForRespawnPoint.position, waitingForRespawnPoint.rotation);
      CharacterManager characterManager = currentCharacter.GetComponent<CharacterManager>();
      HealthDamage healthDamage = currentCharacter.GetComponent<HealthDamage>();

      characterManager.clientId = connectionToClient.connectionId;
      characterManager.player = gameObject;
      characterManager.team = teamButtons.chosenTeam;
      if (teamButtons.chosenName != "")
      {
        healthDamage.playerName = teamButtons.chosenName;
      }

      NetworkServer.Spawn(currentCharacter);
    }
    RpcSetupCharacter(connectionToClient.connectionId);
  }

  [ClientRpc]
  void RpcSetupCharacter(int clientId)
  {
    GameObject charactersManager = GameObject.Find("CharactersManager");
    if (charactersManager)
    {
      CharacterManager[] characterManagers = charactersManager.GetComponentsInChildren<CharacterManager>();
      foreach (CharacterManager characterManager in characterManagers)
      {
        if (characterManager.clientId == clientId)
        {
          character = characterManager.gameObject;
          if (isLocalPlayer)
          {
            character.tag = "PlayerCharacter";
            virtualCamera = GameObject.Find("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
            virtualCamera.Follow = character.transform;
            isCameraOnCharacter = true;
          }
          hasCharacter = true;
          hasSetupCharacter = true;
          break;
        }
      }
    }

    // if (teamPanel)
    // {
    //   if (teamButtons && teamButtons.selectionComplete)
    //   {
    //     // BaseMoveAttacc[] characs = GameObject.Find("CharactersManager").GetComponentsInChildren<BaseMoveAttacc>();
    //     // foreach (BaseMoveAttacc charac in characs)
    //     // {
    //     //   CharacterManager characterManager = charac.gameObject.GetComponent<CharacterManager>();
    //     //   if (!hasCharacter && characterManager.clientId == -1 && characterManager.team == teamButtons.chosenTeam)
    //     //   {
    //     //     character = charac.gameObject;
    //     //     characterManager.clientId = clientId;
    //     //     characterManager.player = gameObject;
    //     //     if (isLocalPlayer)
    //     //     {
    //     //       charac.gameObject.tag = "PlayerCharacter";
    //     //       virtualCamera = GameObject.Find("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
    //     //       virtualCamera.Follow = character.transform;
    //     //       isCameraOnCharacter = true;
    //     //     }
    //     //     hasCharacter = true;
    //     //   }
    //     // }
    //     // hasSetupCharacter = true;
    //   }
    // }
  }


  void OnDisable()
  {
    if (character)
    {
      character.GetComponent<CharacterManager>().clientId = -1;
      character.GetComponent<CharacterManager>().player = null;
      character.GetComponent<CharacterManager>().isPlayerCharacter = false;
    }
  }

  // Update is called once per frame
  void Update()
  {
    if (disable)
      return;
    if (!hasSetupCharacter && teamPanel && teamButtons && teamButtons.selectionComplete)
    {
      CmdSetupCharacter();
    }
    // if (!isLocalPlayer)
    // {
    Ray rayMouse = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit hitMouse;

    if (Physics.Raycast(rayMouse, out hitMouse, 2500))
    {
      if (hitMouse.collider.CompareTag("Character") || hitMouse.collider.CompareTag("PlayerCharacter") || hitMouse.collider.CompareTag("TeamCharacter") || hitMouse.collider.CompareTag("EnemyCharacter"))
      {
        if (character)
        // if (character && hitMouse.collider.name != character.name)
        {
          if (outlinedCharacterName == null)
          {
            Renderer[] targetRenderers;
            targetRenderers = hitMouse.collider.GetComponentsInChildren<Renderer>();
            foreach (Renderer targetRenderer in targetRenderers)
            {
              List<Material> targetMaterials = new List<Material>();
              targetRenderer.GetMaterials(targetMaterials);
              foreach (Material targetMaterial in targetMaterials)
              {
                targetMaterial.SetFloat("OutlineIntensity", 1f);
                outlinedCharacterMaterials.Add(targetMaterial);
              }
              outlinedCharacterName = hitMouse.collider.name;
            }
          }
          else
          {
            if (outlinedCharacterName != hitMouse.collider.name)
            {
              foreach (Material outlinedCharacterMaterial in outlinedCharacterMaterials)
              {
                outlinedCharacterMaterial.SetFloat("OutlineIntensity", 0f);
              }
              Renderer[] targetRenderers;
              targetRenderers = hitMouse.collider.GetComponentsInChildren<Renderer>();
              foreach (Renderer targetRenderer in targetRenderers)
              {
                List<Material> targetMaterials = new List<Material>();
                targetRenderer.GetMaterials(targetMaterials);
                foreach (Material targetMaterial in targetMaterials)
                {
                  targetMaterial.SetFloat("OutlineIntensity", 1f);
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
        if (outlinedCharacterName != null)
        {
          foreach (var outlinedCharacterMaterial in outlinedCharacterMaterials)
          {
            outlinedCharacterMaterial.SetFloat("OutlineIntensity", 0f);
          }
          outlinedCharacterMaterials.Clear();
          outlinedCharacterName = null;
        }
      }
    }

    if (Input.GetAxis("Mouse ScrollWheel") != 0)
    {
      if (Input.GetAxis("Mouse ScrollWheel") > 0)
      {
        if (currentZoomIndex > 0)
        {
          currentZoomIndex--;
        }
      }
      if (Input.GetAxis("Mouse ScrollWheel") < 0)
      {
        if (currentZoomIndex < zoomLevels.Length - 1)
        {
          currentZoomIndex++;
        }
      }

      if (virtualCamera)
      {
        CinemachineComponentBase componentBase = virtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
        if (componentBase is CinemachineFramingTransposer)
        {
          (componentBase as CinemachineFramingTransposer).m_CameraDistance = zoomLevels[currentZoomIndex]; // your value
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
          if ((hit.collider.CompareTag("Character") || hit.collider.CompareTag("EnemyCharacter")) && hit.collider.name != character.name)
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
  }

  public void cameraOnCorpse()
  {
    if (character)
    {
      HealthDamage healthDamage = character.GetComponent<HealthDamage>();
      if (healthDamage)
      {
        virtualCamera.Follow = healthDamage.playerCorpse.transform;
        isCameraOnCharacter = false;
      }
    }
  }

  public void cameraOnCharacter()
  {
    if (character)
    {
      virtualCamera.Follow = character.transform;
      isCameraOnCharacter = true;
    }
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