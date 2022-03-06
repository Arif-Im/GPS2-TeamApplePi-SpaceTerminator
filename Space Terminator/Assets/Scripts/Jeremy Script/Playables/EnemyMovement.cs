using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : TacticMove
{
    [SerializeField] GameObject player;
    private GameObject cover;

    private GameObject target;

    UnitPoitsSystem unitPoints;

    // Start is called before the first frame update
    void Start()
    {
        unitPoints = GetComponent<UnitPoitsSystem>();
        TurnManager.AddUnit(this);
        unit = GetComponent<Unit>();
        //gameObject.GetComponent<MeshRenderer>().enabled = false;
        //player = GameObject.FindGameObjectWithTag("Player");
        //FindNearestTarget();
    }
    bool isAbsolutePosition = false;

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position, transform.forward);

        //if (!unit.isCurrentTurn) return;

        if (unit.Health <= 0)
        {
            GetComponent<Grid>().selectable = false;
            transform.gameObject.tag = "Grid";
            Destroy(gameObject.GetComponent<CapsuleCollider>());
        }

        if (turn && unit.Health <= 0)
        {
            TurnManager.EndTurn();
            return;
        }

        if (!turn)
            return;

        if (!moving)
        {
            FindSelectableGrid();
            FindNearestTarget();
            FindClosestCoverPosition();

            if (unit.HealthPercentage > 50)
            {
                target = player;
                isAbsolutePosition = false;
            }
            else
            {
                target = cover;
                isAbsolutePosition = true;
            }

            CalculatePath(isAbsolutePosition);
            if (actualTargetGrid != null)
            {
                actualTargetGrid.target = true;
            }
        } 
    }

    public void EnemyMove()
    {
        if(moving)
        {
            Move(() => {
                //hasFoundTargetGrid = false;
                unit.DeductPointsOrChangeTurn(1);
            });
        }
    }

    void CalculatePath(bool isAbsolutePosition)
    {
        Grid targetGrid = GetTargetTile(target);
        FindPath(targetGrid, isAbsolutePosition);
    }

    public Grid Player
    {
        get => player.GetComponent<Grid>();
    }

    public void FindNearestTarget()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");
        GameObject nearest = null;
        float dist = Mathf.Infinity;
        foreach(GameObject obj in targets)
        {
            float d = Vector3.Distance(transform.position, obj.transform.position);
            if(d < dist)
            {
                dist = d;
                nearest = obj;
            }
        }
        player = nearest;
        //Debug.Log($"Player: {player.name}");
    }

    public void FindClosestCoverPosition()
    {
        List<Cover> covers = new List<Cover>();

        float closestCoverDistance = Mathf.Infinity;
        Cover closestCover = null;

        foreach (GameObject cover in GameObject.FindGameObjectsWithTag("Cover"))
        {
            covers.Add(cover.GetComponent<Cover>());
        }

        foreach (Cover cover in covers)
        {
            if (Vector3.Distance(transform.position, cover.transform.position) < closestCoverDistance)
            {
                closestCover = cover;
                closestCoverDistance = Vector3.Distance(transform.position, cover.transform.position);
            }
        }

        List<Grid> coverPositions = closestCover.CoverPositions;
        Vector3 directionOfPlayerFromCover = (player.transform.position - closestCover.transform.position).normalized;

        float safestPositionDot = Mathf.Infinity;
        foreach (Grid coverPosition in coverPositions)
        {
            float dot = Vector3.Dot(coverPosition.GetDirectionOfCover(closestCover.transform.position - new Vector3(0, 1, 0), coverPosition.transform.position), directionOfPlayerFromCover);

            if (dot < 0 && dot < safestPositionDot)
            {
                if (!coverPosition.GetComponent<Grid>().playerPresent)
                {
                    cover = coverPosition.gameObject;
                    safestPositionDot = dot;
                    //Debug.Log($"Cover name: {cover.name}");
                }
            }
        }
    }
}
