using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
  public GameObject PlayerPing = null;
  private Animator anim;
  public Camera minimapCam;

  // Start is called before the first frame update
  void Start()
  {
    anim = GetComponent<Animator>();
    // gameObject.tag = "Player";
    Physics.IgnoreLayerCollision(11, 0, true);
    Physics.IgnoreLayerCollision(11, 8, true);
    Physics.IgnoreLayerCollision(11, 9, true);
    Physics.IgnoreLayerCollision(11, 11, true);
    Physics.IgnoreLayerCollision(11, 12, true);
    Physics.IgnoreLayerCollision(11, 13, true);
    Physics.IgnoreLayerCollision(9, 16, true);
  }

  // Update is called once per frame
  void Update()
  {

  }
}