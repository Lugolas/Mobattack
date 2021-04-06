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
    leftArm.ragdoll = false;
    rightArm.ragdoll = false;
    anim.SetInteger("Version", version);
    spawnAnimationName = "Skelett_Spawn" + version;
  }

  // Update is called once per frame
  void Update()
  {
    DestroyOnWaveChange();

    if (anim.GetCurrentAnimatorStateInfo(0).IsName(spawnAnimationName) && !hasSpawned) {
      SpawnAnimationProcess();
    }
    else if (spawning)
    {
      PostSpawnAnimationProcess();
      if (isRunner)
      {
        leftArm.ragdoll = false;
        rightArm.ragdoll = false;
      }
      else
      {
        leftArm.ragdoll = true;
        rightArm.ragdoll = true;
      }
    }

    if (HasTouchedObjective()) {
      TouchedObjectiveProcess();
    }

    if (health.isDead && !hasDied && hasSpawned)
    {
      DyingProcess();
      leftArm.ragdoll = false;
      leftArm.ragdollArm.SetActive(false);
      rightArm.ragdoll = false;
      rightArm.ragdollArm.SetActive(false);
    }
  }
}