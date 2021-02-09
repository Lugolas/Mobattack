using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfroHandController : MonoBehaviour
{
  public GameObject afroParent;
  public GameObject fistPrefab;
  public bool fire = false;
  public SphereCollider sphereCollider;
  public MoneyManager characterWallet;
  bool fireFired = false;
  AfroFistController fist;
  SpellControllerAfro spellController;
  public bool punchAttempted = false;

  void Start()
  {
    spellController = afroParent.GetComponent<SpellControllerAfro>();
    fist = GetComponentInChildren<AfroFistController>();
  }

  void FixedUpdate()
  {
    if (fire && !fireFired)
    {
      fireFired = true;
      sphereCollider.enabled = false;
      fist.transform.SetParent(afroParent.transform);
      fist.Fire();

      fist = null;
      transform.localScale = Vector3.zero;
      GameObject fistObject = Instantiate(fistPrefab, transform.position, transform.rotation);
      fist = fistObject.GetComponent<AfroFistController>();
      fist.transform.SetParent(transform);
      fist.transform.localScale = Vector3.one;
      fist.spellController = spellController;
      fist.characterWallet = characterWallet;
    } else if (!fire && fireFired) {
      fireFired = false;
    }
  }
}
