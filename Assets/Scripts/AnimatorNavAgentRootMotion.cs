using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimatorNavAgentRootMotion : MonoBehaviour
{
  NavMeshAgent navMeshAgent;
  Animator animator;

  // Start is called before the first frame update
  void Start()
  {
    animator = GetComponent<Animator>();
    navMeshAgent = GetComponent<NavMeshAgent>();
  }

  void OnAnimatorMove()
  {
    navMeshAgent.velocity = animator.deltaPosition / Time.deltaTime;
  }
}
