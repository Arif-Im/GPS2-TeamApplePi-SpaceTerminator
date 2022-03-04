using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bullet : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float damagePoint = 2f;

    bool opponentInCover = false;
    bool inCoverEffect = false;

    float damage;

    public GameObject Shooter { get;  set; }

    public bool OpponentInCover { set => opponentInCover = value; }
    public bool InCoverEffect { set => inCoverEffect = value; }

    public float Damage { get => damage; }

    void Start()
    {
        damage = damagePoint;
        if (opponentInCover)
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
        switch (GameObject.FindGameObjectWithTag("Dice").GetComponent<Dice>().FinalSide)
        {
            case 1: case 3: case 5:
                damage = 0;
                break;

            case 2: case 4: case 6:
                int critSide = GameObject.FindGameObjectWithTag("Crit Dice").GetComponent<Dice>().FinalSide;
                if (critSide == 2 || critSide == 4 || critSide == 6)
                    damage = damagePoint * 2;
                else
                    damage = failCritDamage;
                break;
        }
    }

    private void Update()
    {
        Destroy(gameObject, 3f);
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Grid"))
            Destroy(gameObject);
    }
}