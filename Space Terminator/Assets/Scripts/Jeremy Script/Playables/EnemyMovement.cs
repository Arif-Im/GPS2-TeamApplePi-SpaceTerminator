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
    Animator anim;
    public bool isWalking = false;


    new void Start()
    {
        anim = GetComponent<Animator>();
        unitPoints = GetComponent<UnitPoitsSystem>();
        TurnManager.AddUnit(this);
        unit = GetComponent<Unit>();
        FindNearestPlayer();
        target = player;
    }
    bool isAbsolutePosition = false;

    void Update()
    {
        Debug.DrawRay(transform.position, transform.forward);

        if (unit.Health <= 0)
        {
            GetComponent<Grid>().selectable = false;
            transform.gameObject.tag = "Grid";
            Destroy(gameObject.GetComponent<CapsuleCollider>());
        }

        if (turn && unit.Health <= 0)
        {
            GetComponent<TacticMove>().arrow.SetActive(false);
            TurnManager.EndTurn();
            return;
        }

        if (!turn)
            return;
    }

    public void ComputeTarget()
    {
        if (!moving)
        {
            FindSelectableGrid();
            CalculatePath(isAbsolutePosition);
            if (actualTargetGrid != null)
            {
                actualTargetGrid.target = true;
            }
        }
    }

    public void SetTargetAndMoveCondition(Grid target, bool moveCondition)
    {
        this.target = target.gameObject;
        isAbsolutePosition = moveCondition;
    }

    public void EnemyMove()
    {
        isWalking = true;
        if(moving)
        {
            Move(() => {
                unit.DeductPointsOrChangeTurn(1);
                isWalking = false;
            });
        }
    }

    void CalculatePath(bool isAbsolutePosition)
    {
        if (target == null)
        {
            return;
        }
        Grid targetGrid = GetTargetTile(target);
        FindPath(targetGrid, isAbsolutePosition);
    }

    public Grid Player
    {
        get => player.GetComponent<Grid>();
    }

    public Grid Cover
    {
        get => cover.GetComponent<Grid>();
    }

    public void FindNearestPlayer()
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
    }

    public void FindRandomPosition(out GameObject target)
    {
        GameObject chosenGrid = grids[UnityEngine.Random.Range(0, grids.Length)];
        target = chosenGrid.gameObject;
    }

    public void FindClosestCoverPosition(out Cover closestCover)
    {
        List<Cover> covers = new List<Cover>();

        float closestCoverDistance = Mathf.Infinity;
        closestCover = null;

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

        if (closestCover == null) return;

        List<Grid> coverPositions = closestCover.CoverPositions;
        Vector3 directionOfPlayerFromCover = (player.transform.position - closestCover.transform.position).normalized;

        float safestPositionDot = Mathf.Infinity;
        foreach (Grid coverPosition in coverPositions)
        {
            float dot = Vector3.Dot(coverPosition.GetDirectionOfCover(closestCover.transform.position - new Vector3(0, 1, 0), coverPosition.transform.position), directionOfPlayerFromCover);

            if (dot < 0 && dot < safestPositionDot)
            {
                cover = coverPosition.gameObject;
                safestPositionDot = dot;
            }
        }
    }
}
