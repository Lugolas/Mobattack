using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurateShootController : MonoBehaviour
{
  TurateShootPoint[] shootPoints;
  public GameObject fireballPrefab;
  public GameObject target;
  private List<GameObject> alreadyShotAt = new List<GameObject>();

  // Start is called before the first frame update
  void Start()
  {
    shootPoints = GetComponentsInChildren<TurateShootPoint>();
  }

  // Update is called once per frame
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.P))
    {
    }
  }
  void OnTriggerEnter(Collider collider)
  {
    if (Tools.FindObjectOrParentWithTag(collider.gameObject, "Character"))
    {
      Debug.Log("Hey");
      if (!alreadyShotAt.Contains(collider.gameObject))
      {
        Debug.Log("Shoot");
        target = collider.gameObject;
        alreadyShotAt.Add(target);

        foreach (TurateShootPoint shootPoint in shootPoints)
        {
          // shootPoint.target = target;
          shootPoint.fire(target);
        }
      }
    }
  }

  void OnTriggerExit(Collider collider)
  {
    if (Tools.FindObjectOrParentWithTag(collider.gameObject, "Character"))
    {
      alreadyShotAt.Remove(collider.gameObject);
    }
  }
}
