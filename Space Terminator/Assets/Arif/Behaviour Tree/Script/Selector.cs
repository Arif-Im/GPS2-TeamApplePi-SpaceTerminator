using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : Node
{
    public Selector(string n)
    {
        name = n;
    }

    public override Status Process()
    {
        // My Answer
        //Status childStatus = children[currentChild].Process();

        //Debug.Log($"Current Sequence: {children[currentChild].name}\nCurrent Status: {childStatus}");

        //if (childStatus == Status.SUCCESS)
        //{
        //    return Status.SUCCESS;
        //}
        //else
        //{
        //    currentChild++;
        //}

        //return Status.RUNNING;

        // Correct Answer
        Status childStatus = children[currentChild].Process();

        //Debug.Log($"Current Sequence: {children[currentChild].name}\nCurrent Status: {childStatus}");

        if (childStatus == Status.RUNNING) return Status.RUNNING;

        if (childStatus == Status.SUCCESS)
        {
            //Must be 0 for the next call of Process
            currentChild = 0;
            return Status.SUCCESS;
        }

        currentChild++;

        if (currentChild >= children.Count)
        {
            currentChild = 0;
            //Must return failure since none of the children are successful
            return Status.FAILURE;
        }

        return Status.RUNNING;
    }
}
