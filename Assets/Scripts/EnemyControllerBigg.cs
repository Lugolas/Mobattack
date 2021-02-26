using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyControllerBigg : EnemyController {
  public int damageHit;
  public int damageHit2;
  public int damageSlam;
  public float delay;
  float lastAttack = 0;
  public TogglableRagdollController leftArm;
  public TriggerCheck visionTrigger;
  public FireMomentListener listenerHit;
  public FireMomentListener listenerHit2;
  public FireMomentListener listenerSlam;
  public Transform hitPoint;
  public Transform hit2Point;
  public Transform slamPoint;

  bool triggeredAttack = false;
  bool triggeredHit = false;
  bool triggeredHit2 = false;
  bool triggeredSlam = false;
  bool attacking = false;
  List<GameObject> listHit = new List<GameObject>();
  List<GameObject> listHit2 = new List<GameObject>();
  List<GameObject> listSlam = new List<GameObject>();
  public LayerMask layerMaskCharacters;

  void Start()
  {
    Init();
    leftArm.synchronize = false;
  }

  // Update is called once per frame
  void Update()
  {
    DestroyOnWaveChange();

    if (anim.GetCurrentAnimatorStateInfo(0).IsName("Spawn") && !hasSpawned) {
      SpawnAnimationProcess();
    }
    else if (spawning)
    {
      PostSpawnAnimationProcess();
      leftArm.synchronize = true;
    }

    if (visionTrigger.triggered && !triggeredAttack && Time.time >= lastAttack + delay) {
      anim.SetTrigger("Attack");
      triggeredAttack = true;
      lastAttack = Time.time;
    }

    if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack")) {
      attacking = true;
    } else if (attacking == true) {
      attacking = false;
      listHit.Clear();
      listHit2.Clear();
      listSlam.Clear();
      triggeredAttack = false;
      triggeredHit = false;
      triggeredHit2 = false;
      triggeredSlam = false;
    }

    if (triggeredAttack && listenerHit.timeToFire && !triggeredHit) {
      triggeredHit = true;
      Collider[] colliders = Physics.OverlapBox(
        hitPoint.position,
        new Vector3(5, 5, 5), body.rotation, layerMaskCharacters, QueryTriggerInteraction.Collide
      );
      if (colliders.Length > 0) {
        foreach (Collider collider in colliders)
        {
          GameObject character = Tools.FindObjectOrParentWithTag(collider.gameObject, "PlayerCharacter");
          if (!listHit.Contains(character)) {
            listHit.Add(character);
            SpellController spellController = character.GetComponent<SpellController>();
            if (spellController) {
              Tools.InflictDamage(spellController.body.transform, damageHit, null, gameObject);
            }
          }
        }
      }
    } 

    if (triggeredAttack && listenerHit2.timeToFire && !triggeredHit2) {
      triggeredHit2 = true;
      Collider[] colliders = Physics.OverlapBox(
        hit2Point.position,
        new Vector3(3.75f, 3.75f, 3.75f), body.rotation, layerMaskCharacters, QueryTriggerInteraction.Collide
      );
      if (colliders.Length > 0) {
        foreach (Collider collider in colliders)
        {
          GameObject character = Tools.FindObjectOrParentWithTag(collider.gameObject, "PlayerCharacter");
          if (!listHit2.Contains(character)) {
            listHit2.Add(character);
            SpellController spellController = character.GetComponent<SpellController>();
            if (spellController) {
              Tools.InflictDamage(spellController.body.transform, damageHit2, null, gameObject);
            }
          }
        }
      }
    } 

    if (triggeredAttack && listenerSlam.timeToFire && !triggeredSlam) {
      triggeredSlam = true;
      Collider[] colliders = Physics.OverlapSphere(
        slamPoint.position, 10, layerMaskCharacters, QueryTriggerInteraction.Collide
      );
      if (colliders.Length > 0) {
        foreach (Collider collider in colliders)
        {
          GameObject character = Tools.FindObjectOrParentWithTag(collider.gameObject, "PlayerCharacter");
          if (!listSlam.Contains(character)) {
            listSlam.Add(character);
            SpellController spellController = character.GetComponent<SpellController>();
            if (spellController) {
              Tools.InflictDamage(spellController.body.transform, damageSlam, null, gameObject);
            }
          }
        }
      }
    } 



    if (HasTouchedObjective()) {
      TouchedObjectiveProcess();
    }

    if (health.isDead && !hasDied && hasSpawned)
    {
      DyingProcess();
      leftArm.synchronize = false;
      leftArm.ragdollArm.SetActive(false);
    }
  }

  // void OnDrawGizmos()
  // {
  //   Gizmos.color = Color.red;
  //   if (triggeredAttack)
  //     Gizmos.DrawWireCube(
  //       hit2Point.position, 
  //       new Vector3(10, 10, 10)
  //     );
  // }
}