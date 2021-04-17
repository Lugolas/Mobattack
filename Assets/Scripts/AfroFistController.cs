using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AfroFistController : MonoBehaviour {
  public int id = -1;
  public int damageInitial = 25;
  public int damageModifiedBase = 25;
  public int damageFinal = 25;
  public float baseDamageModifier = 0.5f;
  float outsideDamageModifier = 0f;
  public GameObject attacker;
  public GameObject fistBurstPrefab;
  public bool useLifeTimeLimit = false;
  public float lifeTimeLimit = 15;

  public float movementSpeed = 5.0f;
  private TrailRenderer trail;
  private SphereCollider sphereCollider;
  private MeshRenderer meshRenderer;
  private Rigidbody rigidbodyFist;
  public RigidbodyConstraints constraints;

  private float startTime;
  private bool hasHit = false;
  public MoneyManager characterWallet;
  public SpellControllerAfro spellController;
  AfroFistDamage fistDamage;
  bool initiatedSelfDestruction;
  bool hasCollided = false;
  Collision lastCollision;
  SpriteRenderer[] sprites;
  bool fired = false;

  void Start () {
    startTime = Time.time;

    trail = GetComponent<TrailRenderer> ();
    sphereCollider = GetComponent<SphereCollider> ();
    meshRenderer = GetComponent<MeshRenderer> ();
    rigidbodyFist = GetComponent<Rigidbody> ();
    fistDamage = GetComponent<AfroFistDamage> ();
    sprites = GetComponentsInChildren<SpriteRenderer>();

    if (useLifeTimeLimit) {
      Destroy (gameObject, lifeTimeLimit);
    }
  }
  void Update () {
    if (useLifeTimeLimit && lifeTimeLimit > 5 && Time.time >= (startTime + lifeTimeLimit - 5) && !initiatedSelfDestruction) {
      if (trail) {
        trail.emitting = false;
      }
      initiatedSelfDestruction = true;
    }

    outsideDamageModifier = fistDamage.outsideDamageModifier;
    // rigidbody.velocity = transform.forward * movementSpeed;

    if (fired && Vector3.Distance(transform.position, Vector3.zero) > 100) {
      Destroy(gameObject);
    }
  }

  void FixedUpdate () {
    damageInitial = Mathf.RoundToInt (rigidbodyFist.velocity.magnitude);
    damageModifiedBase = Mathf.RoundToInt (damageInitial + (damageInitial * baseDamageModifier));
    damageFinal = Mathf.RoundToInt (damageModifiedBase + (damageModifiedBase * outsideDamageModifier));
  }

  void LateUpdate () {
    if (hasCollided && !hasHit && !initiatedSelfDestruction) {
      hasCollided = false;
      GameObject objectHit = Tools.FindObjectOrParentWithTag (lastCollision.collider.gameObject, "EnemyCharacter");
      if (objectHit && objectHit != attacker) {
        if (trail) {
          trail.emitting = false;
        }
        foreach (SpriteRenderer sprite in sprites)
        {
          sprite.enabled = false;
        }
        sphereCollider.enabled = false;
        meshRenderer.enabled = false;
        hasHit = true;
        initiatedSelfDestruction = true;
        if (Tools.InflictDamage(lastCollision.collider.transform, damageFinal, characterWallet, gameObject)) {
          spellController.FistKilledEnemy();
        }
        if (fistBurstPrefab) {
          GameObject burst = Instantiate (fistBurstPrefab, transform.position, transform.rotation) as GameObject;
          // GameObject burst = Instantiate(fistBurstPrefab, collision.GetContact(0).point, transform.rotation) as GameObject;

          Destroy (burst, 2f);
          Destroy (gameObject, 5.5f);
        } else {
          Destroy (gameObject, 5.5f);
          for (int i = 0; i < transform.childCount; i++) {
            Destroy (transform.GetChild (i).gameObject);
          }
        }
      }
    }
  }

  public void Fire () {
    sphereCollider.enabled = true;
    rigidbodyFist.isKinematic = false;
    rigidbodyFist.constraints = constraints;
    transform.localScale = Vector3.one;
    fired = true;

    rigidbodyFist.AddForce (spellController.body.transform.forward * 250f * 2, ForceMode.Impulse);
  }

  void OnCollisionEnter (Collision collision) {
    hasCollided = true;
    lastCollision = collision;
  }
}