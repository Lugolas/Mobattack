using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour
{
  Vector2 rotation = new Vector2(0, 0);
  public float speed = 3;
  public List<Light> LightsToIgnore;
  public GameObject playerToFollow;
  private Quaternion newRotation;
  private Quaternion newRotationOnX;
  private Vector3 relativePosition;

  void OnPreCull()
  {
    foreach (Light light in LightsToIgnore)
    {
      light.enabled = false;
    }
  }
  void OnPreRender()
  {
    foreach (Light light in LightsToIgnore)
    {
      light.enabled = false;
    }
  }

  void OnPostRender()
  {
    foreach (Light light in LightsToIgnore)
    {
      light.enabled = true;
    }
  }

  // Start is called before the first frame update
  void Start()
  {
  }

  // Update is called once per frame
  void Update()
  {
    float zAxis = Input.GetAxis("Horizontal");
    float xAxis = Input.GetAxis("Vertical");
    float zoom = Input.GetAxis("Mouse ScrollWheel");

    gameObject.transform.Translate(-xAxis, 0, zAxis, Space.World);
    gameObject.transform.Translate(0, 0, zoom * 15);


    // LOOK AT TARGET MAIS C'EST À APPLIQUER JUSTE PENDANT LE ZOOM EN FAIT PAS TOUT LE TEMPS DONC BON JE LE LAISSE LÀ ON SAIT JAMAIS L0L
    // relativePosition = new Vector3(playerToFollow.transform.position.x, playerToFollow.transform.position.y, transform.position.z) - transform.position;
    // newRotation = Quaternion.LookRotation(relativePosition);
    // transform.rotation = Quaternion.RotateTowards(transform.rotation, newRotation, Time.time * 1f);
    // transform.eulerAngles = new Vector3(transform.eulerAngles.x, -90, 0);
  }
}
