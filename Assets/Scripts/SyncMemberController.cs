using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncMemberController : MonoBehaviour
{
  public Transform ragdollMemberTransform;
  public bool RagdollOn = false;

  void LateUpdate()
  {
    if (RagdollOn) {
      transform.position = ragdollMemberTransform.position;
      transform.rotation = ragdollMemberTransform.rotation;
    } else {
      ragdollMemberTransform.position = transform.position;
      ragdollMemberTransform.rotation = transform.rotation;
    }
  }
}
