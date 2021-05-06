using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretAfroBallController : TurretController
{
  public int caneDamage = 20;
  public int damageInitial = 25;
  public int damageModifiedBase = 25;
  public int damageFinal = 25;
  public float baseDamageModifier = 2.5f;
  public Transform projectileSpawnPoint;
  public GameObject projectilePrefab;
  private bool hasExtortedCharacter = false;
  private float fireTime;
  TurretStatManager statManager;
  float oldDelay;
  TurretPlayerLink playerLink;
  public bool activated = false;
  Animator animator;
  public bool fireTrigger = false;
  public bool fireTriggerControl = true;
  FireMomentListener fireMomentListener;
  JumpMomentListener jumpMomentListener;
  CaneMomentListener caneMomentListener;
  bool animatingAttack = false;
  float attackAnimationLength;
  public float attackAnimationSpeed = 1f;
  string attackAnimationName = "AttackSpeed";
  TogglableRagdollController[] togglableRagdollControllers;
  List<Rigidbody> membersRigidbodies = new List<Rigidbody>();
  public bool launched = false;
  public GameObject spine;
  public GameObject hipL;
  public GameObject hipR;
  public Transform cane;
  public GameObject canePrefab;
  HealthSimple health;
  public RigidbodyConstraints constraints;
  public Rigidbody rigidbodyAfro;
  public SpellControllerAfro spellController;
  float launchTime;
  public float flyingDuration = 1;
  bool jumping = false;
  Vector3 startingPoint;
  Vector3 jumpingPoint;
  Vector3 startingPointRotation;
  Vector3 jumpingPointRotation;
  float jumpTime;
  float jumpDuration = 1.7f;
  float jumpSpeed = 10f;
  float journeyLength;
  bool jumpTrigger = false;
  bool jumpTriggerControl = true;
  bool animatingJump = true;
  bool ready = true;


  private void Start()
  {
    playerLink = GetComponentInParent<TurretPlayerLink>();
    statManager = GetComponentInParent<TurretStatManager>();
    animator = GetComponent<Animator>();
    health = GetComponentInChildren<HealthSimple>();
    togglableRagdollControllers = GetComponentsInChildren<TogglableRagdollController>();

    Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
    foreach (Rigidbody rigidbody in rigidbodies)
    {
      membersRigidbodies.Add(rigidbody);
    }

    if (!animator) {
      animator = GetComponentInChildren<Animator>();
    }

    fireMomentListener = GetComponentInChildren<FireMomentListener>();
    jumpMomentListener = GetComponentInChildren<JumpMomentListener>();

    AnimationClip[] animationClips = animator.runtimeAnimatorController.animationClips;
    foreach (AnimationClip animationClip in animationClips)
    {
      if (animationClip.name == "Attack")
      {
        attackAnimationLength = animationClip.length;
      }
    }
    attackAnimationSpeed = animator.GetFloat(attackAnimationName);

    fireTime = Time.time;
  }

  void Update()
  {
    if (playerLink.activated && activated == false) {
      activated = true;
      startingPoint = rigidbodyAfro.transform.position;
      startingPointRotation = rigidbodyAfro.transform.rotation.eulerAngles;
      rigidbodyAfro.GetComponent<Collider>().enabled = true;
    }

    if (statManager.delay != oldDelay)
    {
      oldDelay = statManager.delay;
      if (attackAnimationLength > statManager.delay)
      {
        attackAnimationSpeed = (attackAnimationLength / statManager.delay);
        animator.SetFloat(attackAnimationName, attackAnimationSpeed);
      }
    }

    if (playerLink && playerLink.activated && !hasExtortedCharacter)
    {
      playerLink.characterWallet.SubstractMoney(statManager.price);
      hasExtortedCharacter = true;
    }

    animator.SetBool("Jumping", jumping);

    TriggerFireAnimation();
  }

  void TriggerFireAnimation()
  {
    if (playerLink && playerLink.activated && enemiesInRange.Count > 0 && Time.time >= fireTime + statManager.delay)
    {
      fireTime = Time.time;
      animator.SetTrigger("Fire");
    }
  }

  void LateUpdate()
  {
    if (targetUpdateWanted)
    {
      UpdateTarget(statManager.range);
    }
    fireTrigger = fireMomentListener.timeToFire;
    if (fireTrigger && fireTriggerControl && enemiesInRange.Count > 0)
    {
      fireTrigger = false;
      fireTriggerControl = false;
      Fire();
    }

    if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
    {
      animatingAttack = true;
    }
    else if (animatingAttack)
    {
      fireTriggerControl = true;
      TriggerFireAnimation();
    }

    jumpTrigger = jumpMomentListener.timeToFire;
    if (jumpTrigger && jumpTriggerControl && launched)
    {
      launched = false;
      jumpTrigger = false;
      jumpTriggerControl = false;
      jumpTime = Time.time;
    }

    if (animator.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
    {
      animatingJump = true;
    }
    else if (animatingJump)
    {
      launched = false;
      health.isDead = false;
      animatingJump = false;
      jumping = false;
      jumpTriggerControl = true;
      ready = true;
      // fistDamage.outsideDamageModifier = 0;
    }

    animator.SetBool("Flying", launched);

    damageInitial = Mathf.RoundToInt(rigidbodyAfro.velocity.magnitude);
    damageModifiedBase = Mathf.RoundToInt(damageInitial + (damageInitial * baseDamageModifier));
    damageFinal = Mathf.RoundToInt(damageModifiedBase);

    // FAIRE PASSER SUR AFROFIST ¯\_(ツ)_/¯

    if (launched && Time.time > launchTime + flyingDuration) {
      JumpBack();
    }

    if (jumping && jumpTriggerControl == false) {
      jumpSpeed = Vector3.Distance(jumpingPoint, startingPoint) / 1.5f;
      float distCovered = (Time.time - jumpTime) * jumpSpeed;
      float fractionOfJourney = distCovered / journeyLength;
      rigidbodyAfro.transform.position = Vector3.Lerp(jumpingPoint, startingPoint, fractionOfJourney);
      rigidbodyAfro.transform.rotation = Quaternion.Euler(Vector3.Lerp(jumpingPointRotation, startingPointRotation, fractionOfJourney));
    }
  }

  public void Launch(GameObject attacker) {
    ready = false;
    launched = true;
    launchTime = Time.time;

    rigidbodyAfro.tag = "Fist";
    rigidbodyAfro.gameObject.layer = LayerMask.NameToLayer("BigFist");

    rigidbodyAfro.useGravity = true;
    rigidbodyAfro.isKinematic = false;

    GameObject currentCane = Instantiate(canePrefab, cane.position, cane.rotation);
    Rigidbody currentCaneBody = currentCane.GetComponent<Rigidbody>();

    rigidbodyAfro.constraints = constraints;
    currentCaneBody.AddForce(attacker.transform.forward * 15f, ForceMode.Impulse);
    rigidbodyAfro.AddForce(attacker.transform.forward * 250f, ForceMode.Impulse);
    // rigidbodyAfro.AddForce(transform.forward * 10f, ForceMode.Impulse);
  }

  void JumpBack() {
    jumping = true;

    rigidbodyAfro.Sleep();
    rigidbodyAfro.tag = "Building";
    rigidbodyAfro.gameObject.layer = LayerMask.NameToLayer("Default");
    rigidbodyAfro.rotation = Quaternion.Euler(0, rigidbodyAfro.rotation.eulerAngles.y, 0);

    rigidbodyAfro.useGravity = false;
    rigidbodyAfro.isKinematic = true;
    rigidbodyAfro.constraints = RigidbodyConstraints.None;

    jumpingPoint = rigidbodyAfro.transform.position;
    jumpingPointRotation = rigidbodyAfro.transform.rotation.eulerAngles;
    journeyLength = Vector3.Distance(jumpingPoint, startingPoint);
  }

  void Land() {
    launched = false;
    jumping = false;
    health.isDead = false;
  }

  void OnCollisionEnter(Collision collisionInfo)
  {
    Collision(collisionInfo.collider);
  }

  public void Collision(Collider collider) {
    if (launched) {
      GameObject enemyHit = Tools.FindObjectOrParentWithTag(collider.gameObject, "EnemyCharacter");
      if (enemyHit)
      {
        Tools.InflictDamage(collider.transform, damageFinal, playerLink.characterWallet, gameObject);
      }
      GameObject corpseHit = Tools.FindObjectOrParentWithTag(collider.gameObject, "Corpse");
      if (corpseHit)
      {
        corpseHit.GetComponent<Rigidbody>().AddForce(rigidbodyAfro.velocity * 5f, ForceMode.Impulse);
      }
    }
  }

  void Fire()
  {
    foreach (GameObject enemyInRange in enemiesInRange)
    {
      if (enemyInRange) {
        Tools.InflictDamage(enemyInRange.transform, caneDamage, playerLink.characterWallet, gameObject);
      }
      targetUpdateWanted = true;
    }
    Collider[] enemyParts = Physics.OverlapSphere(transform.position, statManager.range, Tools.GetEnemyPartsDetectionMask());

    foreach (Collider enemyPart in enemyParts)
    {
      GameObject corpseHit = Tools.FindObjectOrParentWithTag(enemyPart.gameObject, "Corpse");
      if (corpseHit)
      {
        corpseHit.GetComponent<Rigidbody>().AddExplosionForce(caneDamage, transform.position, statManager.range, 0, ForceMode.Impulse);
      }
    }
  }
}
