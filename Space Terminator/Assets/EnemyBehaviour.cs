using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    protected BehaviourTree tree;
    protected EnemyMovement enemyMovement;
    protected Unit unit;
    //UnitPoitsSystem unitPointSystem;

    protected int attackOrOverwatch = 1;
    protected int tacticOrDuck = 1;

    protected Node.Status treeStatus = Node.Status.RUNNING;

    private void Awake()
    {
        Initialize();
    }

    public virtual void Initialize()
    {
        attackOrOverwatch = 1;
        unit = GetComponent<Unit>();
        //unitPointSystem = GetComponent<UnitPoitsSystem>();
        enemyMovement = GetComponent<EnemyMovement>();
    }

    private void SetTimeScale(float scale)
    {

        Time.timeScale = scale;
        Time.fixedDeltaTime = scale * 0.02f;

    }

    private void Start()
    {
        InitializeTree();
    }

    public virtual void InitializeTree()
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
        state.AddChild(tactic);
        state.AddChild(pursuit);

        //add selector to behaviour tree
        tree.AddChild(state);

        //print out total states
        tree.PrintTree();
    }

    protected void Update()
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
        enemyMovement.FindNearestPlayer();
        attackOrOverwatch = UnityEngine.Random.Range(1, 3);
        return Node.Status.SUCCESS;
    }

    public Node.Status CanAttack()
    {
        if (attackOrOverwatch != 1 || tacticOrDuck != 1) return Node.Status.FAILURE;

        if (Vector3.Distance(this.gameObject.transform.position, enemyMovement.Player.transform.position) > enemyMovement.MoveTile || unit.isDucking || unit.overwatchCooldown > 0)
        {
            return Node.Status.FAILURE;
        }

        if (unit.interrupted) return Node.Status.RUNNING;

        return Node.Status.SUCCESS;
    }

    public Node.Status AttackPlayer()
    {
        if (enemyMovement.attacking == false)
        {
            StartCoroutine(enemyMovement.Shoot(enemyMovement.Player.gameObject.GetComponent<Grid>(), () => unit.DeductPointsOrChangeTurn(unit.GetUnitPoints())));
            return Node.Status.SUCCESS;
        }
        return Node.Status.FAILURE;
    }

    public Node.Status Overwatch()
    {
        if (enemyMovement.attacking == false && attackOrOverwatch == 2)
        {
            unit.Activate(() =>
            {
                unit.isOverwatch = true;
                unit.overwatchCooldown = 2;
            });
            return Node.Status.SUCCESS;
        }
        return Node.Status.FAILURE;
    }

    #endregion

    #region Pursuit
    public Node.Status StartPursuit()
    {
        enemyMovement.FindNearestPlayer();
        enemyMovement.SetTargetAndMoveCondition(enemyMovement.Player, false);
        return Node.Status.SUCCESS;
    }

    public Node.Status GoToTarget()
    {
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
            return Node.Status.FAILURE;
        }

        enemyMovement.SetTargetAndMoveCondition(enemyMovement.Cover, true);
        return Node.Status.SUCCESS;
    }

    #endregion

    #region Duck

    public Node.Status CanDuck()
    {
        tacticOrDuck = UnityEngine.Random.Range(1, 3);

        if (tacticOrDuck == 1 && enemyMovement.GetTargetTile(gameObject).isCover && !unit.isDucking)
        {
            return Node.Status.SUCCESS;
        }
        return Node.Status.FAILURE;
    }

    public Node.Status StartDucking()
    {
        if (!unit.isDucking)
        {
            unit.Activate(() => unit.isDucking = true);
            return Node.Status.SUCCESS;
        }
        return Node.Status.FAILURE;
    }
    #endregion

    #endregion
}
