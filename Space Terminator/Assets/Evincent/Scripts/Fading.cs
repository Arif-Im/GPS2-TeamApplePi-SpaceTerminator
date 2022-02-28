using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fading : MonoBehaviour
{
    public Animator animator;

    private void Start()
    {
        animator.SetBool("start", true);
    }
}
