using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceHolder : MonoBehaviour
{
    public GameObject self;
    public Animator animator;
    public Animator nextAnim;
    public void Start()
    {
        Time.timeScale = 1;

    }
    public void nextStage(GameObject nextObject)
    {
        StartCoroutine(nextDecision(nextObject));
    }

    IEnumerator nextDecision(GameObject nextObject)
    {

        animator.SetBool("change", true);
        yield return new WaitForSeconds(1f);
        self.gameObject.SetActive(false);
        nextObject.gameObject.SetActive(true);
        nextAnim.SetBool("back", true);
        yield return new WaitForSeconds(1f);
        animator.SetBool("change", false);
        nextAnim.SetBool("back", false);

    }
}
