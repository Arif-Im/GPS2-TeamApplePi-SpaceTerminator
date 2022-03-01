using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackState { Idle, UnderAttack, FinishAttacked}

public class Unit : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    public Healthbar healthBar;
    public float damage;
    public float speed;

    BattleSystem battleSystem;
    public AttackState state;
    [SerializeField] float punchDamagePoint;
    [SerializeField] GameObject floatingText;

    public float Health { get => currentHealth; }
    public float PunchDamage { get => punchDamagePoint; }


    public UnitPoitsSystem unitPointsSystem;
    bool isTakingCover = false;
    public bool isCurrentTurn;

    public bool IsTakingCover
    {
        set
        {
            Debug.Log("Player taking cover");
            isTakingCover = value;
        }
    }

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
        currentHealth = maxHealth;
        battleSystem = FindObjectOfType<BattleSystem>();
    }

    private void Update()
    {
        healthBar.UpdateHealth(currentHealth / maxHealth);

        if (currentHealth <= 0)
        {
            //Destroy(gameObject);
            //unitPointsSystem.CurrentPoints = 0;
            //GetComponent<Grid>().selectable = false;
            //transform.gameObject.tag = "Grid";
            //Destroy(gameObject.GetComponent<CapsuleCollider>());
        }
    }

    public void DeductPointsOrChangeTurn(int amount)
    {
        unitPointsSystem.minusPoints(amount);

        if(unitPointsSystem.CurrentPoints < 1 && GameObject.FindGameObjectWithTag("Turn Manager").GetComponent<TurnManager>().attackState == AttacksState.Idle)
        {
            //ChangeTurn();
            TurnManager.EndTurn();
        }
    }

    public void ChangeTurn()
    {
        battleSystem.ChangeTurn(this, unitPointsSystem.maxPoints);
    }


    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Bullet"))
        {
            if(gameObject.CompareTag("Player") || gameObject.CompareTag("Alien"))
            {
                if (gameObject.tag == other.gameObject.GetComponent<Bullet>().Shooter.tag) return;

                Destroy(other.gameObject);

                if (floatingText != null)
                {
                    var go = Instantiate(floatingText, transform.position, Quaternion.identity, transform);

                    if (other.gameObject.GetComponent<Bullet>().Damage != 0)
                        go.GetComponent<TextMesh>().text = other.gameObject.GetComponent<Bullet>().Damage.ToString();
                    else
                        go.GetComponent<TextMesh>().text = "Miss";
                }
                Debug.Log($"Damage: {other.gameObject.GetComponent<Bullet>().Damage}");
                healthBar.UpdateHealth(currentHealth / maxHealth);
                TakeDamage(other.gameObject.GetComponent<Bullet>().Damage);
            }
        }
    }
}
