using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurateShootPoint : MonoBehaviour
{
  public GameObject target;
  public GameObject fireballPrefabHurt;
  public GameObject fireballPrefabHeal;
  bool HURT = false;
  bool HEAL = true;

  // Let's have a 0.5s timer or something
  float targetChangeTime;
  float delay = 0.5f;

  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    if (Time.time >= targetChangeTime + delay)
    {
      target = null;
    }
  }
  public void fire(GameObject newTarget, bool fireballBehaviour)
  {
    if (target != newTarget)
    {
      target = newTarget;
      targetChangeTime = Time.time;

      GameObject fireballPrefab;
      if (fireballBehaviour == HEAL)
      {
        fireballPrefab = fireballPrefabHeal;
      }
      else
      {
        fireballPrefab = fireballPrefabHurt;
      }

      GameObject fireball = Instantiate(fireballPrefab, transform.position, transform.rotation) as GameObject;
      fireball.GetComponent<Fireball>().target = target.transform;
    }
  }
}
