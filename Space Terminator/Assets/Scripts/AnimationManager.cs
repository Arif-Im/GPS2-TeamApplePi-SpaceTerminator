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
            anim.SetBool("isDead", GetComponentInParent<EnemyMovement>().isDead);
            anim.SetBool("isDamaged", GetComponentInParent<Unit>().isDamaged);
        }
        
        if (isPlayer)
        {
            var parentName = transform.name;

            if (parentName == "Assault" || parentName == "Unit4")
            {
                anim.SetBool("isWalking", GetComponentInParent<PlayableMovement>().isWalking);
                //anim.SetBool("isWalking", GetComponentInParent<ScoutMovement>().isWalking);
                anim.SetBool("isAttacking", GetComponentInParent<TacticMove>().isAttack);
                anim.SetBool("isDead", GetComponentInParent<PlayableMovement>().isDead);
                //anim.SetBool("isDead", GetComponentInParent<ScoutMovement>().isDead);
                anim.SetBool("isPunching", GetComponentInParent<TacticMove>().isPunching);
                anim.SetBool("isDamaged", GetComponentInParent<Unit>().isDamaged);
                anim.SetBool("isOverwatch", GetComponent<Unit>().isOverwatch);
            }
           
           
            if (parentName == "Scout" || parentName == "Heavy")
            {
                anim.SetBool("isWalking", GetComponentInParent<ScoutMovement>().isWalking);
                anim.SetBool("isAttacking", GetComponentInParent<ScoutMovement>().isAttacking);
                anim.SetBool("isDead", GetComponentInParent<ScoutMovement>().isDead);
                anim.SetBool("isPunching", GetComponentInParent<TacticMove>().isPunching);
                anim.SetBool("isDamaged", GetComponentInParent<Unit>().isDamaged);
                anim.SetBool("isOverwatch", GetComponent<Unit>().isOverwatch);
            }
           
        }

    }
}
