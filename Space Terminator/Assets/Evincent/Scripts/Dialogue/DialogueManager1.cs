//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//public class DialogueManager1 : MonoBehaviour
//{
//    public Text nameText;
//    public string charaName;
//    public Text dialogueText;
//    public Animator animator;
//    public DialogueManager tes;
//    [TextArea(3, 10)]
//    public string[] lines;
//    public float textSpeed;
//    private int index;

//    private void Start()
//    {
//        animator.SetBool("isOpen", true);
//        dialogueText.text = string.Empty;
//        nameText.text = charaName;
//        StartCoroutine(testing());
//    }

//    IEnumerator testing()
//    {
//        if(tes.Index < 0)
//        {
//            yield return new WaitForSeconds(5f);
//            StartDialogue();
//        }
//    }
//    private void Update()
//    {
//        if (Input.GetMouseButtonDown(0))
//        {
//            Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//            target.z = transform.position.z;
//            NextLine();
//        }
//        else
//        {
//            StopAllCoroutines();
//            dialogueText.text = lines[index];
//        }
//    }

//    void StartDialogue()
//    {
//        index = 0;
//        StartCoroutine(TypeLine());
//    }

//    IEnumerator TypeLine()
//    {
//        foreach ( char c in lines[index].ToCharArray())
//        {
//            dialogueText.text += c;
//            yield return new WaitForSeconds(textSpeed);
//        }
//    }

//    void NextLine()
//    {
//        if(index < lines.Length - 1)
//        {
//            index++;
//            dialogueText.text = string.Empty;
//            StartCoroutine(TypeLine());
//        }
//        else
//        {
//            animator.SetBool("isOpen", false);
//        }
//    }
//}
