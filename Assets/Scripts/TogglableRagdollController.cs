using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TogglableRagdollController : MonoBehaviour
{
  Transform[] childTransforms;
  List<Transform> childTransformsSorted = new List<Transform>();
  Transform[] ragdollArmChildTransforms;
  public GameObject ragdollArm;

  public bool synchronize = true;

  void Start()
  {
    childTransforms = GetComponentsInChildren<Transform>();
    foreach (Transform childTransform in childTransforms)
    {
      if(childTransform.tag != "NoRagdoll") {
        childTransformsSorted.Add(childTransform);
      }
    }
    ragdollArmChildTransforms = ragdollArm.GetComponentsInChildren<Transform>();
  }

  void LateUpdate()
  {
    if (synchronize) {
      for (int i = 0; i < ragdollArmChildTransforms.Length; i++)
      {
        childTransforms[i].position = ragdollArmChildTransforms[i].position;
        childTransforms[i].rotation = ragdollArmChildTransforms[i].rotation;
      }
    } else {
      for (int i = 0; i < ragdollArmChildTransforms.Length; i++)
      {
        ragdollArmChildTransforms[i].position = childTransforms[i].position;
        ragdollArmChildTransforms[i].rotation = childTransforms[i].rotation;
      }
    }
  }
}
