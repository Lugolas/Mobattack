using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AfroFistController : MonoBehaviour {
  public int id = -1;
  public int damageBase = 0;
  List<Tools.StatModifier> damageBaseMultipliers = new List<Tools.StatModifier>();
  List<Tools.StatModifier> damageAdditions = new List<Tools.StatModifier>();
  List<Tools.StatModifier> damageMultipliers = new List<Tools.StatModifier>();
  public int damageFinal;
  public GameObject attacker;
  public GameObject fistBurstPrefab;
  public bool useLifeTimeLimit = false;
  public float lifeTimeLimit = 15;

  public float movementSpeed = 5.0f;
  public TrailRenderer trail;
  // public TrailRenderer trailIN;
  // public TrailRenderer trailOUT;
  private SphereCollider sphereCollider;
  private MeshRenderer meshRenderer;
  Material material;
  public Rigidbody rigidbodyFist;
  public RigidbodyConstraints constraints;

  private float startTime;
  private bool hasHit = false;
  public MoneyManager characterWallet;
  public SpellControllerAfro spellController;
  bool initiatedSelfDestruction;
  bool hasCollided = false;
  Collision lastCollision;
  SpriteRenderer[] sprites;
  bool fired = false;
  Color color;
  Color trailColor;
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
  float trailColliderRadius;
  TrailColliderController trailCollider;
  bool velocityChanged = false;
  Vector3 lastTrailColliderSpawnPosition = Vector3.zero;
  bool spawnedOneTrailCollider = false;
  public float magnitude;
  public List<HealthSimple> trailTargetList = new List<HealthSimple>();
  List<TrailColliderController> trailColliders = new List<TrailColliderController>();
  float lastHit = 0;
  float hitDelay = 0.25f;
  float launchForce = 100;

  void Start () {
    startTime = Time.time;
    lastPosition = transform.position;
    lastpositionTime = Time.time;
    trail.emitting = false;
    // trailIN.emitting = false;
    // trailOUT.emitting = false;
    trailTime = trail.time;
    // trailTime = trailIN.time;
    sphereCollider = GetComponent<SphereCollider> ();
    meshRenderer = GetComponent<MeshRenderer> ();
    material = meshRenderer.material;
    outlineColor = Color.HSVToRGB(50, 0, 0);
    sprites = GetComponentsInChildren<SpriteRenderer>();
    trailColliderRadius = trailColliderPrefab.GetComponent<CapsuleCollider>().radius;


    if (useLifeTimeLimit) {
      Destroy (gameObject, lifeTimeLimit);
    }
    UpdateDamage();
  }
  void Update () {
    if (useLifeTimeLimit && lifeTimeLimit > 5 && Time.time >= (startTime + lifeTimeLimit - 5) && !initiatedSelfDestruction) {
      if (trail) {
      // if (trailIN && trailOUT) {
        trail.emitting = false;
        // trailIN.emitting = false;
        // trailOUT.emitting = false;
      }
      initiatedSelfDestruction = true;
    }

    // rigidbody.velocity = transform.forward * movementSpeed;

    if (fired && Vector3.Distance(transform.position, Vector3.zero) > 100) {
      Destroy(gameObject);
    }
  }

  void FixedUpdate () {
    if (spellController.speedAffectsFists) {
      // magnitude = 0;
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
      // trailColor = Color.HSVToRGB(0f/360f, colorMagnitude, 1);
      color = new Color(color.r * colorMultiplier, color.g * colorMultiplier, color.b * colorMultiplier, color.a);
      trailColor = new Color(color.r * 1000, color.g * 1000, color.b * 1000, color.a);
      outlineColor = Color.HSVToRGB(50f/360f, 1, colorMagnitude);
      outlineColor = new Color(outlineColor.r * colorMultiplier, outlineColor.g * colorMultiplier, outlineColor.b * colorMultiplier, outlineColor.a);
      material.SetColor("_Color", color);
      trail.material.SetColor("_Color", trailColor);
      // trailIN.material.SetColor("_BaseColor", trailColor);
      material.SetColor("_OutlineColor", outlineColor);
      trail.material.SetColor("_OutlineColor", outlineColor);
      // trailOUT.material.SetColor("_BaseColor", outlineColor);
      AddDamageBaseMultiplier(1 + (magnitude / 20), "speed");
    }

    velocity = new Vector3(rigidbodyFist.velocity.x, 0, rigidbodyFist.velocity.z);
    // velocity = rigidbodyFist.velocity;
    if (fired && velocity != velocityLast) {
      velocityLast = velocity;
      OnVelocityChange();
    }
    if (!fired && spellController.IsInBreakerUlt()) {
      SpawnTrailCollider();
    }
    if (velocity != Vector3.zero) {
      transform.rotation = Quaternion.LookRotation(velocity);
    }

    if (Time.time > lastHit + hitDelay) {
      TrailTargetListPrep();
      lastHit = Time.time;
      TrailTargetListHit();
    }
  }

  void LateUpdate () {
    if (hasCollided && !hasHit && !initiatedSelfDestruction) {
      hasCollided = false;
      GameObject objectHit = Tools.FindObjectOrParentWithTag (lastCollision.collider.gameObject, "EnemyCharacter");
      if (objectHit && objectHit != attacker) {
        if (trail) {
        // if (trailIN && trailOUT) {
          trail.emitting = false;
          // trailIN.emitting = false;
          // trailOUT.emitting = false;
        }
        foreach (SpriteRenderer sprite in sprites)
        {
          sprite.enabled = false;
        }
        sphereCollider.enabled = false;
        meshRenderer.enabled = false;
        hasHit = true;
        initiatedSelfDestruction = true;
        if (Tools.InflictDamage(lastCollision.collider.transform, damageFinal, characterWallet, spellController)) {
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

  void TrailTargetListPrep() {
    for (int i = trailColliders.Count-1; i > -1; i--)
    {
      if (trailColliders[i]) {
        List<HealthSimple> targetList = trailColliders[i].targetList;
        for (int j = targetList.Count-1; j > -1; j--)
        {
          if (targetList[j]) {
            if (!trailTargetList.Contains(targetList[j])) {
              trailTargetList.Add(targetList[j]);
            }
          } else {
            targetList.RemoveAt(j);
          }
        }
      } else {
        trailColliders.RemoveAt(i);
      }
    }
  }

  void TrailTargetListHit() {
    for (int i = trailTargetList.Count-1; i > -1; i--)
    {
      if(trailTargetList[i]) {
        Tools.InflictDamage(
          trailTargetList[i].transform,
          Mathf.RoundToInt(Mathf.RoundToInt(damageFinal / 2f) * hitDelay),
          characterWallet,
          spellController
        );
      } else {
        trailTargetList.RemoveAt(i);
      }
    }
  }

  void OnVelocityChange() {
    velocityChanged = true;
    if (trailCollider) {
      trailCollider.Detach();
    }
    SpawnTrailCollider();
  }

  void SpawnTrailCollider() {
    if (trail.emitting && !initiatedSelfDestruction && spellController.trailUnlocked) {
      if (Vector3.Distance(trailColliderSpawn.position, lastTrailColliderSpawnPosition) >= trailColliderRadius * 2) {
        GameObject trailObject = Instantiate(trailColliderPrefab, trailColliderSpawn.position, trailColliderSpawn.rotation);
        lastTrailColliderSpawnPosition = trailColliderSpawn.position;
        trailObject.transform.SetParent(trailColliderSpawn);
        trailCollider = trailObject.GetComponent<TrailColliderController>();
        trailColliders.Add(trailCollider);
      }
    }
  }

  public void Fire (float fistSize) {
    damageBase = spellController.healthScript.damageFinal;
    sphereCollider.enabled = true;
    rigidbodyFist.isKinematic = false;
    rigidbodyFist.constraints = constraints;
    transform.localScale = Vector3.one * fistSize;
    trail.startWidth = fistSize;
    fired = true;
    if (spellController.trailUnlocked) {
      trail.emitting = true;
    }
    // trailIN.emitting = true;
    // trailOUT.emitting = true;

    rigidbodyFist.AddForce (spellController.body.transform.forward * launchForce, ForceMode.Impulse);
    velocityLast = rigidbodyFist.velocity;
    UpdateDamage();
  }

  void OnCollisionEnter (Collision collision) {
    hasCollided = true;
    lastCollision = collision;
  }

  public void UpdateDamage() {
    if (!fired)
    {
      damageBase = Mathf.RoundToInt(spellController.healthScript.damageFinal / 4f);
    }
    float damageTemp = damageBase;
    foreach (Tools.StatModifier damageBaseMultiplier in damageBaseMultipliers)
    {
      damageTemp *= damageBaseMultiplier.value;
    }
    foreach (Tools.StatModifier damageAddition in damageAdditions)
    {
      damageTemp += damageAddition.value;
    }
    foreach (Tools.StatModifier damageMultiplier in damageMultipliers)
    {
      damageTemp *= damageMultiplier.value;
    }
    damageFinal = Mathf.RoundToInt(damageTemp);
  }

  public void AddDamageBaseMultiplier(float value, string identifier) {
    if (Tools.AddStatModifier(damageBaseMultipliers, value, identifier)) {
      UpdateDamage();
    }
  }
  public void RemoveDamageBaseMultiplier(string identifier) {
    if (Tools.RemoveStatModifier(damageBaseMultipliers, identifier)) {
      UpdateDamage();
    }
  }
  public void AddDamageAddition(int value, string identifier) {
    if (Tools.AddStatModifier(damageAdditions, value, identifier)) {
      UpdateDamage();
    }
  }
  public void RemoveDamageAddition(string identifier) {
    if (Tools.RemoveStatModifier(damageAdditions, identifier)) {
      UpdateDamage();
    }
  }
  public void AddDamageMultiplier(float value, string identifier) {
    if (Tools.AddStatModifier(damageMultipliers, value, identifier)) {
      UpdateDamage();
    }
  }
  public void RemoveDamageMultiplier(string identifier) {
    if (Tools.RemoveStatModifier(damageMultipliers, identifier)) {
      UpdateDamage();
    }
  }

  public void SetFistMass(float newMass) {
    rigidbodyFist.mass = newMass;
  }
  public void SetLaunchForce(float newLaunchForce)
  {
    launchForce = newLaunchForce;
  }
}