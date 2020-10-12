using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TurretPlayerLink : MonoBehaviour
{
  GameObject playerCharacter;
  public MoneyManager characterWallet;
  TurretSpaceCheck turretSpaceCheck;
  NavMeshObstacle navMeshObstacle;
  public GameObject visualSpace;
  public GameObject visualRange;
  public bool activated = false;


  private void Start()
  {
    turretSpaceCheck = GetComponent<TurretSpaceCheck>();
    navMeshObstacle = GetComponentInChildren<NavMeshObstacle>(true);
  }

  public void InitialLink(GameObject character, MoneyManager wallet)
  {
    playerCharacter = character;
    characterWallet = wallet;
  }

  public bool HasEnoughSpace()
  {
    return turretSpaceCheck.enoughSpace;
  }

  public void Activate()
  {
    navMeshObstacle.enabled = true;
    activated = true;
    SetVisuals(false, false);
  }

  void SetVisuals(bool range, bool space)
  {
    visualRange.SetActive(range);
    visualSpace.SetActive(space);
  }
}
