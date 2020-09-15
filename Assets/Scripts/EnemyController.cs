using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
  private Animator anim;
  NavMeshAgent navAgent;
  Vector3 objective;
  GameObject enemyManager;
  // Start is called before the first frame update
  void Start()
  {
    enemyManager = GameObject.Find("EnemyManager");
    transform.parent = enemyManager.transform;
    anim = GetComponent<Animator>();
    if (!anim)
    {
      anim = GetComponentInChildren<Animator>();
    }
    navAgent = GetComponent<NavMeshAgent>();
    GameObject objectiveObject = GameObject.Find("EnemyObjective");
    if (objectiveObject)
    {
      objective = objectiveObject.transform.position;
    }
    navAgent.destination = objective;
    navAgent.isStopped = false;
    anim.SetBool("IsRunning", true);
  }

  // Update is called once per frame
  void Update()
  {
    if (!navAgent.pathPending && !double.IsInfinity(navAgent.remainingDistance) && navAgent.remainingDistance <= navAgent.stoppingDistance)
    {
      anim.SetBool("IsRunning", false);
      Destroy(gameObject);
    }
  }

  public void refreshDestination()
  {
    if (navAgent.SetDestination(objective))
    {
      Debug.Log("oui");
    }
    else
    {
      Debug.Log("non --------------------------------------------------------");
    }
    navAgent.isStopped = false;
  }
}
