using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
  public float radius;
  public int damage;
  public MoneyManager moneyManager;
  public SpellController spellController;
  public LayerMask layerMask;
  bool passedOnce = false;
  bool passedTwice = false;
  float passedOnceTime;
  float passDelay = .1f;
  public EnemyController enemyToAvoid;
  void Update()
  {
    if (!passedOnce && radius != 0 && moneyManager && spellController) {
      passedOnce = true;
      passedOnceTime = Time.time;
      Collider[] colliders = Physics.OverlapSphere(transform.position, radius, layerMask, QueryTriggerInteraction.Ignore);
      List<EnemyController> enemies = TakeOnlyEnemies(colliders);
      foreach (EnemyController enemy in enemies)
      {
        if (enemy != enemyToAvoid) {
          Tools.InflictDamage(enemy.transform, damage, moneyManager, spellController);
        }
      }
    }
    if (passedOnce && !passedTwice && Time.time > passedOnceTime + passDelay) {
      passedTwice = true;
      Collider[] colliders = Physics.OverlapSphere(transform.position, radius, layerMask, QueryTriggerInteraction.Ignore);
      List<CorpsePartTimer> corpseParts = TakeOnlyCorpsePart(colliders);
      foreach (CorpsePartTimer corpsePart in corpseParts)
      {
        corpsePart.GetComponent<Rigidbody>().AddExplosionForce(damage * 2, transform.position, radius, 0, ForceMode.Impulse);
      }
    }
  }

  List<EnemyController> TakeOnlyEnemies(Collider[] colliders) {
    List<EnemyController> enemies = new List<EnemyController>();

    foreach (Collider collider in colliders)
    {
      EnemyController enemy = collider.GetComponent<EnemyController>();
      if (!enemy) {
        enemy = collider.GetComponentInParent<EnemyController>();
      }
      if (enemy && !enemies.Contains(enemy)) {
        enemies.Add(enemy);
      }
    }

    return enemies;
  }

  List<CorpsePartTimer> TakeOnlyCorpsePart(Collider[] colliders) {
    List<CorpsePartTimer> corpseParts = new List<CorpsePartTimer>();

    foreach (Collider collider in colliders)
    {
      CorpsePartTimer corpsePart = collider.GetComponent<CorpsePartTimer>();
      if (!corpsePart) {
        corpsePart = collider.GetComponentInParent<CorpsePartTimer>();
      }
      if (corpsePart && !corpseParts.Contains(corpsePart)) {
        EnemyController enemy = corpsePart.GetComponent<EnemyController>();
        if (!enemy) {
          enemy = corpsePart.GetComponentInParent<EnemyController>();
        }
        if (enemy && enemy.hasDied) {
          corpseParts.Add(corpsePart);
        }
      }
    }

    return corpseParts;
  }
}
