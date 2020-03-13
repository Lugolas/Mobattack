using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Enemy : MonoBehaviour
{
  public Transform[] points;
  private int destPoint = -1;
  private NavMeshAgent agent;
  private Animator anim;


  void Start()
  {
    agent = GetComponent<NavMeshAgent>();
    anim = GetComponent<Animator>();

    // Disabling auto-braking allows for continuous movement
    // between points (ie, the agent doesn't slow down as it
    // approaches a destination point).
    agent.autoBraking = false;

    GotoNextPoint();
  }


  void GotoNextPoint()
  {
    // Returns if no points have been set up
    if (points.Length == 0)
      return;

    // Choose the next point in the array as the destination,
    // cycling to the start if necessary.
    var nextPoint = destPoint;
    while (nextPoint == destPoint)
    {
      nextPoint = Random.Range(0, points.Length);
    }

    destPoint = nextPoint;

    // Set the agent to go to the currently selected destination.
    agent.destination = points[destPoint].position;
  }


  void Update()
  {
    // Choose the next destination point when the agent gets
    // close to the current one.
    if (!agent.pathPending && agent.remainingDistance < 0.5f)
      GotoNextPoint();

    if (agent.remainingDistance > agent.stoppingDistance)
      anim.SetBool("IsRunning", true);
  }
}