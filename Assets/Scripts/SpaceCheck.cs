using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceCheck : MonoBehaviour
{
  List<Collider> obstacles = new List<Collider>();
  public bool enoughSpace = true;
  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
  }
  void OnTriggerEnter(Collider collider)
  {
    if (collider.tag == "Space")
    {
      enoughSpace = false;
      obstacles.Add(collider);
    }
  }

  void OnTriggerExit(Collider collider)
  {
    if (collider.tag == "Space")
    {
      obstacles.Remove(collider);
      if (obstacles.Count == 0)
      {
        enoughSpace = true;
      }
    }
  }
}
