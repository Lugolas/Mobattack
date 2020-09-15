using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerController : MonoBehaviour
{
  GameObject characterPrefab;
  float startTime;
  // Start is called before the first frame update
  void Start()
  {
    characterPrefab = Resources.Load<GameObject>("Prefabs/Characters/Enemy");

    startTime = Time.time;
  }

  // Update is called once per frame
  void Update()
  {
    if (Time.time > startTime + 1f)
    {
      startTime = Time.time;
      GameObject character = Instantiate(characterPrefab, transform.position, transform.rotation);
    }
  }
}
