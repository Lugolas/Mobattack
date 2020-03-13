using UnityEngine;
using UnityEngine.Networking;

public class Fireball : MonoBehaviour
{
  public int damage = 25;
  public string attackerName;
  public Transform target;
  public GameObject FireballBurst;

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

    // Physics.IgnoreLayerCollision(9, 10);
  }
  void Update()
  {
    // Distance moved equals elapsed time times speed..
    float distCovered = (Time.time - startTime) * movementSpeed;

    // Fraction of journey completed equals current distance divided by total distance.
    float fractionOfJourney = distCovered / journeyLength;

    // Set our position as a fraction of the distance between the markers.
    // transform.position = Vector3.Lerp(startPosition, target.position, fractionOfJourney);
    // transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, Mathf.Lerp(startRotationZ, 14400, fractionOfJourney));

    Vector3 direction = target.position - rb.position;

    direction.y += 1;

    direction.Normalize();

    Vector3 rotateAmount = Vector3.Cross(direction, transform.forward);

    rb.angularVelocity = -rotateAmount * rotateSpeed;

    rb.velocity = transform.forward * movementSpeed;
    // float newZRotation = Mathf.Lerp(0, 14400, 10000);
    // Debug.Log(newZRotation);
    // rb.rotation = Quaternion.Euler(rb.rotation.eulerAngles.x, rb.rotation.eulerAngles.y, newZRotation);
  }

  void OnCollisionEnter(Collision collision)
  {
    if (!hasHit)
    {
      GameObject objectHit = Tools.FindObjectOrParentWithTag(collision.gameObject, "Character");
      if (objectHit && objectHit.name != attackerName)
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

        Destroy(sc);
        hasHit = true;
        InflictsDamage(collision);
        if (FireballBurst)
        {
          GameObject burst = Instantiate(FireballBurst, transform.position, transform.rotation) as GameObject;
          // GameObject burst = Instantiate(FireballBurst, collision.GetContact(0).point, transform.rotation) as GameObject;

          Destroy(burst, 2f);
          Destroy(gameObject, 2.1f);
        }
        else
        {
          Destroy(gameObject, 5);
          for (int i = 0; i < transform.childCount; i++)
          {
            Destroy(transform.GetChild(i).gameObject);
          }
        }
      }
    }
  }

  void InflictsDamage(Collision collision)
  {
    HealthDamage hp = collision.gameObject.GetComponent<HealthDamage>();
    if (hp)
    {
      hp.TakeDamage(damage);
    }
  }
}