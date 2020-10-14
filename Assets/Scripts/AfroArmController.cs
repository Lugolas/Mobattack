using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfroArmController : MonoBehaviour
{
List<Vector3> localPositions = new List<Vector3>();
Rigidbody[] childRigidbodies;

  // Start is called before the first frame update
  void Start()
  {
    childRigidbodies = GetComponentsInChildren<Rigidbody>();
    for (int i = 0; i < childRigidbodies.Length; i++)
    {
      localPositions.Add(childRigidbodies[i].transform.localPosition);
    }
  }

    // Update is called once per frame
  void LateUpdate()
  {
    for (int i = 1; i < childRigidbodies.Length; i++)
    {
      childRigidbodies[i].transform.localPosition = localPositions[i];
    }
  }
}
