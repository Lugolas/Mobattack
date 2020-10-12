using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
  private Animator anim;
  NavMeshAgent navAgent;
  HealthSimple health;
  HealthSimple objectiveHealth;
  GameObject objectiveObject;
  Vector3 objective;
  GameObject enemyManager;
  // Start is called before the first frame update
  void Start()
  {
    GameObject objectiveObject = GameObject.Find("EnemyObjective");

    if (objectiveObject)
    {
      objective = objectiveObject.transform.position;
      objectiveHealth = objectiveObject.GetComponent<HealthSimple>();
    }

    health = GetComponent<HealthSimple>();
    enemyManager = GameObject.Find("EnemiesManager");
    transform.parent = enemyManager.transform;
    anim = GetComponent<Animator>();
    if (!anim)
    {
      anim = GetComponentInChildren<Animator>();
    }
    navAgent = GetComponent<NavMeshAgent>();

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

      objectiveHealth.TakeDamage(health.currentHealth);
    }
  }

  public void refreshDestination()
  {
    navAgent.SetDestination(objective);

    navAgent.isStopped = false;
  }
}
