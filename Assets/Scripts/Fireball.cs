using UnityEngine;
using UnityEngine.Networking;

public class Fireball : MonoBehaviour
{
  public int damage = 25;
  public GameObject attacker;
  public Transform target;
  public GameObject FireballBurst;
  public bool useLifeTimeLimit = false;
  public float lifeTimeLimit = 15;
  public bool heals = false;

  public float rotateSpeed = 200.0f;
  public float movementSpeed = 5.0f;
  public ParticleSystem ps;
  public TrailRenderer trail;
  public SphereCollider sc;
  public CapsuleCollider cc;
  private Rigidbody rb;
  private float startTime;
  private Vector3 startPosition;
  private float startRotationZ;
  private float journeyLength;
  private bool hasHit = false;
  public float accelerationRate = -1;
  public MoneyManager characterWallet;
  bool initiatedSelfDestruction;
  public TurretController emitter;

  void Start()
  {
    startTime = Time.time;
    startPosition = transform.position;
    startRotationZ = transform.rotation.z;
    journeyLength = Vector3.Distance(startPosition, target.position);

    if (!ps)
      ps = GetComponentInChildren<ParticleSystem>();
    if (!trail)
      trail = GetComponentInChildren<TrailRenderer>();
    if (!sc)
      sc = GetComponent<SphereCollider>();
    if (!cc)
      cc = GetComponent<CapsuleCollider>();
    rb = GetComponent<Rigidbody>();

    if (useLifeTimeLimit)
    {
      Destroy(gameObject, lifeTimeLimit);
    }
    // Physics.IgnoreLayerCollision(9, 10);
  }
  void Update()
  {
    if (useLifeTimeLimit && lifeTimeLimit > 5 && Time.time >= (startTime + lifeTimeLimit - 5) && !initiatedSelfDestruction)
    {
      target = null;
      if (ps)
      {
        var em = ps.emission;
        em.enabled = false;
      }
      if (trail)
      {
        trail.emitting = false;
      }
      initiatedSelfDestruction = true;
    }
    if (target && !initiatedSelfDestruction)
    {
      HealthSimple healthDamage = target.GetComponentInParent<HealthSimple>();
      if (healthDamage && healthDamage.isDead)
      {
        if (emitter && emitter.target)
          target = emitter.target.transform;
        else
          target = null;
        // THIS IS FOR WHEN THE FIREBALL KILLED THE TARGET, NOT WHEN THE TARGET IS DEAD
        // target = null;
        // if (ps)
        // {
        //   var em = ps.emission;
        //   em.enabled = false;
        // }
        // if (trail)
        // {
        //   trail.emitting = false;
        // }
        // Destroy(gameObject, 5.1f);
        // initiatedSelfDestruction = true;
      }
    }

    if (!target && !initiatedSelfDestruction)
    {
      if (emitter && emitter.target)
        target = emitter.target.transform;
      else
        target = null;
    }

    // Distance moved equals elapsed time times speed..
    float distCovered = (Time.time - startTime) * movementSpeed;

    // Fraction of journey completed equals current distance divided by total distance.
    float fractionOfJourney = distCovered / journeyLength;

    // Set our position as a fraction of the distance between the markers.
    // transform.position = Vector3.Lerp(startPosition, target.position, fractionOfJourney);
    // transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, Mathf.Lerp(startRotationZ, 14400, fractionOfJourney));
    Vector3 rotateAmount = Vector3.zero;

    if (target != null)
    {
      Vector3 direction = target.position - rb.position;
      direction.y += 1;
      direction.Normalize();
      rotateAmount = Vector3.Cross(direction, transform.forward);
    }

    rb.angularVelocity = -rotateAmount * rotateSpeed;
    rb.velocity = transform.forward * movementSpeed;

    if (accelerationRate > 1)
    {
      movementSpeed *= accelerationRate;
      rotateSpeed *= accelerationRate;
    }
    // float newZRotation = Mathf.Lerp(0, 14400, 10000);
    // Debug.Log(newZRotation);
    // rb.rotation = Quaternion.Euler(rb.rotation.eulerAngles.x, rb.rotation.eulerAngles.y, newZRotation);
  }

  void OnTriggerEnter(Collider collider)
  {
    if (!hasHit && !initiatedSelfDestruction)
    {
      GameObject objectHit = Tools.FindObjectOrParentWithTag(collider.gameObject, "Character");
      GameObject playerCharacterHit = Tools.FindObjectOrParentWithTag(collider.gameObject, "PlayerCharacter");
      if (!playerCharacterHit && objectHit && objectHit != attacker)
      {
        if (emitter)
        {
          emitter.targetUpdateWanted = true;
        }
        if (ps)
        {
          var em = ps.emission;
          em.enabled = false;
        }
        if (trail)
        {
          trail.emitting = false;
        }
        target = null;
        if (sc)
          Destroy(sc);
        if (cc)
          Destroy(cc);
        hasHit = true;
        initiatedSelfDestruction = true;
        if (heals)
        {
          InflictsHealing(collider);
        }
        else
        {
          Tools.InflictDamage(collider.transform, damage, characterWallet, gameObject);
          if (emitter) {
            emitter.targetUpdateWanted = true;
          }
        }
        if (FireballBurst)
        {
          GameObject burst = Instantiate(FireballBurst, transform.position, transform.rotation) as GameObject;
          // GameObject burst = Instantiate(FireballBurst, collision.GetContact(0).point, transform.rotation) as GameObject;

          Destroy(burst, 2f);
          Destroy(gameObject, 5.5f);
        }
        else
        {
          Destroy(gameObject, 10f);
          MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
          foreach (MeshRenderer meshRenderer in meshRenderers)
          {
            meshRenderer.enabled = false;
          }
        }
      }
    }
  }

  void InflictsHealing(Collider collider)
  {
    HealthSimple hp = collider.gameObject.GetComponent<HealthSimple>();
    if (hp)
    {
      hp.ReceiveHealing(damage);
    }
  }
}