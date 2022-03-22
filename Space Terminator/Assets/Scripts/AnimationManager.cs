using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    Animator anim;
    public bool isAlien;
    public bool isPlayer;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isAlien)
        {
            anim.SetBool("isWalking", GetComponentInParent<EnemyMovement>().isWalking);
            anim.SetBool("isAttacking", GetComponentInParent<TacticMove>().isAttack);
        }
        
        if (isPlayer)
        {
            anim.SetBool("isWalking", GetComponentInParent<PlayableMovement>().isWalking);
            anim.SetBool("isAttacking", GetComponentInParent<TacticMove>().isAttack);
        }

    }
}
