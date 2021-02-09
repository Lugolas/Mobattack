using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangloxSlamController : MonoBehaviour
{
  List<GameObject> touchedObjects = new List<GameObject>();
  public int damage;
  public MoneyManager characterWallet;
  Animator animator;
  bool isAnimating = false;
  TriangloxSlamCollision slamCollision;

  // Start is called before the first frame update
  void Start()
  {
    animator = GetComponent<Animator>();
    slamCollision = GetComponentInChildren<TriangloxSlamCollision>();
  }

  // Update is called once per frame
  void Update()
  {
    if (animator.GetCurrentAnimatorStateInfo(0).IsName("Slam"))
    {
      isAnimating = true;
    }
    else if (isAnimating)
    {
      isAnimating = false;
      slamCollision.enabled = false;

      Destroy(gameObject, 2f);
    }
  }

  public void Slam(GameObject target)
  {
    if (!touchedObjects.Contains(target))
    {
      Tools.InflictDamage(target.transform, damage, characterWallet, gameObject);
      touchedObjects.Add(target);
    }
  }
}
