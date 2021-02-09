using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpellController : MonoBehaviour
{
  public GameObject body;
  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }

  virtual public void Spell1()
  {
    Debug.Log("Default Spell1 Behaviour");
  }
  virtual public void Spell2()
  {
    Debug.Log("Default Spell2 Behaviour");
  }
  virtual public void Spell3()
  {
    Debug.Log("Default Spell3 Behaviour");
  }

  virtual public bool Fire1(bool down)
  {
    Debug.Log("Default Fire1 Behaviour");
    return true;
  }
  virtual public void Fire2(bool down)
  {
    Debug.Log("Default Fire2 Behaviour");
  }
}
