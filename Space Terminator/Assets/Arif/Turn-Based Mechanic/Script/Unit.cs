using System;
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

    public int overwatchCooldown = 0;
    public int duckingCooldown = 0;
    //public delegate void OverwatchAction(Grid targetEnemy);
    //public OverwatchAction onOverWatchAction;

    public float Health { get => currentHealth; }

    public float HealthPercentage 
    {
        get
        {
            //Debug.Log(currentHealth / maxHealth * 100);
            return currentHealth / maxHealth * 100;
        }
    }

    public float PunchDamage { get => punchDamagePoint; }


    public UnitPoitsSystem unitPointsSystem;
    public bool isDucking = false;
    public bool isOverwatch = false;
    public bool isCurrentTurn;
    public bool interrupted;

    public int GetUnitPoints()
    {
        return unitPointsSystem.CurrentPoints;
    }

    private void Awake()
    {
        unitPointsSystem = GetComponent<UnitPoitsSystem>();
    }

    private void Start()
    {
        //currentHealth = maxHealth;
        overwatchCooldown = 0;
        battleSystem = FindObjectOfType<BattleSystem>();
    }

    private void Update()
    {
        healthBar.UpdateHealth(currentHealth / maxHealth);
        Overwatch();

        //if (currentHealth <= 0)
        //{
        //Destroy(gameObject);
        //unitPointsSystem.CurrentPoints = 0;
        //GetComponent<Grid>().selectable = false;
        //transform.gameObject.tag = "Grid";
        //Destroy(gameObject.GetComponent<CapsuleCollider>());
        //}
    }

    [SerializeField] LayerMask whatIsEnemy;

    public void Overwatch()
    {
        if (isOverwatch/* && overwatchCooldown <= 0*/)
        {
            Collider[] enemiesToDamage = Physics.OverlapSphere(transform.position, GetComponent<TacticMove>().MoveTile, whatIsEnemy);
            foreach (Collider enemy in enemiesToDamage)
            {
                Debug.Log("Overwatch");
                GetComponent<TacticMove>().attacking = true; // avoids repeating attack
                StartCoroutine(GetComponent<TacticMove>().Shoot(enemy.GetComponent<Grid>(), null)); // shoot without removing AP
                enemy.GetComponent<Unit>().interrupted = true; // stops the opponent when being attack
                isOverwatch = false; // stops overwatch when attacking
                break;
            }
            //DeductPointsOrChangeTurn(unitPointsSystem.CurrentPoints);
        }
    }

    public void Activate(Action action)
    {
        action.Invoke();
        DeductPointsOrChangeTurn(GetUnitPoints());
    }

    public void DeductPointsOrChangeTurn(int amount)
    {
        unitPointsSystem.minusPoints(amount);

        if(GetUnitPoints() < 1 && GameObject.FindGameObjectWithTag("Turn Manager").GetComponent<TurnManager>().attackState == AttacksState.Idle)
        {
            //ChangeTurn();
            if(overwatchCooldown > 0)
            {
                overwatchCooldown -= 1;
            }
            if(duckingCooldown > 0)
            {
                duckingCooldown -= 1;
            }
            //else
            //{
            //    isOverwatch = false;
            //}
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
        if (!GetComponent<TacticMove>().GetTargetTile(gameObject).isCover)
        {
            isDucking = false;
        }

        if (other.gameObject.CompareTag("Bullet"))
        {
            if(gameObject.CompareTag("Player") || gameObject.CompareTag("Alien"))
            {
                if (gameObject.tag == other.gameObject.GetComponent<Bullet>().Shooter.tag) return;

                Destroy(other.gameObject);

                if (floatingText != null)
                {
                    var go = Instantiate(floatingText, transform.position, Quaternion.identity, transform);
                    if (isDucking)
                        go.GetComponent<TextMesh>().text = "Ducking";
                    else if (other.gameObject.GetComponent<Bullet>().Damage != 0)
                        go.GetComponent<TextMesh>().text = other.gameObject.GetComponent<Bullet>().Damage.ToString();
                    else
                        go.GetComponent<TextMesh>().text = "Miss";
                }
                TacticMove shooter = other.GetComponent<Bullet>().Shooter.GetComponent<TacticMove>();
                if (GetComponent<TacticMove>().GetTargetTile(gameObject).CoverOrigin != null && GetComponent<TacticMove>().GetTargetTile(gameObject).isCover && shooter.GetTargetTile(shooter.gameObject).isCoverEffectArea)
                {
                    GetComponent<TacticMove>().GetTargetTile(gameObject).CoverObject.DamageGrid(1);
                }
                if (isDucking) return;
                healthBar.UpdateHealth(currentHealth / maxHealth);
                TakeDamage(other.gameObject.GetComponent<Bullet>().Damage);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (isOverwatch)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, GetComponent<TacticMove>().MoveTile);
        }
    }
}
