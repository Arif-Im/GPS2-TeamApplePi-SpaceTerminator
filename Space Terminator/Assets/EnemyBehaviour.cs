using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    BehaviourTree tree;
    EnemyMovement enemyMovement;
    Unit unit;

    Node.Status treeStatus = Node.Status.RUNNING;

    private void Awake()
    {
        unit = GetComponent<Unit>();
        enemyMovement = GetComponent<EnemyMovement>();
    }

    private void Start()
    {
        //Initialize tree
        tree = new BehaviourTree();

        //Check if the agent has enough action points
        Leaf checkAP = new Leaf("Check AP", ChangeTurnOnFailure);

        //Idle sequence
        Sequence idle = new Sequence("Idle");
        Leaf stayIdle = new Leaf("Stay Idle", Idle);

        //attack sequence
        Sequence attack = new Sequence("Attack");
        Leaf canAttackPlayer = new Leaf("Can Attack Player", CanAttack);
        Leaf attackPlayer = new Leaf("Attack Player", AttackPlayer);

        //pursuit sequence
        Sequence pursuit = new Sequence("Pursuit");
        Leaf cannotAttack = new Leaf("Cannot Attack", CannotAttack);
        Leaf goToPlayer = new Leaf("Go To Player", GoToPlayer);

        //cover sequence
        Sequence cover = new Sequence("Cover");
        Leaf lowHealth = new Leaf("Low Health", LowHealth);
        Leaf getToCover = new Leaf("Get To Cover", GetToCover);

        //add leaves to idle sequence
        idle.AddChild(stayIdle);

        //add leaves to attack sequence
        attack.AddChild(checkAP);
        attack.AddChild(canAttackPlayer);
        attack.AddChild(attackPlayer);

        //add leaves to pursuit sequence
        pursuit.AddChild(checkAP);
        pursuit.AddChild(cannotAttack);
        pursuit.AddChild(goToPlayer);

        //add leaves to cover sequence
        cover.AddChild(checkAP);
        cover.AddChild(lowHealth);
        cover.AddChild(getToCover);

        //add sequences to state selector
        Selector state = new Selector("State");
        state.AddChild(idle);
        state.AddChild(attack);
        state.AddChild(pursuit);
        state.AddChild(cover);

        //add selector to behaviour tree
        tree.AddChild(state);

        //print out total states
        tree.PrintTree();
    }

    public Node.Status ChangeTurnOnFailure()
    {
        if (!unit.gameObject.GetComponent<TacticMove>().turn || unit.GetUnitPoints() < 1)
        {
            TurnManager.EndTurn();
            return Node.Status.FAILURE;
        }
        return Node.Status.SUCCESS;
    }

    #region Behaviours

    #region Idle
    public Node.Status Idle()
    {
        if(unit.gameObject.GetComponent<TacticMove>().turn)
        {
            return Node.Status.FAILURE;
        }
        return Node.Status.SUCCESS;
    }
    #endregion

    #region Attack
    public Node.Status CanAttack()
    {
        Debug.Log("Is Checking Attack");
        if (Vector3.Distance(this.gameObject.transform.position, enemyMovement.Player.transform.position) > enemyMovement.MoveTile || unit.HealthPercentage < 50)
        {
            return Node.Status.FAILURE;
        }
        //hasAttacked = false;
        return Node.Status.SUCCESS;
    }

    public Node.Status AttackPlayer()
    {
        //Debug.Log($"Current State: Attack");
        if (enemyMovement.attacking == false)
        {
            enemyMovement.attacking = true;
            StartCoroutine(enemyMovement.Shoot(enemyMovement.Player.gameObject.GetComponent<Grid>()));
            return Node.Status.SUCCESS;
        }
        return Node.Status.FAILURE;
    }

    private void Update()
    {
        //if (treeStatus != Node.Status.SUCCESS)
        if (unit.GetComponent<TacticMove>().attacking) return;
        treeStatus = tree.Process();
    }
    #endregion

    #region Pursuit
    public Node.Status CannotAttack()
    {
        if (Vector3.Distance(this.gameObject.transform.position, enemyMovement.Player.transform.position) < enemyMovement.MoveTile || unit.HealthPercentage < 50)
        {
            return Node.Status.FAILURE;
        }
        enemyMovement.FindNearestTarget();
        return Node.Status.SUCCESS;
    }

    public Node.Status GoToPlayer()
    {
        //Debug.Log($"Current State: Pursuit");
        if (!enemyMovement.Moving && enemyMovement.Path.Count <= 0)
        {
            return Node.Status.SUCCESS;
        }
        else if (!enemyMovement.Moving && enemyMovement.Path.Count > 0)
        {
            return Node.Status.FAILURE;
        }
        else
        {
            enemyMovement.EnemyMove();
            return Node.Status.RUNNING;
        }
    }
    #endregion

    #region Cover

    public Node.Status LowHealth()
    {
        if(unit.HealthPercentage >= 50)
        {
            return Node.Status.FAILURE;
        }
        enemyMovement.FindClosestCoverPosition();
        return Node.Status.SUCCESS;
    }    

    public Node.Status GetToCover()
    {
        //Debug.Log($"Current State: Cover");
        

        if (!enemyMovement.Moving && enemyMovement.Path.Count <= 0)
        {
            return Node.Status.SUCCESS;
        }
        else if (!enemyMovement.Moving && enemyMovement.Path.Count > 0)
        {
            return Node.Status.FAILURE;
        }
        else
        {
            enemyMovement.EnemyMove();
            return Node.Status.RUNNING;
        }
    }

    #endregion

    #endregion
}
