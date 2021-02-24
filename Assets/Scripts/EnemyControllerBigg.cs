using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyControllerBigg : EnemyController {
  public TogglableRagdollController leftArm;

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
}