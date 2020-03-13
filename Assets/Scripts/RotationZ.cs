using UnityEngine;

public class RotationZ : MonoBehaviour
{
  public float rotationSpeed = 2.5f;
  private float t = 0.0f;
  void Update()
  {
    if (t >= 1)
    {
      t = 0.0f;
    }
    else
    {
      transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, Mathf.Lerp(0, 360 * rotationSpeed, t));
      t += Time.deltaTime;
    }
  }
}