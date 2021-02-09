using UnityEngine;

public class BearClawsController : MonoBehaviour {
  public BearClawLMomentListener bearClawLMoment;
  public BearClawRMomentListener bearClawRMoment;
  public BearClawController bearClawL;
  public BearClawController bearClawR;

  void Update()
  {
    bearClawL.emitting = bearClawLMoment.timeToFire;
    bearClawR.emitting = bearClawRMoment.timeToFire;
  }
}