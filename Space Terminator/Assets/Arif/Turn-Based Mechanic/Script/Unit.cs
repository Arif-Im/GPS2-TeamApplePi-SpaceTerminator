using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    public int damage;
    public float speed;

    BattleSystem battleSystem;

    public UnitPoitsSystem unitPointsSystem;
    bool isTakingCover = false;
    public bool isCurrentTurn;

    public float GetUnitPoints()
    {
        return unitPointsSystem.CurrentPoints;
    }

    private void Awake()
    {
        unitPointsSystem = GetComponent<UnitPoitsSystem>();
    }

    private void Start()
    {
        battleSystem = FindObjectOfType<BattleSystem>();
    }

    public void DeductPointsOrChangeTurn(int amount)
    {
        unitPointsSystem.minusPoints(amount);

        if(unitPointsSystem.CurrentPoints < 1)
        {
            ChangeTurn();
        }
    }

    public void ChangeTurn()
    {
        battleSystem.ChangeTurn(this, unitPointsSystem.maxPoints);
    }


    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
    }

    private void Update()
    {
        if(currentHealth <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
