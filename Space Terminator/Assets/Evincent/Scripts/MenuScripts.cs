using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuScripts : MonoBehaviour
{
    public GameObject self;
    public Animator animator;
    public Animator nextAnim;
    public StackMnager stackMnager;
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

        //animator.SetBool("change", true);
        //yield return new WaitForSeconds(1f);
        //self.gameObject.SetActive(false);
        //nextObject.gameObject.SetActive(true);
        //nextAnim.SetBool("back", true);
        //yield return new WaitForSeconds(1f);
        //animator.SetBool("change", false);
        //animator.SetBool("back", false);
        //nextAnim.SetBool("change", false);
        //nextAnim.SetBool("back", false);
        animator.SetBool("change", true);
        yield return new WaitForSeconds(2f);
        self.gameObject.SetActive(false);
        nextObject.gameObject.SetActive(true);
        nextAnim.SetBool("back", true);
        yield return new WaitForSeconds(2f);
        animator.SetBool("change", false);
        animator.SetBool("back", false);
        nextAnim.SetBool("back", false);
        nextAnim.SetBool("change", false);

    }
    public void testing(GameObject nextOne)
    {
        self.gameObject.SetActive(false);
        nextOne.gameObject.SetActive(true);
    }

    public void gameScene()
    {
        if (stackMnager.queue.Count == 0)
        {
            Debug.Log("You must have atleast 1 unit in your squad");
            
        }
        else
        {
            SceneManager.LoadScene(2);
        }
    }

    public void BckMainMenu()
    {
        SceneManager.LoadScene("LobbyMenu");
    }

}
