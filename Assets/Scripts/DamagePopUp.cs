using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamagePopUp : MonoBehaviour
{
  public Animator animator;
  private Text damageText;

  // Start is called before the first frame update
  void Start()
  {
    animator.SetInteger("direction", Random.Range(-3, 3));
    AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
    Destroy(gameObject, clipInfo[0].clip.length);

    damageText = animator.GetComponent<Text>();
  }

  public void SetText(string text)
  {
    // damageText.text = text;
    animator.GetComponent<Text>().text = text;
  }

  // Update is called once per frame
  void Update()
  {
  }
}
