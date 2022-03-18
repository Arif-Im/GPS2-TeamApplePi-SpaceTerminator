using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bullet : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float damagePoint = 2f;
    int rollToHit;
    bool inCoverEffect = false;

    float damage;

    public GameObject Shooter { get;  set; }
    public int UnitRollToHit { set => rollToHit = value; }
    public bool InCoverEffect { set => inCoverEffect = value; }

    public float Damage { get => damage; }

    void Start()
    {
        damage = damagePoint;
        if (inCoverEffect)
        {
            if (inCoverEffect)
            {
                Debug.Log("Opponent in cover");
                DamageCheck(0);
            }
            else
            {
                DamageCheck(damagePoint);
            }
        }
        else
        {
            DamageCheck(damagePoint);
        }
    }

    private void DamageCheck(float failCritDamage)
    {
        //switch (GameObject.FindGameObjectWithTag("Dice").GetComponent<Dice>().FinalSide)
        //{
            //case 1: case 3: case 5:
            //    damage = 0; // should be 0
            //    break;

            //case 2: case 4: case 6:
            //    int critSide = GameObject.FindGameObjectWithTag("Crit Dice").GetComponent<Dice>().FinalSide;
            //    if (critSide == 2 || critSide == 4 || critSide == 6)
            //        damage = damagePoint * 2;
            //    else
            //        damage = failCritDamage;
            //    break;
        //}

        if(Shooter.GetComponent<TacticMove>().GetTargetTile(Shooter).isCover)
        {
            rollToHit -= 1;
            CalculateHit(failCritDamage);
        }
        else
        {
            CalculateHit(failCritDamage);
        }
    }

    private void CalculateHit(float failCritDamage)
    {
        if (GameObject.FindGameObjectWithTag("Dice").GetComponent<Dice>().FinalSide > rollToHit)
            damage = 0;
        else
        {
            int critSide = GameObject.FindGameObjectWithTag("Crit Dice").GetComponent<Dice>().FinalSide;
            if (critSide == GameObject.FindGameObjectWithTag("Dice").GetComponent<Dice>().FinalSide)
                damage = damagePoint * 2;
            else
                damage = failCritDamage;
        }
    }

    private void Update()
    {
        Destroy(gameObject, 3f);
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    //private void OnTriggerEnter(Collider collision)
    //{
    //    if (collision.gameObject.tag != Shooter.tag)
    //    {
    //        if (collision.gameObject.CompareTag("Grid"))
    //            Destroy(gameObject);
    //    }
    //}
}