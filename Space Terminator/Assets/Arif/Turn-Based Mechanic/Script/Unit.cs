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

    public AttackState state;
    [SerializeField] float punchDamagePoint;
    [SerializeField] GameObject floatingText;

    public int rollToHit = 3;

    public int overwatchCooldown = 0;
    public int duckingCooldown = 0;

    public float Health { get => currentHealth; }
    public bool isDamaged = false;
    public float HealthPercentage 
    {
        get
        {
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
        currentHealth = maxHealth;
        overwatchCooldown = 0;
        if (gameObject.tag == "Player")
            StarSystem.instance.AddTroop();
    }

    private void Update()
    {
        healthBar.UpdateHealth(currentHealth / maxHealth);
        if (gameObject.tag == "Player")
        {
            healthBar.UpdateTextHealth(currentHealth);
        }
        Overwatch();
    }

    [SerializeField] LayerMask whatIsEnemy;

    public void Overwatch()
    {
        if (isOverwatch)
        {
            Collider[] enemiesToDamage = Physics.OverlapSphere(transform.position, GetComponent<TacticMove>().MoveTile, whatIsEnemy);
            foreach (Collider enemy in enemiesToDamage)
            {
                //Debug.Log("Overwatch");
                //GetComponent<TacticMove>().attacking = true; // avoids repeating attack
                StartCoroutine(GetComponent<TacticMove>().Shoot(enemy.GetComponent<Grid>(), null)); // shoot without removing AP
                enemy.GetComponent<Unit>().interrupted = true; // stops the opponent when being attack
                isOverwatch = false; // stops overwatch when attacking
                break;
            }
        }
    }

    public void Activate(Action action)
    {
        if(CooldownHasEnded())
        {
            action.Invoke();
            DeductPointsOrChangeTurn(GetUnitPoints());
        }
    }

    public bool CooldownHasEnded()
    {
        return overwatchCooldown <= 0 || duckingCooldown <= 0;
    }

    public void DeductPointsOrChangeTurn(int amount)
    {
        unitPointsSystem.minusPoints(amount);

        if (GetUnitPoints() > 0 && gameObject.tag == "Player") return;

        if(GetUnitPoints() < 1 && GameObject.FindGameObjectWithTag("Turn Manager").GetComponent<TurnManager>().attackState == AttacksState.Idle)
        {
            if(overwatchCooldown > 0)
            {
                overwatchCooldown -= 1;
            }
            if(duckingCooldown > 0)
            {
                duckingCooldown -= 1;
            }
            GetComponent<TacticMove>().arrow.SetActive(false);
            TurnManager.EndTurn();
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            isOverwatch = false;
            isDucking = false;

            if(TryGetComponent(out ScoutMovement scoutMovement))
                TurnManager.RemoveUnit(scoutMovement);
            else
                TurnManager.RemoveUnit(this.GetComponent<TacticMove>());

            if (gameObject.tag == "Player")
                StarSystem.instance.RemoveTroop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!GetComponent<TacticMove>().GetTargetTile(gameObject).isCover)
        {
            isDucking = false;
        }

        if (other.gameObject.CompareTag("Bullet"))
        {
            //Debug.Log("Hit Bullet");
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
                if (gameObject.tag == "Player")
                {
                    healthBar.UpdateTextHealth(currentHealth);
                }
                TakeDamage(other.gameObject.GetComponent<Bullet>().Damage);
                StartCoroutine(DamageAnim());
            }
        }

        if (gameObject.tag == "Player")
        {
            if (other.gameObject.CompareTag("Documents"))
            {
                StarSystem.instance.CollectedDocuments();
                other.gameObject.SetActive(false);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (isOverwatch)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, GetComponent<TacticMove>().MoveTile);
        }
    }

    IEnumerator DamageAnim()
    {
        isDamaged = true;
        yield return new WaitForSeconds(.3f);
        isDamaged = false;
    }

    public void BurnDamage()
    {
        Debug.Log("floating text");
        var go = Instantiate(floatingText, transform.position, Quaternion.identity, transform);
        go.GetComponent<TextMesh>().text = 3.ToString();
        currentHealth -= 3;
        if (healthBar != null && floatingText != null)
          healthBar.UpdateHealth(currentHealth / maxHealth);
        if(gameObject.tag == "Player")
        {
            healthBar.UpdateTextHealth(currentHealth);
        }
    
    }
}
