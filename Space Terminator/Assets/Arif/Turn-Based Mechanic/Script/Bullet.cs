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

    public float Damage { get => damage; }

    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
        switch(GameObject.FindGameObjectWithTag("Dice").GetComponent<Dice>().FinalSide)
        {
            case 1: case 3: case 5:
                damage = 0;
                break;
            case 2: case 4: case 6:
                damage = damagePoint;
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Grid") || collision.gameObject.CompareTag("Alien"))
            Destroy(gameObject);
    }
}