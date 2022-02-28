using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bullet : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float damagePoint = 2f;
    float damage;
    public GameObject Shooter { get;  set; }

    public float Damage { get => damage; }

    void Start()
    {
        damage = damagePoint;
        switch (GameObject.FindGameObjectWithTag("Dice").GetComponent<Dice>().FinalSide)
        {
            case 1:case 3:case 5:
                damage = 0;
                break;

            case 2:case 4:case 6:
                int critSide = GameObject.FindGameObjectWithTag("Crit Dice").GetComponent<Dice>().FinalSide;
                if (critSide == 2 || critSide == 4 || critSide == 6)
                    damage = damagePoint * 2;
                else
                    damage = damagePoint;
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