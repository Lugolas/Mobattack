// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class TurretFalconController : TurretController
// {
//   public Transform projectileSpawnPoint;
//   public GameObject projectilePrefab;
//   private bool hasExtortedCharacter = false;
//   private float fireTime;
//   TurretStatManager statManager;
//   float oldDelay;
//   TurretPlayerLink playerLink;
//   public Animator animatorMain;
//   public Animator animatorGun;
//   public bool fired = false;
//   bool animatingAttack = false;
//   public Transform falconBody;
//   public FireMomentListener fireMoment;


//   private void Start()
//   {
//     playerLink = GetComponentInParent<TurretPlayerLink>();
//     statManager = GetComponentInParent<TurretStatManager>();

//     fireTime = Time.time;
//   }

//   void Update()
//   {
//     if (playerLink.activated && !hasExtortedCharacter)
//     {
//       playerLink.characterWallet.SubstractMoney(statManager.price);
//       hasExtortedCharacter = true;
//     }

//     TriggerFireAnimation();
//   }

//   void TriggerFireAnimation()
//   {
//     if (playerLink.activated && target && Time.time >= fireTime + statManager.delay)
//     {
//       fireTime = Time.time;
//       animatorMain.SetTrigger("Fire");
//       animatorGun.SetTrigger("Fire");
//     }
//   }

//   void LateUpdate()
//   {
//     if (targetUpdateWanted)
//     {
//       UpdateTarget(statManager.range);
//     }
//     if (target && playerLink.activated) {
//       falconBody.LookAt(target.transform.position);
//     }
//     if (fireMoment && fireMoment.timeToFire && !fired && target)
//     {
//       fired = true;
//       Fire();
//     }

//     if (animatorMain.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
//     {
//       animatingAttack = true;
//     }
//     else if (animatingAttack)
//     {
//       fired = false;
//       animatingAttack = false;
//       TriggerFireAnimation();
//     }
//   }

//   void Fire()
//   {
//     GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation) as GameObject;
//     Fireball fireballScript = projectile.GetComponent<Fireball>();
//     if (fireballScript)
//     {
//       fireballScript.damage = statManager.damage;
//       // fireballScript.emitter = this;
//       fireballScript.target = target.transform;
//       fireballScript.characterWallet = playerLink.characterWallet;
//     }
//   }
// }
