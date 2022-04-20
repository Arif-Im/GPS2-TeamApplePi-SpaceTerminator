using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfinedEnemyBehaviour : EnemyBehaviour
{
    [SerializeField] List<GameObject> roomList;

    private void Awake()
    {
        Initialize();
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializeTree();
    }

    public override void InitializeTree()
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

        //Patrol sequence
        Sequence patrol = new Sequence("Patrol");
        Leaf isPlayerNearby = new Leaf("Is Player Nearby", IsPlayerFar);
        Leaf startPatrol = new Leaf("Start Patrol", StartPatrol);

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
        attack.AddChild(canAttackPlayer);
        attack.AddChild(computeTarget);
        attack.AddChild(attackPlayer);

        //add leaves to overwatch sequence
        overwatch.AddChild(startOverwatch);

        //add leaves to pursuit sequence
        pursuit.AddChild(startPursuit);
        pursuit.AddChild(computeTarget);
        pursuit.AddChild(goToTarget);

        //add leaves to cover sequence
        cover.AddChild(lowHealth);
        cover.AddChild(computeTarget);
        cover.AddChild(goToTarget);

        //add leaves to duck sequence
        duck.AddChild(canDuck);
        duck.AddChild(computeTarget);
        duck.AddChild(startDucking);

        patrol.AddChild(isPlayerNearby);
        patrol.AddChild(startPatrol);
        patrol.AddChild(computeTarget);
        patrol.AddChild(goToTarget);

        //choose attack or overwatch
        Sequence tactic = new Sequence("Tactic");
        Leaf chooseAttackOrOverwatch = new Leaf("Choose Attack Or Overwatch", ChooseAttackOrOverwatch);

        Selector chooseTactic = new Selector("Choose Tactic");
        chooseTactic.AddChild(attack);
        chooseTactic.AddChild(overwatch);

        //add attack and overwatch sequence to tactic selector
        tactic.AddChild(chooseAttackOrOverwatch);
        tactic.AddChild(chooseTactic);
        tactic.AddChild(pursuit);

        Sequence move = new Sequence("Move");

        Selector chooseMove = new Selector("Choose Move");
        chooseMove.AddChild(patrol);
        chooseMove.AddChild(tactic);

        move.AddChild(chooseMove);

        //add sequences to state selector
        Selector state = new Selector("State");
        state.AddChild(idle);
        state.AddChild(duck);
        state.AddChild(cover);
        state.AddChild(move);

        //add selector to behaviour tree
        tree.AddChild(state);

        //print out total states
        tree.PrintTree();
    }

    public Node.Status IsPlayerFar()
    {
        enemyMovement.FindNearestPlayer();
        if (Vector3.Distance(transform.position, enemyMovement.Player.transform.position) > 5)
        {
            return Node.Status.SUCCESS;
        }
        return Node.Status.FAILURE;
    }

    public Node.Status StartPatrol()
    {
        Collider[] gridColliders = Physics.OverlapSphere(transform.position, 5, enemyMovement.whatIsGrid);
        List<GameObject> gridGameObjects = new List<GameObject>();

        foreach (Collider gridCollider in gridColliders)
        {
            for (int i = 0; i < roomList.Count; i++)
            {
                if (gridCollider.gameObject == roomList[i])
                    gridGameObjects.Add(gridCollider.gameObject);
            }
        }

        GameObject target = enemyMovement.FindRandomPosition(gridGameObjects);

        if (target == null)
        {
            Debug.Log("target null");
            return Node.Status.RUNNING;
        }
        else
        {
            enemyMovement.SetTargetAndMoveCondition(target.GetComponent<Grid>(), false);
            return Node.Status.SUCCESS;
        }
    }
}
