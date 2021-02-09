using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyControllerBasic : EnemyController {
  int version = -1;
  public GameObject thigh;
  public GameObject shin;
  public GameObject foot;
  public TogglableRagdollController leftArm;
  public TogglableRagdollController rightArm;

  void Start()
  {
    Init();
    version = Random.Range(1, 5);
    switch (version)
    {
      case 2:
        foot.SetActive(false);
        break;
      case 3:
        foot.SetActive(false);
        shin.SetActive(false);
        break;
      case 4:
        foot.SetActive(false);
        shin.SetActive(false);
        thigh.SetActive(false);
        break;
      default:
        break;
    }
    leftArm.synchronize = false;
    rightArm.synchronize = false;
    anim.SetInteger("Version", version);
    spawnAnimationName = "Skelett_Spawn" + version;
  }

  // Update is called once per frame
  void Update()
  {
    if (anim.GetCurrentAnimatorStateInfo(0).IsName(spawnAnimationName) && !hasSpawned) {
      SpawnAnimationProcess();
    }
    else if (spawning)
    {
      PostSpawnAnimationProcess();
      if (isRunner)
      {
        leftArm.synchronize = false;
        rightArm.synchronize = false;
      }
      else
      {
        leftArm.synchronize = true;
        rightArm.synchronize = true;
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
      rightArm.synchronize = false;
      rightArm.ragdollArm.SetActive(false);
    }
  }
}