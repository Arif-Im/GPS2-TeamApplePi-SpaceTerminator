using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    public int damage;
    public float speed;

    public bool isCurrentTurn;

    private void Update()
    {
        if(currentHealth <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
