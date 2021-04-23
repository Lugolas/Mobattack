using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AfroFistController : MonoBehaviour {
  public int id = -1;
  public int damageInitial = 0;
  public int damageModifiedBase = 0;
  public int damageFinal = 0;
  public float baseDamageModifier = 0f;
  float outsideDamageModifier = 0f;
  public GameObject attacker;
  public GameObject fistBurstPrefab;
  public bool useLifeTimeLimit = false;
  public float lifeTimeLimit = 15;

  public float movementSpeed = 5.0f;
  private TrailRenderer trail;
  private SphereCollider sphereCollider;
  private MeshRenderer meshRenderer;
  Material material;
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
  Color color;
  Color outlineColor;
  Vector3 lastPosition;
  float lastpositionTime;
  float lastMagnitude = 0;
  float newMagnitude = 0;
  float lastMagnitudeChange = 0;
  float magnitudeChangeDelay = 0.1f;
  float magnitudeTransitionPoint = 0;
  float colorMultiplier = 1;
  public Vector3 velocity;
  Vector3 velocityLast;
  public float trailTime;
  public Transform trailColliderSpawn;
  public GameObject trailColliderPrefab;
  TrailColliderController trailCollider;

  void Start () {
    startTime = Time.time;
    lastPosition = transform.position;
    lastpositionTime = Time.time;
    trail = GetComponent<TrailRenderer> ();
    trailTime = trail.time;
    sphereCollider = GetComponent<SphereCollider> ();
    meshRenderer = GetComponent<MeshRenderer> ();
    material = meshRenderer.material;
    outlineColor = Color.HSVToRGB(50, 0, 0);
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
    if (spellController.speedAffectsFists) {
      float magnitude = 0;
      if (Time.time > lastMagnitudeChange + magnitudeChangeDelay) {
        magnitude = newMagnitude;
        lastMagnitude = newMagnitude;
        newMagnitude = (transform.position - lastPosition).magnitude / (Time.time - lastpositionTime);
        lastMagnitudeChange = Time.time;
      } else {
        magnitudeTransitionPoint = (Time.time - lastMagnitudeChange) / magnitudeChangeDelay;
        magnitude = Mathf.Lerp(lastMagnitude, newMagnitude, magnitudeTransitionPoint);
      }
      lastPosition = transform.position;
      lastpositionTime = Time.time;
      float colorMagnitude = (magnitude * 5) / 100f;

      if (colorMagnitude > 1) {
        colorMultiplier = (colorMagnitude - 1) * 10;
        if (colorMultiplier < 1) {
          colorMultiplier = 1;
        }
      } else {
        colorMultiplier = 1;
      }

      color = Color.HSVToRGB(0f/360f, 1, colorMagnitude);
      color = new Color(color.r * colorMultiplier, color.g * colorMultiplier, color.b * colorMultiplier, color.a);
      outlineColor = Color.HSVToRGB(50f/360f, 1, colorMagnitude);
      outlineColor = new Color(outlineColor.r * colorMultiplier, outlineColor.g * colorMultiplier, outlineColor.b * colorMultiplier, outlineColor.a);
      material.SetColor("_Color", color);
      material.SetColor("_OutlineColor", outlineColor);
      damageInitial = Mathf.RoundToInt (magnitude);

    }

    // velocity = new Vector3(rigidbodyFist.velocity.x, rigidbodyFist.velocity.y, 0);
    velocity = rigidbodyFist.velocity;
    if (velocity != velocityLast) {
      velocityLast = velocity;
      OnVelocityChange();
    }
    if (velocity != Vector3.zero) {
      transform.rotation = Quaternion.LookRotation(velocity);
    }

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

  void OnVelocityChange() {
    if (trailCollider) {
      trailCollider.Detach();
    }
    GameObject trailObject = Instantiate(trailColliderPrefab, trailColliderSpawn.position, trailColliderSpawn.rotation);
    trailObject.transform.SetParent(trailColliderSpawn);
    trailCollider = trailObject.GetComponent<TrailColliderController>();
  }

  public void Fire () {
    damageInitial = spellController.healthScript.damageFinal;
    sphereCollider.enabled = true;
    rigidbodyFist.isKinematic = false;
    rigidbodyFist.constraints = constraints;
    transform.localScale = Vector3.one;
    fired = true;

    rigidbodyFist.AddForce (spellController.body.transform.forward * 250f * 2, ForceMode.Impulse);
    velocityLast = rigidbodyFist.velocity;
  }

  void OnCollisionEnter (Collision collision) {
    hasCollided = true;
    lastCollision = collision;
  }
}