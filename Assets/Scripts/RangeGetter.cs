using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeGetter : MonoBehaviour
{
  public int mainDamageAmount = 75;
  public int damageAmount = 50;
  string nameOfAttacker;
  BoxCollider hitbox;
  SphereCollider hitsphere;
  public LayerMask mask;
  public Vector3 hitboxSize;
  public float hitsphereRadius;
  public Color inactiveColor;
  public Color collisionOpenColor;
  public Color collidingColor;
  private ColliderState state;
  private BaseMoveAttacc baseMoveAttacc;
  private List<string> nameOfCollidersAlreadyChecked = new List<string>();


  public enum ColliderState
  {
    Closed,
    Open,
    Colliding
  }

  // Start is called before the first frame update
  void Start()
  {
    GameObject parentCharacter = Tools.FindObjectOrParentWithTag(gameObject, "Character");
    baseMoveAttacc = parentCharacter.GetComponent<BaseMoveAttacc>();
    nameOfAttacker = parentCharacter.name;
    hitbox = GetComponent<BoxCollider>();
    hitsphere = GetComponent<SphereCollider>();
    if (hitbox)
    {
      hitboxSize = hitbox.size;
    }
    else if (hitsphere)
    {
      hitsphereRadius = hitsphere.radius;
    }
  }

  // Update is called once per frame
  void Update()
  {
    if (hitbox.enabled && state == ColliderState.Closed)
    {
      startCheckingCollision();
    }
    else if (!hitbox.enabled && state != ColliderState.Closed)
    {
      stopCheckingCollision();
      nameOfCollidersAlreadyChecked.Clear();
    }

    if (state == ColliderState.Closed) { return; }

    Collider[] colliders = Physics.OverlapBox(transform.position, hitboxSize, transform.rotation, mask);

    if (colliders.Length > 0)
    {
      state = ColliderState.Colliding;

      foreach (Collider collider in colliders)
      {
        GameObject collidedCharacter = Tools.FindObjectOrParentWithTag(collider.gameObject, "Character");

        if (collidedCharacter != null && collidedCharacter.name != nameOfAttacker && !nameOfCollidersAlreadyChecked.Contains(collidedCharacter.name))
        {
          nameOfCollidersAlreadyChecked.Add(collidedCharacter.name);
          HealthSimple health = collidedCharacter.GetComponent<HealthSimple>();
          if (health)
          {
            if (collidedCharacter.name == baseMoveAttacc.targetedEnemy.name)
            {
              health.TakeDamage(mainDamageAmount);
            }
            else
            {
              health.TakeDamage(damageAmount);
            }
          }
        }
      }
    }
    else
    {
      state = ColliderState.Open;
    }
  }

  private void OnDrawGizmos()
  {
    checkGizmoColor();

    Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);

    Gizmos.DrawCube(Vector3.zero, new Vector3(hitboxSize.x * 2, hitboxSize.y * 2, hitboxSize.z * 2)); // Because size is halfExtents

  }

  private void checkGizmoColor()
  {
    switch (state)
    {
      case ColliderState.Closed:
        Gizmos.color = inactiveColor;
        break;
      case ColliderState.Open:
        Gizmos.color = collisionOpenColor;
        break;
      case ColliderState.Colliding:
        Gizmos.color = collidingColor;
        break;
    }
  }

  void OnCollisionStay(Collision collision)
  {
    Debug.Log("WIIHOUUWIIHOUU");
    Debug.Log(collision.gameObject.name);
    GameObject collidedCharacter = Tools.FindObjectOrParentWithTag(collision.gameObject, "Character");

    if (collidedCharacter != null && collidedCharacter.name != nameOfAttacker)
    {
      HealthSimple health = collidedCharacter.GetComponent<HealthSimple>();
      if (health)
      {
        health.TakeDamage(damageAmount);
      }
    }
  }

  public void startCheckingCollision()
  {
    state = ColliderState.Open;
  }

  public void stopCheckingCollision()
  {
    state = ColliderState.Closed;
  }
}
