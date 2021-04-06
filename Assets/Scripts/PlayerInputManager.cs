using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerInputManager : NetworkBehaviour
{
  int characterIndex;
  // Index of the right character in the array of the children of the CharactersManager
  public GameObject targetPointer;
  public GameObject character;
  public GameObject target;

  bool hasSetupCharacter = false;
  bool hasCharacter = false;
  bool disable = false;

  bool moveClickDown = false;
  bool createModeOn = false;
  Vector3 moveClickPosition = Vector3.zero;
  List<Material> outlinedCharacterMaterials = new List<Material>();
  string outlinedCharacterName = null;
  int currentZoomIndex;
  float[] zoomLevels = { 5, 10, 15, 20, 25, 30 };
  public CinemachineVirtualCamera virtualCamera;
  public CinemachineVirtualCamera verticalCamera;  
  public CinemachineVirtualCamera verticalMinimapCameraTerrain;
  public CinemachineVirtualCamera verticalMinimapCameraToken;
  private bool isCameraOnCharacter = false;
  public Transform spawnPoint;
  GameObject teamPanel;
  SelectionButtonsController teamButtons;
  float hasTriedToSetupCharacter;
  GameObject charactersManager;
  NavMeshAgent characterNavAgent;
  string TEMP_NAME = "Girouette";
  int GROUND_LAYER = 8;
  int UI_LAYER = 5;
  int clientIdLast;
  public LayerMask layerMaskGround;
  public LayerMask layerMaskMove;
  public LayerMask layerMaskCheck;
  Vector3 turretCreationPoint;
  GameObject turretPrefab;
  GameObject enemyManager;
  GameObject previewTurret;
  TurretPlayerLink previewTurretPlayerLink;
  TurretSpaceCheck previewTurretSpaceCheck;
  bool isCameraVertical = false;
  CheckInfo checking;
  GraphicRaycaster raycaster;
  PointerEventData pointerEventData;
  EventSystem eventSystem;
  SpellController spellController;
  bool fire1Down = false;
  bool fire2Down = false;


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
      raycaster = GameObject.Find("Canvas").GetComponent<GraphicRaycaster>();
      eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
      enemyManager = GameObject.Find("EnemyManager");
      charactersManager = GameObject.Find("CharactersManager");
      teamPanel = GameObject.Find("TeamPanel");
      hasTriedToSetupCharacter = Time.time;

      if (!spawnPoint)
      {
        GameObject spawnObject = GameObject.Find("Spawn");
        if (spawnObject)
        {
          spawnPoint = spawnObject.transform;
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

      turretPrefab = Resources.Load<GameObject>("Prefabs/Turrets/Turret");
    }
    currentZoomIndex = zoomLevels.Length - 2;
  }

  void SetupCharacter(string characterName)
  {
    GameObject characterPrefab = null;

    characterPrefab = Resources.Load<GameObject>("Prefabs/Characters/afro");

    if (characterPrefab)
    {
      GameObject spawnPoint = GameObject.Find("Spawn");
      character = Instantiate(characterPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
      characterNavAgent = character.GetComponent<NavMeshAgent>();
      if (!characterNavAgent) {
        characterNavAgent = character.GetComponentInChildren<NavMeshAgent>();
      }
      CharacterManager characterManager = character.GetComponent<CharacterManager>();
      HealthDamage healthDamage = character.GetComponent<HealthDamage>();
      spellController = character.GetComponent<SpellController>();

      characterManager.clientId = connectionToClient.connectionId;
      characterManager.player = gameObject;
      healthDamage.entityName = characterName;

      if (isLocalPlayer)
      {
        character.tag = "PlayerCharacter";
        virtualCamera = GameObject.Find("CameraInitiale").GetComponent<CinemachineVirtualCamera>();
        verticalCamera = GameObject.Find("CameraVerticale").GetComponent<CinemachineVirtualCamera>();
        verticalMinimapCameraTerrain = GameObject.Find("CameraTerrainFollow").GetComponent<CinemachineVirtualCamera>();
        verticalMinimapCameraToken = GameObject.Find("CameraTokenFollow").GetComponent<CinemachineVirtualCamera>();
        virtualCamera.Follow = characterNavAgent.transform;
        verticalCamera.Follow = characterNavAgent.transform;
        verticalMinimapCameraTerrain.Follow = characterNavAgent.transform;
        verticalMinimapCameraToken.Follow = characterNavAgent.transform;
        CameraZoomUpdate();
        isCameraOnCharacter = true;
      }

      hasCharacter = true;
      hasSetupCharacter = true;
    }
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
    if (!hasSetupCharacter)
    {
      CharacterManager[] characterManagers = charactersManager.GetComponentsInChildren<CharacterManager>();

      hasTriedToSetupCharacter = Time.time;
      SetupCharacter(TEMP_NAME);
    }
    // if (!isLocalPlayer)
    // {
    Ray rayMouse = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit hitMouse;

    if (Physics.Raycast(rayMouse, out hitMouse, 2500, layerMaskMove))
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

      CameraZoomUpdate();
    }

    if (Input.GetKeyDown(KeyCode.F))
    {
      if (isCameraVertical)
      {
        isCameraVertical = false;
        virtualCamera.enabled = true;
      }
      else
      {
        isCameraVertical = true;
        virtualCamera.enabled = false;
      }
    }

    if (spellController)
    {
      if (Input.GetKeyDown(KeyCode.A))
      {
        spellController.Spell1();
      }
      if (Input.GetKeyDown(KeyCode.Z))
      {
        spellController.Spell2();
      }
      if (Input.GetKeyDown(KeyCode.E))
      {
        spellController.Spell3();
      }

      if (Input.GetButton("Fire1"))
      {
        fire1Down = true;
        bool fireUsed = spellController.Fire1(fire1Down);
        if (!fireUsed)
        {
          bool UIHit = false;

          pointerEventData = new PointerEventData(eventSystem);
          pointerEventData.position = Input.mousePosition;

          List<RaycastResult> results = new List<RaycastResult>();
          raycaster.Raycast(pointerEventData, results);
          foreach (RaycastResult result in results)
          {
            UIHit = true;
          }
          if (!UIHit)
          {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray, 2500, layerMaskCheck);
            if (hits.Length > 0)
            {
              System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));
              foreach (RaycastHit hit in hits)
              {
                CheckInfo checkableHit = hit.collider.gameObject.GetComponent<CheckInfo>();
                if (checkableHit)
                {
                  if (checking != checkableHit)
                  {
                    if (checking)
                    {
                      checking.StopCheck();
                    }
                    checking = checkableHit;
                    checkableHit.Check();
                  }
                  break;
                }
                else
                {
                  if (hit.collider.gameObject.layer == UI_LAYER)
                  {
                    break;
                  }
                  if (hit.collider.gameObject.layer == GROUND_LAYER)
                  {
                    if (checking)
                    {
                      checking.StopCheck();
                      checking = null;
                    }
                    break;
                  }
                }
              }
            }
          }
        }
      } else if (fire1Down) 
      {
        fire1Down = false;
        spellController.Fire1(fire1Down);
      }

      if (Input.GetButton("Fire2"))
      {
        fire2Down = true;
        spellController.Fire2(fire2Down);
      }
      else if (fire2Down)
      {
        fire2Down = false;
        spellController.Fire2(fire2Down);
      }
    }
  }

  void CameraZoomUpdate()
  {
    if (virtualCamera)
    {
      CinemachineComponentBase componentBase = virtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
      if (componentBase is CinemachineFramingTransposer)
      {
        (componentBase as CinemachineFramingTransposer).m_CameraDistance = zoomLevels[currentZoomIndex]; // your value
      }

      CinemachineComponentBase componentBaseVertical = verticalCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
      if (componentBaseVertical is CinemachineFramingTransposer)
      {
        (componentBaseVertical as CinemachineFramingTransposer).m_CameraDistance = zoomLevels[currentZoomIndex] + 5; // your value
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
        virtualCamera.Follow = healthDamage.corpse.transform;
        verticalCamera.Follow = healthDamage.corpse.transform;
        verticalMinimapCameraTerrain.Follow = healthDamage.corpse.transform;
        verticalMinimapCameraToken.Follow = healthDamage.corpse.transform;
        isCameraOnCharacter = false;
      }
    }
  }

  public void cameraOnCharacter()
  {
    if (character && virtualCamera)
    {
      virtualCamera.Follow = characterNavAgent.transform;
      verticalCamera.Follow = characterNavAgent.transform;
      verticalMinimapCameraTerrain.Follow = characterNavAgent.transform;
      verticalMinimapCameraToken.Follow = characterNavAgent.transform;
      isCameraOnCharacter = true;
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