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
  float hasTriedToSetupCharacter;
  GameObject charactersManager;
  int GROUND_LAYER = 8;
  int ENVIRONMENT_NO_RAY_LAYER = 17;
  int clientIdLast;
  int layerMask;

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
      layerMask = 1 << ENVIRONMENT_NO_RAY_LAYER;
      layerMask = ~layerMask;
      charactersManager = GameObject.Find("CharactersManager");
      teamPanel = GameObject.Find("TeamPanel");
      hasTriedToSetupCharacter = Time.time;

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
  void CmdFinishSetupCharacter(string chosenName)
  {
    RpcSetupCharacter(chosenName);
  }

  [Command]
  void CmdSetupCharacter(int chosenCharacter, int chosenTeam, string chosenName)
  {
    GameObject characterPrefab = null;

    switch (chosenCharacter)
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
      GameObject spawnPoint = GameObject.Find("waitingForRespawnPoint");
      GameObject character = Instantiate(characterPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
      CharacterManager characterManager = character.GetComponent<CharacterManager>();
      HealthDamage healthDamage = character.GetComponent<HealthDamage>();

      characterManager.clientId = connectionToClient.connectionId;
      characterManager.player = gameObject;
      characterManager.team = chosenTeam;
      healthDamage.playerName = chosenName;

      NetworkServer.Spawn(character);
      RpcSetupCharacter(chosenName);
    }
  }

  [ClientRpc]
  void RpcSetupCharacter(string characterName)
  {
    charactersManager = GameObject.Find("CharactersManager");
    if (charactersManager)
    {
      HealthDamage[] healthDamages = charactersManager.GetComponentsInChildren<HealthDamage>();
      foreach (HealthDamage healthDamage in healthDamages)
      {
        if (healthDamage.playerName == characterName)
        {
          character = healthDamage.gameObject;
          character.GetComponent<CharacterManager>().player = gameObject;

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
    if (teamButtons)
    {
      teamButtons.visible(false);
    }
  }

  // Update is called once per frame
  void Update()
  {
    if (disable)
      return;
    if (!hasSetupCharacter && teamPanel && teamButtons && teamButtons.selectionComplete && Time.time >= (hasTriedToSetupCharacter + 0.5))
    {
      CharacterManager[] characterManagers = charactersManager.GetComponentsInChildren<CharacterManager>();
      bool alreadySetup = false;
      foreach (CharacterManager characterManager in characterManagers)
      {
        if (characterManager.gameObject.GetComponent<HealthDamage>().playerName == teamButtons.chosenName)
        {
          alreadySetup = true;
          CmdFinishSetupCharacter(teamButtons.chosenName);
        }
      }
      if (!alreadySetup)
      {
        hasTriedToSetupCharacter = Time.time;
        CmdSetupCharacter(teamButtons.chosenCharacter, teamButtons.chosenTeam, teamButtons.chosenName);
      }
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
    // RaycastHit hit;

    if (Input.GetButton("Fire2"))
    // if (Input.GetButton("Fire2") && !healthDamage.isDead)
    {
      RaycastHit[] hits = Physics.RaycastAll(ray, 2500, layerMask);
      if (hits.Length > 0)
      {
        System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));
        foreach (RaycastHit hit in hits)
        {
          if (character && hit.collider.gameObject != character)
          {
            GameObject characterHit = Tools.FindObjectOrParentWithTag(hit.collider.gameObject, "Character");
            if (characterHit)
            {
              target = hit.transform.gameObject;
              HealthDamage targetInfo = target.GetComponent<HealthDamage>();
              if (targetInfo)
              {
                CmdAttack(targetInfo.playerName);
              }
              break;
            }
            else
            {
              moveClickDown = true;
              moveClickPosition = new Vector3(hit.point.x, hit.point.y + 0.1f, hit.point.z);
              CmdMoveTo(hit.point);
              break;
            }
          }

        }
      }
      // rightClicked = true;
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
      if (healthDamage && virtualCamera)
      {
        virtualCamera.Follow = healthDamage.playerCorpse.transform;
        isCameraOnCharacter = false;
      }
    }
  }

  public void cameraOnCharacter()
  {
    if (character && virtualCamera)
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
          GameObject characters = GameObject.Find("CharactersManager");
          HealthDamage[] maybeTargets = characters.GetComponentsInChildren<HealthDamage>();
          foreach (HealthDamage maybeTarget in maybeTargets)
          {
            if (maybeTarget.playerName == name)
            {
              target = maybeTarget.gameObject;
              moveScript.hasNavigationTarget = true;
              moveScript.navigationTargetMovable = target.transform;
              moveScript.isNavigationTargetMovable = true;
              break;
            }
          }
          // }
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