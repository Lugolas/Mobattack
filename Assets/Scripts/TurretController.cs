using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
  public GameObject target;
  [ColorUsageAttribute(true, true)]
  public Color green;
  [ColorUsageAttribute(true, true)]
  public Color red;
  public Collider space;
  public GameObject visualSpace;
  public GameObject visualRange;
  public GameObject visualModel;
  float rangeRatio = 5f;
  public List<GameObject> enemiesInRange = new List<GameObject>();
  public Transform fireballSpawnPoint;
  public GameObject fireballPrefabHurt;
  public GameObject fireballPrefabHeal;
  public MoneyManager masterWallet;
  public TurretSpaceCheck spaceCheck;
  CheckInfo checkInfo;
  public bool activated = false;
  public bool isBeingChecked = false;
  bool HURT = false;
  bool HEAL = true;
  private bool targetUpdateWanted = false;
  CapsuleCollider rangeCollider;
  TurretStatManager statManager;
  public GameObject upgradedModelParts;

  // Let's have a 0.5s timer or something
  float fireTime;

  // Start is called before the first frame update
  void Start()
  {
    statManager = GetComponent<TurretStatManager>();
    spaceCheck = GetComponentInChildren<TurretSpaceCheck>();
    rangeCollider = GetComponent<CapsuleCollider>();
    checkInfo = GetComponentInChildren<CheckInfo>();
    rangeCollider.radius = statManager.range;
    visualRange.transform.localScale = new Vector3(statManager.range / rangeRatio, statManager.range / rangeRatio, statManager.range / rangeRatio);
    fireTime = Time.time;
  }

  // Update is called once per frame
  void Update()
  {
    if (activated && target && Time.time >= fireTime + statManager.delay)
    {
      fire(HURT);
    }
    if (!activated)
    {
      Color color;
      if (spaceCheck.enoughSpace)
      {
        color = green;
      }
      else
      {
        color = red;
      }
      Renderer[] renderers;
      renderers = visualSpace.GetComponentsInChildren<Renderer>();
      foreach (Renderer renderer in renderers)
      {
        List<Material> materials = new List<Material>();
        renderer.GetMaterials(materials);
        foreach (Material material in materials)
        {
          material.SetColor("_EmissionColor", color);
        }
      }
    }
    else
    {
      if (isBeingChecked)
      {
      }
    }
  }

  void LateUpdate()
  {
    if (targetUpdateWanted)
    {
      updateTarget();
    }
  }

  void OnTriggerEnter(Collider collider)
  {
    EnemyController enemy = collider.gameObject.GetComponent<EnemyController>();

    if (activated && enemy)
    {
      enemiesInRange.Add(enemy.gameObject);
    }
    targetUpdateWanted = true;
  }

  void OnTriggerExit(Collider collider)
  {
    EnemyController enemy = collider.gameObject.GetComponent<EnemyController>();

    if (activated && enemy)
    {
      enemiesInRange.Remove(enemy.gameObject);
    }
    targetUpdateWanted = true;
  }

  void updateTarget()
  {
    List<int> invalidEnemiesIndexes = new List<int>();

    for (int i = 0; i < enemiesInRange.Count; i++)
    {
      GameObject enemy = enemiesInRange[i];
      if (!enemy || enemy.GetComponent<HealthSimple>().isDead)
      {
        invalidEnemiesIndexes.Add(i);
      }
    }

    foreach (int index in invalidEnemiesIndexes)
    {
      // This is no no good good
      // Because sometimes it removes the same index two times in a row, so the wrong target gets removed from the array
      // This only stops the case where the index gets greater than the array length
      if (index < enemiesInRange.Count)
      {
        enemiesInRange.RemoveAt(index);
      }
    }

    if (enemiesInRange.Count > 0)
    {
      target = enemiesInRange[0];
    }
    else
    {
      target = null;
    }
    targetUpdateWanted = false;
  }

  void fire(bool fireballBehaviour)
  {
    fireTime = Time.time;

    GameObject fireballPrefab;
    if (fireballBehaviour == HEAL)
    {
      fireballPrefab = fireballPrefabHeal;
    }
    else
    {
      fireballPrefab = fireballPrefabHurt;
    }

    GameObject fireball = Instantiate(fireballPrefab, fireballSpawnPoint.position, fireballSpawnPoint.rotation) as GameObject;
    Fireball fireballScript = fireball.GetComponent<Fireball>();
    fireballScript.damage = statManager.damage;
    // fireballScript.emitter = this;
    fireballScript.target = target.transform;
    // fireballScript.masterWallet = masterWallet;
  }

  public void Activate()
  {
    activated = true;
    SetVisuals(false, false);
  }

  public void Check()
  {
    SetVisuals(true, false);
    isBeingChecked = true;
  }

  public void StopCheck()
  {
    SetVisuals(false, false);
    isBeingChecked = false;
  }

  void SetVisuals(bool range, bool space)
  {
    visualRange.SetActive(range);
    visualSpace.SetActive(space);
  }

  void VisualUpgrade()
  {
    rangeCollider.radius = statManager.range;
    visualRange.transform.localScale = new Vector3(statManager.range / rangeRatio, statManager.range / rangeRatio, statManager.range / rangeRatio);
    upgradedModelParts.SetActive(true);
    visualSpace.transform.localScale = new Vector3(visualSpace.transform.localScale.x * 1.1f, visualSpace.transform.localScale.y * 1.1f, visualSpace.transform.localScale.z * 1.1f);
    visualModel.transform.localScale = new Vector3(visualModel.transform.localScale.x * 1.1f, visualModel.transform.localScale.y * 1.1f, visualModel.transform.localScale.z * 1.1f);
    upgradedModelParts.transform.localScale = new Vector3(upgradedModelParts.transform.localScale.x * 1.1f, upgradedModelParts.transform.localScale.y * 1.1f, upgradedModelParts.transform.localScale.z * 1.1f);
  }
}
