using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RobberBehaviour : MonoBehaviour
{
    BehaviourTree tree;
    public GameObject diamond;
    public GameObject van;
    public GameObject backDoor;
    public GameObject frontDoor;
    NavMeshAgent agent;

    public enum ActionState { IDLE, WORKING }
    ActionState state = ActionState.IDLE;

    [Range(0, 1000)]
    public int money = 800;

    Node.Status treeStatus = Node.Status.RUNNING;

    private void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();

        tree = new BehaviourTree();
        Sequence steal = new Sequence("Steal Something");
        Leaf hasGotMoney = new Leaf("Has Got Money", HasMoney);
        Leaf goToDiamond = new Leaf("Go To Diamond", GoToDiamond);
        Leaf goToBackDoor = new Leaf("Go To Back Door", GoToBackDoor);
        Leaf goToFrontDoor = new Leaf("Go To Front Door", GoToFrontDoor);
        Leaf goToVan = new Leaf("Go To Van", GoToVan);
        Selector openDoor = new Selector("Open Door");

        openDoor.AddChild(goToFrontDoor);
        openDoor.AddChild(goToBackDoor);

        steal.AddChild(hasGotMoney);
        steal.AddChild(openDoor);
        steal.AddChild(goToDiamond);
        //steal.AddChild(goToBackDoor);
        steal.AddChild(goToVan);
        tree.AddChild(steal);

        tree.PrintTree();
    }

    public Node.Status HasMoney()
    {
        if(money >= 500)
        {
            return Node.Status.FAILURE;
        }
        return Node.Status.SUCCESS;
    }

    public Node.Status GoToDiamond()
    {
        //My Answer
        //return StealDiamond(diamond);

        //Tutorial Answer
        Node.Status s = GoToLocation(diamond.transform.position);
        if (s == Node.Status.SUCCESS)
        {
            diamond.transform.parent = this.gameObject.transform;
        }
        return s;
    }

    public Node.Status GoToVan()
    {
        //My Answer
        //return GoToLocation(diamond, van.transform.position);

        //Tutorial Answer
        Node.Status s = GoToLocation(van.transform.position);
        if (s == Node.Status.SUCCESS)
        {
            money += 300;
            diamond.SetActive(false);
        }
        return s;
    }

    public Node.Status GoToBackDoor()
    {
        return GoToDoor(backDoor);
    }

    public Node.Status GoToFrontDoor()
    {
        return GoToDoor(frontDoor);
    }

    public Node.Status GoToDoor(GameObject door)
    {
        Node.Status s = GoToLocation(door.transform.position);
        if (s == Node.Status.SUCCESS)
        {
            if (!door.GetComponent<Lock>().isLocked)
            {
                door.SetActive(false);
                return Node.Status.SUCCESS;
            }
            return Node.Status.FAILURE;
        }
        else
            return s;
    }

    public Node.Status StealDiamond(GameObject diamond)
    {
        Node.Status s = GoToLocation(diamond.transform.position);
        if (s == Node.Status.SUCCESS)
        {
            if (diamond.GetComponent<Stealable>() != null)
            {
                diamond.transform.parent = transform;
                diamond.transform.localPosition = new Vector3(0, 1, 0);
                return Node.Status.SUCCESS;
            }
            return Node.Status.FAILURE;
        }
        else
            return s;
    }

    Node.Status GoToLocation(Vector3 destination)
    {
        float distanceToTarget = Vector3.Distance(destination, this.transform.position);
        if(state == ActionState.IDLE)
        {
            agent.SetDestination(destination);
            state = ActionState.WORKING;
        }
        else if(Vector3.Distance(agent.pathEndPosition, destination) >= 2)
        {
            state = ActionState.IDLE;
            return Node.Status.FAILURE;
        }
        else if(distanceToTarget < 2)
        {
            state = ActionState.IDLE;
            return Node.Status.SUCCESS;
        }
        return Node.Status.RUNNING;
    }

    Node.Status GoToLocation(GameObject diamond, Vector3 destination)
    {
        float distanceToTarget = Vector3.Distance(destination, this.transform.position);
        if (state == ActionState.IDLE)
        {
            agent.SetDestination(destination);
            state = ActionState.WORKING;
        }
        else if (Vector3.Distance(agent.pathEndPosition, destination) >= 2)
        {
            state = ActionState.IDLE;
            return Node.Status.FAILURE;
        }
        else if (distanceToTarget < 2)
        {
            state = ActionState.IDLE;
            Destroy(diamond);
            money += 300;
            return Node.Status.SUCCESS;
        }
        return Node.Status.RUNNING;
    }

    private void Update()
    {
        if(treeStatus != Node.Status.SUCCESS)
            treeStatus = tree.Process();
    }
}
