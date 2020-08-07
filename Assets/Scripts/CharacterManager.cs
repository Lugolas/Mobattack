using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Networking;

public class CharacterManager : NetworkBehaviour
{
  [SyncVar]
  public int clientId = -1;
  [SyncVar]
  public int team = 0;
  public GameObject player;
  public int playerCharacterTeam;
  public bool isPlayerCharacter = false;
  GameObject charactersManager;
  bool teamsChanged = true;
  private Color playerGreen = new Color(73 / 255, 255 / 255, 106 / 255);
  private Color teammateBlue = new Color(106 / 255, 73 / 255, 255 / 255);
  private Color enemyRed = new Color(255 / 255, 106 / 255, 73 / 255);

  void Start()
  {
    charactersManager = GameObject.Find("CharactersManager");
    if (charactersManager)
    {
      transform.SetParent(charactersManager.transform);
    }
  }

  void Update()
  {
    checkTeamStatus();
    if (teamsChanged)
    {
      assignTeam();
    }
  }

  public void assignTeam()
  {
    Color color;
    if (isPlayerCharacter)
    {
      color = playerGreen;
      gameObject.tag = "PlayerCharacter";
    }
    else if (team == playerCharacterTeam)
    {
      color = teammateBlue;
      gameObject.tag = "TeamCharacter";
    }
    else
    {
      color = enemyRed;
      gameObject.tag = "EnemyCharacter";
    }

    // Change Health Bar Color
    Image healthBar;
    healthBar = gameObject.GetComponent<HealthDamage>().playerHealthBar;
    if (healthBar)
      healthBar.color = color;

    // Change Token Pointer Color
    GameObject pointer;
    pointer = gameObject.GetComponentInChildren<TokenPointer>().gameObject;
    Renderer rendererPointer;
    rendererPointer = pointer.GetComponent<Renderer>();
    List<Material> materialPointers = new List<Material>();
    rendererPointer.GetMaterials(materialPointers);
    foreach (Material materialPointer in materialPointers)
    {
      materialPointer.SetColor("_EmissionColor", color);
    }

    // Change Outline Color
    Renderer[] renderers;
    renderers = gameObject.GetComponentsInChildren<Renderer>();
    foreach (Renderer renderer in renderers)
    {
      List<Material> materials = new List<Material>();
      renderer.GetMaterials(materials);
      foreach (Material material in materials)
      {
        material.SetColor("OutlineColor", color);
      }
    }
  }

  void checkTeamStatus()
  {
    if (gameObject.tag == "PlayerCharacter")
    {
      if (playerCharacterTeam != team || isPlayerCharacter != true)
      {
        playerCharacterTeam = team;
        isPlayerCharacter = true;
        teamsChanged = true;
      }
      else
      {
        teamsChanged = false;
      }
    }
    else
    {
      if (charactersManager)
      {
        CharacterManager[] characterManagers = charactersManager.GetComponentsInChildren<CharacterManager>();
        foreach (CharacterManager characterManager in characterManagers)
        {
          GameObject character = characterManager.gameObject;
          if (character.name != gameObject.name && character.tag == "PlayerCharacter")
          {
            if (playerCharacterTeam != characterManager.team || isPlayerCharacter != false)
            {
              playerCharacterTeam = characterManager.team;
              isPlayerCharacter = false;
              teamsChanged = true;
            }
            else
            {
              teamsChanged = false;
            }
          }
        }
      }
    }
  }
}
