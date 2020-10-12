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
  private ParticleSystem ps;
  private TrailRenderer trail;
  private SphereCollider sc;
  private Rigidbody rb;
  private float startTime;
  private Vector3 startPosition;
  private float startRotationZ;
  private float journeyLength;
  private bool hasHit = false;
  public float accelerationRate = -1;
  public MoneyManager characterWallet;
  bool initiatedSelfDestruction;
  public TurretCanonController emitter;

  void Start()
  {
    startTime = Time.time;
    startPosition = transform.position;
    startRotationZ = transform.rotation.z;
    journeyLength = Vector3.Distance(startPosition, target.position);

    ps = GetComponent<ParticleSystem>();
    trail = GetComponent<TrailRenderer>();
    sc = GetComponent<SphereCollider>();
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
      HealthSimple healthDamage = target.GetComponent<HealthSimple>();
      if (healthDamage && healthDamage.isDead)
      {
        if (emitter.target)
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
      if (emitter.target)
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
      if (objectHit && objectHit != attacker)
      {
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
        Destroy(sc);
        hasHit = true;
        initiatedSelfDestruction = true;
        if (heals)
        {
          InflictsHealing(collider);
        }
        else
        {
          Tools.InflictDamage(collider.transform, damage, characterWallet);
          emitter.targetUpdateWanted = true;
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
          Destroy(gameObject, 5.5f);
          for (int i = 0; i < transform.childCount; i++)
          {
            Destroy(transform.GetChild(i).gameObject);
          }
        }
      }
    }
  }

  void InflictsDamage(Collider collider)
  {
    HealthSimple hp = collider.gameObject.GetComponent<HealthSimple>();
    if (hp)
    {
      hp.TakeDamage(damage);
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