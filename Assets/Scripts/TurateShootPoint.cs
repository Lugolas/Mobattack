using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurateShootPoint : MonoBehaviour
{
  public GameObject target;
  public GameObject fireballPrefab;

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
  public void fire(GameObject newTarget)
  {
    // Debug.Log(gameObject.name + " shoot at " + target.name);
    // Debug.Log(fireballPrefabr.name);
    if (target != newTarget)
    {
      target = newTarget;
      targetChangeTime = Time.time;

      GameObject fireball = Instantiate(fireballPrefab, transform.position, transform.rotation) as GameObject;
      fireball.GetComponent<Fireball>().target = target.transform;
    }
  }
}
