using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AfroFistController : MonoBehaviour {
  public int id = -1;
  public int damageBase = 0;
  List<Tools.StatModifier> damageBaseMultipliers = new List<Tools.StatModifier> ();
  List<Tools.StatModifier> damageAdditions = new List<Tools.StatModifier> ();
  List<Tools.StatModifier> damageMultipliers = new List<Tools.StatModifier> ();
  public int damageFinal;
  int damageFinalControl;
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
  public List<HealthSimple> trailTargetList = new List<HealthSimple> ();
  List<TrailColliderController> trailColliders = new List<TrailColliderController> ();
  float lastHit = 0;
  float hitDelay = 0.25f;
  float launchForce = 100;
  int enemyBounce = 0;
  int enemyBounceMax = 0;
  List<EnemyController> enemiesHit = new List<EnemyController> ();
  public List<ParticleSystem> particleSystemsMove = new List<ParticleSystem> ();
  public List<GameObject> objectsToDisable = new List<GameObject> ();
  public GameObject shock;
  public GameObject explosionBig;
  public GameObject explosion;
  bool firstContact = false;
  EnemyController lastEnemyHit;
  public bool canDivide = true;
  public int division = 0;
  public bool initiated = false;

  void Start () {
    Init();
  }

  public void Init() {
    if (!initiated) {
      initiated = true;
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
      outlineColor = Color.HSVToRGB (50, 0, 0);
      sprites = GetComponentsInChildren<SpriteRenderer> ();
      trailColliderRadius = trailColliderPrefab.GetComponent<CapsuleCollider> ().radius;

      if (useLifeTimeLimit) {
        Destroy (gameObject, lifeTimeLimit);
      }
      UpdateDamage (division);

      foreach (ParticleSystem particleSystemMove in particleSystemsMove) {
        particleSystemMove.gameObject.SetActive (false);
      }
    }
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

    if (fired && Vector3.Distance (transform.position, Vector3.zero) > 100) {
      Destroy (gameObject);
    }
  }

  void FixedUpdate () {
    if(fired)
    if (spellController.speedAffectsFists) {
      // magnitude = 0;
      if (Time.time > lastMagnitudeChange + magnitudeChangeDelay) {
        magnitude = newMagnitude;
        lastMagnitude = newMagnitude;
        newMagnitude = (transform.position - lastPosition).magnitude / (Time.time - lastpositionTime);
        lastMagnitudeChange = Time.time;
      } else {
        magnitudeTransitionPoint = (Time.time - lastMagnitudeChange) / magnitudeChangeDelay;
        magnitude = Mathf.Lerp (lastMagnitude, newMagnitude, magnitudeTransitionPoint);
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

      color = Color.HSVToRGB (0f / 360f, 1, colorMagnitude);
      // trailColor = Color.HSVToRGB(0f/360f, colorMagnitude, 1);
      color = new Color (color.r * colorMultiplier, color.g * colorMultiplier, color.b * colorMultiplier, color.a);
      trailColor = new Color (color.r * 1000, color.g * 1000, color.b * 1000, color.a);
      outlineColor = Color.HSVToRGB (50f / 360f, 1, colorMagnitude);
      outlineColor = new Color (outlineColor.r * colorMultiplier, outlineColor.g * colorMultiplier, outlineColor.b * colorMultiplier, outlineColor.a);
      material.SetColor ("_Color", color);
      trail.material.SetColor ("_Color", trailColor);
      // trailIN.material.SetColor("_BaseColor", trailColor);
      material.SetColor ("_OutlineColor", outlineColor);
      trail.material.SetColor ("_OutlineColor", outlineColor);
      // trailOUT.material.SetColor("_BaseColor", outlineColor);
      AddDamageBaseMultiplier (1 + (magnitude / 20), "speed");
    }

    velocity = new Vector3 (rigidbodyFist.velocity.x, 0, rigidbodyFist.velocity.z);
    // velocity = rigidbodyFist.velocity;
    if (fired && velocity != velocityLast) {
      velocityLast = velocity;
      OnVelocityChange ();
    }
    if (!fired && spellController.IsInBreakerUlt ()) {
      SpawnTrailCollider ();
    }
    if (velocity != Vector3.zero) {
      transform.rotation = Quaternion.LookRotation (velocity);
    }

    if (Time.time > lastHit + hitDelay) {
      TrailTargetListPrep ();
      lastHit = Time.time;
      TrailTargetListHit ();
    }
  }

  void LateUpdate () {
    if (hasCollided && !hasHit && !initiatedSelfDestruction) {
      hasCollided = false;
      EnemyController enemyHit = lastCollision.collider.GetComponent<EnemyController> ();
      if (!enemyHit) {
        enemyHit = lastCollision.collider.GetComponentInParent<EnemyController> ();
        if (!enemyHit) {
          enemyHit = lastCollision.collider.GetComponentInChildren<EnemyController> ();
        }
      }
      // GameObject objectHit = Tools.FindObjectOrParentWithTag (lastCollision.collider.gameObject, "EnemyCharacter");
      // if (objectHit && objectHit != attacker) {
      if (enemyHit && !enemyHit.GetIsDead ()) {
        HitEnemy (enemyHit);
        if (enemyBounce < enemyBounceMax) {
          enemyBounce++;
        } else {
          DestroySelf ();
        }
      } else {
        if (firstContact) {
          InstantiateImpact(false);
        } else {
          firstContact = true;
        }
      }
    }
    if (fired && damageFinal != damageFinalControl) {
      damageFinalControl = damageFinal;
      UpdateSize();
    }
  }

  public void InstantiateImpact(bool enemy, bool useLastCollision = true) {
    Vector3 position;
    EnemyController enemyToAvoid = null;
    if (useLastCollision)
    {
      position = lastCollision.GetContact(0).point;
      enemyToAvoid = lastEnemyHit;
    } else {
      position = transform.position;
    }

    GameObject impact;
    if (enemy) {
      if (spellController.fistExplode) {
        impact = explosionBig;
        ExplosionController explosionBigController = impact.GetComponent<ExplosionController>();
        explosionBigController.damage = Mathf.RoundToInt((float)damageFinal / 2);
        explosionBigController.radius = (transform.localScale.x / 2) + 5;
        // explosionBigController.radius = 20;
        explosionBigController.enemyToAvoid = enemyToAvoid;
        explosionBigController.moneyManager = characterWallet;
        explosionBigController.spellController = spellController;
      } else {
        impact = explosion;
      }
    } else {
      impact = shock;
    }


    GameObject impactObject = Instantiate (impact, position, transform.rotation);
    Destroy (impactObject, 1.5f);
  }
  public void HitEnemy (EnemyController enemyHit, bool useLastCollision = true) {
    lastEnemyHit = enemyHit;
    InstantiateImpact(true, useLastCollision);
    if (Tools.InflictDamage (enemyHit.transform, damageFinal, characterWallet, spellController)) {
      spellController.FistKilledEnemy ();
    }
    if (spellController.fistEnemyBouncesQuest && enemiesHit.Contains (enemyHit)) {
      spellController.GenerateQuestDing (transform.position);
      spellController.enemyHitTwiceBySameFist++;
    }
    enemiesHit.Add (enemyHit);
  }
  void DestroySelf () {
    if (spellController.fistDivide) {
      Divide();
    }

    if (trail) {
      // if (trailIN && trailOUT) {
      trail.emitting = false;
      // trailIN.emitting = false;
      // trailOUT.emitting = false;
    }
    foreach (SpriteRenderer sprite in sprites) {
      sprite.enabled = false;
    }
    foreach (GameObject objectToDisable in objectsToDisable) {
      objectToDisable.SetActive (false);
    }
    foreach (ParticleSystem particleSystemMove in particleSystemsMove) {
      particleSystemMove.Stop ();
    }
    sphereCollider.enabled = false;
    meshRenderer.enabled = false;
    hasHit = true;
    initiatedSelfDestruction = true;

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

  void TrailTargetListPrep () {
    for (int i = trailColliders.Count - 1; i > -1; i--) {
      if (trailColliders[i]) {
        List<HealthSimple> targetList = trailColliders[i].targetList;
        for (int j = targetList.Count - 1; j > -1; j--) {
          if (targetList[j]) {
            if (!trailTargetList.Contains (targetList[j])) {
              trailTargetList.Add (targetList[j]);
            }
          } else {
            targetList.RemoveAt (j);
          }
        }
      } else {
        trailColliders.RemoveAt (i);
      }
    }
  }

  void TrailTargetListHit () {
    for (int i = trailTargetList.Count - 1; i > -1; i--) {
      if (trailTargetList[i]) {
        Tools.InflictDamage (
          trailTargetList[i].transform,
          Mathf.RoundToInt (Mathf.RoundToInt (damageFinal / 2f) * hitDelay),
          characterWallet,
          spellController
        );
      } else {
        trailTargetList.RemoveAt (i);
      }
    }
  }

  void OnVelocityChange () {
    velocityChanged = true;
    if (trailCollider) {
      trailCollider.Detach ();
    }
    // ParticleSystem shockObject = Instantiate (shock, lastCollision.GetContact(0).point, transform.rotation);
    // Destroy (shockObject, 1.5f);
    SpawnTrailCollider ();
  }

  void SpawnTrailCollider () {
    if (trail.emitting && !initiatedSelfDestruction && spellController.trailUnlocked) {
      if (Vector3.Distance (trailColliderSpawn.position, lastTrailColliderSpawnPosition) >= trailColliderRadius * 2) {
        GameObject trailObject = Instantiate (trailColliderPrefab, trailColliderSpawn.position, trailColliderSpawn.rotation);
        lastTrailColliderSpawnPosition = trailColliderSpawn.position;
        trailObject.transform.SetParent (trailColliderSpawn);
        trailCollider = trailObject.GetComponent<TrailColliderController> ();
        trailColliders.Add (trailCollider);
      }
    }
  }

  public void Fire(float fistSize, bool ownDirection = false) {
    // foreach (ParticleSystem particleSystemMove in particleSystemsMove)
    // {
    //   particleSystemMove.gameObject.SetActive(true);
    // }
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
    Vector3 forceDirection;
    if (ownDirection) {
      forceDirection = transform.forward;
      // transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + 90, transform.rotation.eulerAngles.z));
      // Debug.Log(transform.forward);
    } else {
      forceDirection = spellController.body.transform.forward;
    }

    rigidbodyFist.AddForce(forceDirection * launchForce, ForceMode.Impulse);
    velocityLast = rigidbodyFist.velocity;
    UpdateDamage (division);
  }

  void OnCollisionEnter (Collision collision) {
    hasCollided = true;
    lastCollision = collision;
  }

  public void UpdateDamage (int division = 0) {
    if (!fired) {
      damageBase = Mathf.RoundToInt(spellController.healthScript.damageFinal / 4f);
    }
    float ratio = division * 2;
    float damageTemp = damageBase;
    if (ratio > 0) {
      damageTemp = Mathf.RoundToInt((float) damageBase / ratio);
    } else {
      damageTemp = damageBase;
    }
    foreach (Tools.StatModifier damageBaseMultiplier in damageBaseMultipliers) {
      damageTemp *= damageBaseMultiplier.value;
    }
    foreach (Tools.StatModifier damageAddition in damageAdditions) {
      damageTemp += damageAddition.value;
    }
    foreach (Tools.StatModifier damageMultiplier in damageMultipliers) {
      damageTemp *= damageMultiplier.value;
    }
    damageFinal = Mathf.RoundToInt (damageTemp);
  }

  public void AddDamageBaseMultiplier (float value, string identifier) {
    if (Tools.AddStatModifier (damageBaseMultipliers, value, identifier)) {
      UpdateDamage (division);
    }
  }
  public void RemoveDamageBaseMultiplier (string identifier) {
    if (Tools.RemoveStatModifier (damageBaseMultipliers, identifier)) {
      UpdateDamage (division);
    }
  }
  public void AddDamageAddition (int value, string identifier = null) {
    if (Tools.AddStatModifier (damageAdditions, value, identifier)) {
      UpdateDamage (division);
    }
  }
  public void RemoveDamageAddition (string identifier) {
    if (Tools.RemoveStatModifier (damageAdditions, identifier)) {
      UpdateDamage (division);
    }
  }
  public void AddDamageMultiplier (float value, string identifier) {
    if (Tools.AddStatModifier (damageMultipliers, value, identifier)) {
      UpdateDamage (division);
    }
  }
  public void RemoveDamageMultiplier (string identifier) {
    if (Tools.RemoveStatModifier (damageMultipliers, identifier)) {
      UpdateDamage (division);
    }
  }

  public void SetFistMass (float newMass) {
    rigidbodyFist.mass = newMass;
  }
  public void SetLaunchForce (float newLaunchForce) {
    launchForce = newLaunchForce;
  }
  public void SetEnemyBounceMax (int newEnemyBounceMax) {
    enemyBounceMax = newEnemyBounceMax;
  }
  public float GetFistMass () {
    return rigidbodyFist.mass;
  }
  public float GetLaunchForce () {
    return launchForce;
  }
  public int GetEnemyBounceMax () {
    return enemyBounceMax;
  }

  public SphereCollider GetSphereCollider() {
    return sphereCollider;
  }
  public void SetSphereCollider(SphereCollider collider) {
    sphereCollider = collider;
  }

  void FireCopy(bool left) {
    float angle = 180;
    float xPos;
    GameObject fistCopy = Instantiate(gameObject, transform.position, transform.rotation);
    if (left) {
      angle += 45/2;
      xPos = -fistCopy.transform.localScale.x;
    } else {
      angle -= 45/2;
      xPos = fistCopy.transform.localScale.x;
    }
    // fistCopy.transform.position = new Vector3(
    //   fistCopy.transform.position.x + xPos, 
    //   fistCopy.transform.position.y, 
    //   fistCopy.transform.position.z
    // );
    fistCopy.transform.rotation = Quaternion.Euler(new Vector3(
      transform.rotation.eulerAngles.x, 
      transform.rotation.eulerAngles.y + angle, 
      transform.rotation.eulerAngles.z
    ));
    fistCopy.transform.localScale /= 2;
    AfroFistController fistCopyController = fistCopy.GetComponent<AfroFistController>();
    fistCopyController.Init();
    if (!fistCopyController.GetSphereCollider()) {
      fistCopyController.SetSphereCollider(fistCopyController.GetComponent<SphereCollider>());
    }
    fistCopyController.division = division + 1;
    fistCopyController.canDivide = false;
    fistCopyController.damageBase /= 2;
    fistCopyController.SetFistMass(fistCopyController.GetFistMass() / 2);
    fistCopyController.SetLaunchForce(fistCopyController.GetLaunchForce() / 2);
    fistCopyController.initiated = false;
    fistCopyController.rigidbodyFist.Sleep();
    fistCopyController.Fire(transform.localScale.x / 2, true);
  }

  void Divide() {
    if (canDivide) {
      FireCopy(true);
      FireCopy(false);
    }
  }

  public void UpdateSize(bool checkSelf = true) {
    float augmentation = (float)damageFinal / spellController.healthScript.damageBase;
    if (checkSelf) {
      augmentation = (float)damageFinal / spellController.healthScript.damageBase;
    } else {
      augmentation = (float)spellController.healthScript.damageFinal / spellController.healthScript.damageBase;
    }
    float augmentationHalved = ((augmentation - 1) / 2) + 1;
    float size = augmentationHalved;
    float radius = size / 2;
    transform.localScale = new Vector3(size, size, size);
    trail.startWidth = size;

    float weight = augmentationHalved * spellController.fistWeightInitial;
    SetFistMass(weight);
  }
  void UpdateLaunchForce() {
    SetLaunchForce(spellController.fistLaunchForce);
  }
  public void UpdateFist() {
    UpdateLaunchForce();
    UpdateBounces();
  }
  void UpdateBounces() {
    SetEnemyBounceMax(spellController.fistEnemyBounces);
  }
}