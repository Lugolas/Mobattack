using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryCollider : MonoBehaviour
{
  public bool hasTouched = false;
  void OnCollisionEnter(Collision collision)
  {
    hasTouched = true;
    Debug.Log("COUCOU C KIKI LE PETIT COUCOU");
    CharacterManager character = collision.collider.GetComponent<CharacterManager>();
    if (character)
    {
      Debug.Log(gameObject.name.Substring(gameObject.name.Length - 2));
      if (character.team.ToString() == gameObject.name.Substring(gameObject.name.Length - 2))
      {

      }
    }
  }
}
