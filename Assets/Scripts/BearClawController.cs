using UnityEngine;

public class BearClawController : MonoBehaviour {
  public bool emitting = false;
  TrailRenderer[] trails;

  void Start()
  {
    trails = GetComponentsInChildren<TrailRenderer>();
  }

  void Update()
  {
    foreach (TrailRenderer trail in trails)
    {
      trail.emitting = emitting;
    }
  }
}