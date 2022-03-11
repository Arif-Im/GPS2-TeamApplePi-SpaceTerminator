using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    BehaviourTree tree;
    EnemyMovement enemyMovement;
    Unit unit;
    //UnitPoitsSystem unitPointSystem;

    int random = 1;

    Node.Status treeStatus = Node.Status.RUNNING;

    private void Awake()
    {
        random = 1;
        unit = GetComponent<Unit>();
        //unitPointSystem = GetComponent<UnitPoitsSystem>();
        enemyMovement = GetComponent<EnemyMovement>();
    }

    private void Start()
    {
        //Initialize tree
        tree = new BehaviourTree();

        //Check if the agent has enough action points
        Leaf checkAP = new Leaf("Check AP", ChangeTurnOnFailure);

        //Update the tree on the specified target's position within the grid
        Leaf computeTarget = new Leaf("Compute Target", ComputeTarget);

        //Sets a target for the enemy to go to and moves it there
        Leaf goToTarget = new Leaf("Go To Player", GoToTarget);

        //Idle sequence
        Sequence idle = new Sequence("Idle");
        Leaf stayIdle = new Leaf("Stay Idle", Idle);

        //attack sequence
        Sequence attack = new Sequence("Attack");
        Leaf canAttackPlayer = new Leaf("Can Attack Player", CanAttack);
        Leaf attackPlayer = new Leaf("Attack Player", AttackPlayer);

        //overwatch sequence
        Sequence overwatch = new Sequence("Overwatch");
        Leaf startOverwatch = new Leaf("Start Overwatch", Overwatch);

        //pursuit sequence
        Sequence pursuit = new Sequence("Pursuit");
        Leaf startPursuit = new Leaf("Cannot Attack", StartPursuit);

        //cover sequence
        Sequence cover = new Sequence("Cover");
        Leaf lowHealth = new Leaf("Low Health", LowHealth);

        //ducking sequence
        Sequence duck = new Sequence("Duck");
        Leaf canDuck = new Leaf("Can Duck", CanDuck);
        Leaf startDucking = new Leaf("Start Ducking", StartDucking);

        //add leaves to idle sequence
        idle.AddChild(stayIdle);

        //add leaves to attack sequence
        attack.AddChild(computeTarget);
        attack.AddChild(checkAP);
        attack.AddChild(canAttackPlayer);
        attack.AddChild(attackPlayer);

        //add leaves to overwatch sequence
        overwatch.AddChild(startOverwatch);

        //add leaves to pursuit sequence
        pursuit.AddChild(computeTarget);
        pursuit.AddChild(checkAP);
        pursuit.AddChild(startPursuit);
        pursuit.AddChild(goToTarget);

        //add leaves to cover sequence
        cover.AddChild(computeTarget);
        cover.AddChild(checkAP);
        cover.AddChild(lowHealth);
        cover.AddChild(goToTarget);

        //add leaves to duck sequence
        duck.AddChild(computeTarget);
        duck.AddChild(checkAP);
        duck.AddChild(canDuck);
        duck.AddChild(startDucking);

        //choose attack or overwatch
        Sequence tactic = new Sequence("Tactic");
        Leaf chooseAttackOrOverwatch = new Leaf("Choose Attack Or Overwatch", ChooseAttackOrOverwatch);

        Selector choose = new Selector("Choose");
        choose.AddChild(attack);
        choose.AddChild(overwatch);

        //add attack and overwatch sequence to tactic selector
        tactic.AddChild(chooseAttackOrOverwatch);
        tactic.AddChild(choose);

        //add sequences to state selector
        Selector state = new Selector("State");
        state.AddChild(idle);
        state.AddChild(duck);
        state.AddChild(cover);
        //state.AddChild(attack);
        state.AddChild(tactic);
        state.AddChild(pursuit);

        //add selector to behaviour tree
        //tree.AddChild(computeTarget);
        tree.AddChild(state);

        //print out total states
        tree.PrintTree();
    }

    private void Update()
    {
        if (unit.GetComponent<TacticMove>().attacking) return;
        treeStatus = tree.Process();
    }

    #region General
    public Node.Status ChangeTurnOnFailure()
    {
        if (!unit.gameObject.GetComponent<TacticMove>().turn || unit.GetUnitPoints() <= 0)
        {
            TurnManager.EndTurn();
            return Node.Status.FAILURE;
        }
        return Node.Status.SUCCESS;
    }

    public Node.Status ComputeTarget()
    {
        if (!unit.gameObject.GetComponent<TacticMove>().turn)
        {
            return Node.Status.FAILURE;
        }
        //if (!enemyMovement.GetTargetTile(gameObject).isCover) unit.isDucking = false;
        enemyMovement.ComputeTarget();
        return Node.Status.SUCCESS;
    }
    #endregion

    #region Behaviours

    #region Idle
    public Node.Status Idle()
    {
        if(unit.gameObject.GetComponent<TacticMove>().turn)
        {
            return Node.Status.FAILURE;
        }
        if (unit.isDucking)
        {
            unit.isDucking = false;
        }
        return Node.Status.SUCCESS;
    }
    #endregion

    #region Attack

    public Node.Status ChooseAttackOrOverwatch()
    {
        if (unit.overwatchCooldown > 0 || unit.duckingCooldown > 0) return Node.Status.FAILURE;
        Debug.Log("Choosing Attack Or Overwatch");
        random = UnityEngine.Random.Range(1, 2);
        Debug.Log($"Random: {random}");
        return Node.Status.SUCCESS;
    }

    public Node.Status CanAttack()
    {
        if (random != 1) return Node.Status.FAILURE;

        if (Vector3.Distance(this.gameObject.transform.position, enemyMovement.Player.transform.position) > enemyMovement.MoveTile || unit.isDucking || unit.overwatchCooldown > 0)
        {
            return Node.Status.FAILURE;
        }

        if (unit.interrupted) return Node.Status.RUNNING;

        Debug.Log("Is Checking Attack");
        return Node.Status.SUCCESS;
    }

    public Node.Status AttackPlayer()
    {
        if (enemyMovement.attacking == false)
        {
            //enemyMovement.attacking = true;
            StartCoroutine(enemyMovement.Shoot(enemyMovement.Player.gameObject.GetComponent<Grid>(), () => unit.DeductPointsOrChangeTurn(unit.GetUnitPoints())));
            return Node.Status.SUCCESS;
        }
        return Node.Status.FAILURE;
    }

    public Node.Status Overwatch()
    {
        if (enemyMovement.attacking == false && random == 2)
        {
            Debug.Log("Enemy Overwatch");
            //enemyMovement.attacking = true;
            //unit.isOverwatch = true;
            unit.Activate(() =>
            {
                unit.isOverwatch = true;
                unit.overwatchCooldown = 2;
            });
            Debug.Log($"AP: {unit.GetUnitPoints()}");
            return Node.Status.SUCCESS;
        }
        return Node.Status.FAILURE;
    }

    #endregion

    #region Pursuit
    public Node.Status StartPursuit()
    {
        //if (unit.isOverwatch) return Node.Status.FAILURE;
        enemyMovement.FindNearestPlayer();
        enemyMovement.SetTargetAndMoveCondition(enemyMovement.Player, false);
        return Node.Status.SUCCESS;
    }

    public Node.Status GoToTarget()
    {
        Debug.Log($"AP: {unit.GetUnitPoints()}");
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
        if (unit.HealthPercentage >= 50 || enemyMovement.GetTargetTile(gameObject).isCover)
        {
            return Node.Status.FAILURE;
        }
        enemyMovement.FindNearestPlayer();
        enemyMovement.FindClosestCoverPosition(out Cover closestCover);

        if (closestCover == null)
        {
            Debug.Log("No Close Cover");
            return Node.Status.FAILURE;
        }

        enemyMovement.SetTargetAndMoveCondition(enemyMovement.Cover, true);
        return Node.Status.SUCCESS;
    }

    #endregion

    #region Duck

    private Node.Status CanDuck()
    {
        if (/*unit.HealthPercentage <= 20 && */enemyMovement.GetTargetTile(gameObject).isCover && !unit.isDucking)
        {
            Debug.Log("Ducking");
            return Node.Status.SUCCESS;
        }
        return Node.Status.FAILURE;
    }

    private Node.Status StartDucking()
    {
        if (!unit.isDucking)
        {
            unit.Activate(() => unit.isDucking = true);
            //unit.DeductPointsOrChangeTurn(unitPointSystem.CurrentPoints);
            return Node.Status.SUCCESS;
        }
        return Node.Status.FAILURE;
    }
    #endregion

    #endregion
}
