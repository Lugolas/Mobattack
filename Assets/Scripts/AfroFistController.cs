using UnityEngine;
using UnityEngine.Networking;

public class AfroFistController : MonoBehaviour
{
  public int id = -1;
  public int damage = 25;
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
  bool initiatedSelfDestruction;

  void Start()
  {
    startTime = Time.time;

    trail = GetComponent<TrailRenderer>();
    sphereCollider = GetComponent<SphereCollider>();
    meshRenderer = GetComponent<MeshRenderer>();
    rigidbodyFist = GetComponent<Rigidbody>();

    if (useLifeTimeLimit)
    {
      Destroy(gameObject, lifeTimeLimit);
    }
  }
  void Update()
  {
    if (useLifeTimeLimit && lifeTimeLimit > 5 && Time.time >= (startTime + lifeTimeLimit - 5) && !initiatedSelfDestruction)
    {
      if (trail)
      {
        trail.emitting = false;
      }
      initiatedSelfDestruction = true;
    }

    // rigidbody.velocity = transform.forward * movementSpeed;
  }

  void FixedUpdate()
  {
    damage = Mathf.RoundToInt(rigidbodyFist.velocity.magnitude);
  }

  public void Fire() {
    sphereCollider.enabled = true;
    rigidbodyFist.isKinematic = false;
    rigidbodyFist.constraints = constraints;
    transform.localScale = Vector3.one;

    rigidbodyFist.AddForce(transform.up * 10f, ForceMode.Impulse);
  }

  void OnCollisionEnter(Collision collision)
  {
    if (!hasHit && !initiatedSelfDestruction)
    {
      GameObject objectHit = Tools.FindObjectOrParentWithTag(collision.collider.gameObject, "EnemyCharacter");
      if (objectHit && objectHit != attacker)
      {
        if (trail)
        {
          trail.emitting = false;
        }
        sphereCollider.enabled = false;
        meshRenderer.enabled = false;
        hasHit = true;
        initiatedSelfDestruction = true;
        if (Tools.InflictDamage(collision.collider.transform, damage, characterWallet)) {
          spellController.speedUpSpell3();
        }
        if (fistBurstPrefab)
        {
          GameObject burst = Instantiate(fistBurstPrefab, transform.position, transform.rotation) as GameObject;
          // GameObject burst = Instantiate(fistBurstPrefab, collision.GetContact(0).point, transform.rotation) as GameObject;

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
}