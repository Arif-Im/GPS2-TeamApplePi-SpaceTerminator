using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    private void Update()
    {
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //RaycastHit hit;

        //if (Physics.Raycast(ray, out hit))
        //{
        //    if (hit.collider.tag == "Player")
        //    {
        //        if (Input.GetMouseButtonDown(0) && FindObjectOfType<DialogueManager>() != null)
        //        {
        //            GameObject.FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        //        }

        //    }
        //}
        //if(FindObjectOfType<DialogueManager>() != null)
        //{
        //    GameObject.FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        //}
        
    }
    //private void Start()
    //{
    //    GameObject.FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    //}
}
