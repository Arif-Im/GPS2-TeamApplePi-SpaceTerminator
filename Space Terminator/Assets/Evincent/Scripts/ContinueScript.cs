using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ContinueScript : MonoBehaviour
{
    private Vector3 target;
    public Animator animator;

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            StartCoroutine(Starting());
        }
    }

    IEnumerator Starting()
    {
        target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        target.z = transform.position.z;
        animator.SetBool("start", true);
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(1);
    }
}
