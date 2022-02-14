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
        tree = new BehaviourTree();

        Sequence attack = new Sequence("Attack");
        Leaf canAttackPlayer = new Leaf("Can Attack Player", CanAttack);
        Leaf goToPlayer = new Leaf("Go To Player", GoToPlayer);
        Leaf attackPlayer = new Leaf("Attack Player", AttackPlayer);

        attack.AddChild(canAttackPlayer);
        attack.AddChild(goToPlayer);
        attack.AddChild(attackPlayer);

        Sequence idle = new Sequence("Idle");
        Leaf stayIdle = new Leaf("Stay Idle", Idle);

        idle.AddChild(stayIdle);

        Selector state = new Selector("State");

        state.AddChild(idle);
        state.AddChild(attack);

        tree.AddChild(state);

        tree.PrintTree();
    }

    public Node.Status Idle()
    {
        if(unit.isCurrentTurn)
        {
            return Node.Status.FAILURE;
        }
        return Node.Status.SUCCESS;
    }

    public Node.Status CanAttack()
    {
        if(!unit.isCurrentTurn)
        {
            return Node.Status.FAILURE;
        }
        return Node.Status.SUCCESS;
    }

    public Node.Status GoToPlayer()
    {
        if (!enemyMovement.Moving && enemyMovement.Path.Count <= 0)
        {
            return Node.Status.SUCCESS;
        }
        else if(!enemyMovement.Moving && enemyMovement.Path.Count > 0)
        {
            return Node.Status.FAILURE;
        }
        else
        {
            return Node.Status.RUNNING;
        }
    }

    public Node.Status AttackPlayer()
    {
        if(enemyMovement.Player.currentHealth > 0)
        {
            enemyMovement.Player.currentHealth -= 2;
            return Node.Status.SUCCESS;
        }
        return Node.Status.FAILURE;
    }

    private void Update()
    {
        //if (treeStatus != Node.Status.SUCCESS)
            treeStatus = tree.Process();
    }
}
