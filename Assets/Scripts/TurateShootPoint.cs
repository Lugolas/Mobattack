using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurateShootPoint : MonoBehaviour
{
  public GameObject target;
  public GameObject fireballPrefab;
  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }
  public void fire()
  {
    GameObject fireball = Instantiate(fireballPrefab, transform.position, transform.rotation) as GameObject;
    fireball.GetComponent<Fireball>().target = target.transform;
  }
}
