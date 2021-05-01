using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimatorNavAgentRootMotion : MonoBehaviour
{
  NavMeshAgent navMeshAgent;
  Animator animator;
  public bool active = true;
  // Start is called before the first frame update
  void Start()
  {
    animator = GetComponent<Animator>();
    navMeshAgent = GetComponent<NavMeshAgent>();
  }

  void OnAnimatorMove()
  {
    if (active) {
      navMeshAgent.velocity = animator.deltaPosition / Time.deltaTime;
    }
  }
}
