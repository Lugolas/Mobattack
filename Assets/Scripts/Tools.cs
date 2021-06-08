using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine.UI;
using UnityEngine;

public class Tools : MonoBehaviour
{
  [ColorUsageAttribute(true, true)]
  public Color white;
  [ColorUsageAttribute(true, true)]
  public Color green;
  [ColorUsageAttribute(true, true)]
  public Color red;

  [ColorUsageAttribute(true, true)]
  public Color disabledColor;

  public LayerMask enemyDetectionMask;
  public LayerMask enemyPartsDetectionMask;

  public CinemachineVirtualCamera minimapCameraTerrainStatic;
  public CinemachineVirtualCamera minimapCameraTerrainFollow;
  public CinemachineVirtualCamera minimapCameraTokenStatic;
  public CinemachineVirtualCamera minimapCameraTokenFollow;

  public Button minimapToggleButton;
  public Sprite minimapSpriteStatic;
  public Sprite minimapSpriteFollow;

  public class StatModifier {
    public float value;
    public string identifier;
    public StatModifier(float newValue = 0, string newIdentifier = "") {
      value = newValue;
      identifier = newIdentifier;
    }
  }

  public static Color GetWhite() {
    return FindTools().white;
  }

  public static Color GetGreen() {
    return FindTools().green;
  }

  public static Color GetRed() {
    return FindTools().red;
  }

  public static Color GetDisabledColor() {
    return FindTools().disabledColor;
  }

  public static LayerMask GetEnemyDetectionMask() {
    return FindTools().enemyDetectionMask;
  }

  public static LayerMask GetEnemyPartsDetectionMask() {
    return FindTools().enemyPartsDetectionMask;
  }

  public static CinemachineVirtualCamera GetMinimapCameraTerrainStatic() {
    return FindTools().minimapCameraTerrainStatic;
  }
  public static CinemachineVirtualCamera GetMinimapCameraTerrainFollow() {
    return FindTools().minimapCameraTerrainFollow;
  }
  public static CinemachineVirtualCamera GetMinimapCameraTokenStatic() {
    return FindTools().minimapCameraTokenStatic;
  }
  public static CinemachineVirtualCamera GetMinimapCameraTokenFollow() {
    return FindTools().minimapCameraTokenFollow;
  }
  public static Button GetMinimapToggleButton() {
    return FindTools().minimapToggleButton;
  }
  public static Sprite GetMinimapSpriteStatic() {
    return FindTools().minimapSpriteStatic;
  }
  public static Sprite GetMinimapSpriteFollow() {
    return FindTools().minimapSpriteFollow;
  }

  private static Tools FindTools() {
    GameObject toolsObject = GameObject.Find("Tools");
    Tools tools = null;
    if (toolsObject) {
      tools = toolsObject.GetComponent<Tools>();
    }
    return tools; 
  }

  public static GameObject FindObjectOrParentWithTag(GameObject childObject, string tag)
  {
    Transform t = childObject.transform;
    if (tag == "Character")
    {
      if (t.tag == "PlayerCharacter" || t.tag == "TeamCharacter" || t.tag == "EnemyCharacter")
      {
        return t.gameObject;
      }
    }

    if (t.tag == tag)
    {
      return t.gameObject;
    }
    else
    {
      if (tag == "Character")
      {
        while (t.parent != null)
        {
          if (
            t.parent.tag == "PlayerCharacter" || 
            t.parent.tag == "TeamCharacter" || 
            t.parent.tag == "EnemyCharacter" || 
            t.parent.tag == "Character"
          )
          {
            return t.parent.gameObject;
          }
          t = t.parent.transform;
        }
      }
      else
      {
        while (t.parent != null)
        {
          if (t.parent.tag == tag)
          {
            return t.parent.gameObject;
          }
          t = t.parent.transform;
        }
      }
    }
    return null; // Could not find a parent with given tag.
  }

  static Transform GetClosestTarget(Transform source, Transform[] targets)
  {
    Transform bestTarget = null;
    float closestDistanceSqr = Mathf.Infinity;
    Vector3 currentPosition = source.position;
    foreach (Transform potentialTarget in targets)
    {
      Vector3 directionToTarget = potentialTarget.position - currentPosition;
      float dSqrToTarget = directionToTarget.sqrMagnitude;
      if (dSqrToTarget < closestDistanceSqr)
      {
        closestDistanceSqr = dSqrToTarget;
        bestTarget = potentialTarget;
      }
    }

    return bestTarget;
  }

  public static GameController getGameController()
  {
    GameObject gameManager = GameObject.Find("GameManager");
    GameController gameController = null;
    if (gameManager)
    {
      gameController = gameManager.GetComponent<GameController>();
    }
    return gameController;
  }

  public static void SetLayerRecursively(GameObject obj, int newLayer)
  {
    if (null == obj)
    {
      return;
    }

    if (obj.layer != 12)
      obj.layer = newLayer;

    foreach (Transform child in obj.transform)
    {
      if (null == child)
      {
        continue;
      }
      SetLayerRecursively(child.gameObject, newLayer);
    }
  }

  public static HealthSimple GetHealth(GameObject target) {
    HealthSimple hs = target.GetComponent<HealthSimple>();
    if (!hs) {
      hs = target.GetComponentInParent<HealthSimple>();
    }
    if (!hs) {
      hs = target.GetComponentInChildren<HealthSimple>();
    }
    return hs;
  }
  public static bool InflictDamage(Transform targetedEnemy, int damageAmount, MoneyManager moneyManager, SpellController attacker = null)
  {
    HealthSimple health = GetHealth(targetedEnemy.gameObject);
    if (health)
    {
      int reducedDamage = Mathf.RoundToInt((float) damageAmount * (100f / (100f + health.armorFinal)));
      if (health.TakeDamage(reducedDamage, attacker))
      {
        if (moneyManager) {
          moneyManager.AddMoney(health.moneyToReward);
        }
        return true;
      }
    } else {
      Debug.Log("No Health Script");
    }
    return false;
  }

  public static void ToggleMinimapMode() {
    CinemachineVirtualCamera terrainCam = GetMinimapCameraTerrainStatic();
    CinemachineVirtualCamera tokenCam = GetMinimapCameraTokenStatic();
    Image minimapToggleImage = GetMinimapToggleButton().GetComponent<Image>();
    if (terrainCam.enabled)
    {
      terrainCam.enabled = false;
      tokenCam.enabled = false;
      minimapToggleImage.sprite = GetMinimapSpriteFollow();
    }
    else
    {
      terrainCam.enabled = true;
      tokenCam.enabled = true;
      minimapToggleImage.sprite = GetMinimapSpriteStatic();
    }
  }

  public static void ZoomUp() {
    CinemachineVirtualCamera terrainCamStatic = GetMinimapCameraTerrainStatic();
    if(terrainCamStatic.enabled) {
      ZoomLevels zoomLevels = terrainCamStatic.GetComponent<ZoomLevels>();
      if (zoomLevels.currentZoomIndex > 0) 
      {
        zoomLevels.currentZoomIndex -= 1;
        CinemachineVirtualCamera tokenCamStatic = GetMinimapCameraTokenStatic();
        SetVirtualCameraDistancePair(terrainCamStatic, tokenCamStatic, zoomLevels.zoomValues[zoomLevels.currentZoomIndex]);
      }
    } else {
      CinemachineVirtualCamera terrainCamFollow = GetMinimapCameraTerrainFollow();
      ZoomLevels zoomLevels = terrainCamFollow.GetComponent<ZoomLevels>();
      if (zoomLevels.currentZoomIndex > 0)
      {
        zoomLevels.currentZoomIndex -= 1;
        CinemachineVirtualCamera tokenCamFollow = GetMinimapCameraTokenFollow();
        SetVirtualCameraDistancePair(terrainCamFollow, tokenCamFollow, zoomLevels.zoomValues[zoomLevels.currentZoomIndex]);
      }
    }
  }

  public static void ZoomDown() {
    CinemachineVirtualCamera terrainCamStatic = GetMinimapCameraTerrainStatic();
    if(terrainCamStatic.enabled) {
      ZoomLevels zoomLevels = terrainCamStatic.GetComponent<ZoomLevels>();
      if (zoomLevels.currentZoomIndex < zoomLevels.zoomValues.Count - 1) 
      {
        zoomLevels.currentZoomIndex += 1;
        CinemachineVirtualCamera tokenCamStatic = GetMinimapCameraTokenStatic();
        SetVirtualCameraDistancePair(terrainCamStatic, tokenCamStatic, zoomLevels.zoomValues[zoomLevels.currentZoomIndex]);
      }
    } else {
      CinemachineVirtualCamera terrainCamFollow = GetMinimapCameraTerrainFollow();
      ZoomLevels zoomLevels = terrainCamFollow.GetComponent<ZoomLevels>();
      if (zoomLevels.currentZoomIndex < zoomLevels.zoomValues.Count - 1)
      {
        zoomLevels.currentZoomIndex += 1;
        CinemachineVirtualCamera tokenCamFollow = GetMinimapCameraTokenFollow();
        SetVirtualCameraDistancePair(terrainCamFollow, tokenCamFollow, zoomLevels.zoomValues[zoomLevels.currentZoomIndex]);
      }
    }
  }

  public static void ZoomRefresh() {
    CinemachineVirtualCamera terrainCamStatic = GetMinimapCameraTerrainStatic();
    ZoomLevels zoomLevelsStatic = terrainCamStatic.GetComponent<ZoomLevels>();
    CinemachineVirtualCamera tokenCamStatic = GetMinimapCameraTokenStatic();
    SetVirtualCameraDistancePair(terrainCamStatic, tokenCamStatic, zoomLevelsStatic.zoomValues[zoomLevelsStatic.currentZoomIndex]);
    
    CinemachineVirtualCamera terrainCamFollow = GetMinimapCameraTerrainFollow();
    ZoomLevels zoomLevelsFollow = terrainCamFollow.GetComponent<ZoomLevels>();
    CinemachineVirtualCamera tokenCamFollow = GetMinimapCameraTokenFollow();
    SetVirtualCameraDistancePair(terrainCamFollow, tokenCamFollow, zoomLevelsFollow.zoomValues[zoomLevelsFollow.currentZoomIndex]);
  }

  public static void SetVirtualCameraDistance(CinemachineVirtualCamera virtualCamera, float distance) {
    CinemachineComponentBase componentBase = virtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
    if (componentBase is CinemachineFramingTransposer)
    {
      (componentBase as CinemachineFramingTransposer).m_CameraDistance = distance;
    }
  }

  public static void SetVirtualCameraDistancePair(
    CinemachineVirtualCamera virtualCamera1, 
    CinemachineVirtualCamera virtualCamera2, 
    float distance
  ) {
    SetVirtualCameraDistance(virtualCamera1, distance);
    SetVirtualCameraDistance(virtualCamera2, distance);
  }

  public static bool AddStatModifier(List<StatModifier> statModifiers, float value, string identifier) {
    bool found = false;
    bool updateNeeded = true;
    foreach (StatModifier statModifier in statModifiers)
    {
      if (statModifier.identifier == identifier) {
        if (statModifier.value == value) {
          updateNeeded = false;
        } else {
          statModifier.value = value;
        }
        found = true;
      }
    }
    if (!found) {
      statModifiers.Add(new StatModifier(value, identifier));
    }
    return updateNeeded;
  }
  public static bool RemoveStatModifier(List<StatModifier> statModifiers, string identifier) {
    bool updateNeeded = false;
    StatModifier stat = new StatModifier();
    for (int i = statModifiers.Count-1; i > -1; i--)
    {
      if (statModifiers[i].identifier == identifier) {
        stat = statModifiers[i];
        statModifiers.Remove(stat);
        updateNeeded = true;
      }
    }
    return updateNeeded;
  }

  public static Transform GetMainCanvasTransform() {
    GameObject canvas = GetMainCanvasObject();
    Transform canvasTransform = null;
    if (canvas) {
      canvasTransform = canvas.transform;
    }
    return canvasTransform;
  }
  public static GameObject GetMainCanvasObject() {
    return GameObject.Find ("Canvas");
  }
}