using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : TacticMove
{
    [SerializeField] GameObject player;
    bool hasFoundTargetGrid = false;
    int closestGridToPlayerIndex = 0;
    float distanceOfClosestGridToPlayer = 0;

    UnitPoitsSystem unitPoints;

    // Start is called before the first frame update
    void Start()
    {
        unitPoints = GetComponent<UnitPoitsSystem>();
        TurnManager.AddUnit(this);
        unit = GetComponent<Unit>();
        //gameObject.GetComponent<MeshRenderer>().enabled = false;
        //player = GameObject.FindGameObjectWithTag("Player");
    }

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
            FindNearestTarget();
            CalculatePath();
            FindSelectableGrid();
            if (actualTargetGrid != null)
            {
                Debug.Log($"actualTargetGrid: {actualTargetGrid.name}");
                actualTargetGrid.target = true;
            }
            //FindSelectableGrid();
            //SearchClosestGridToPlayer();
        } 
        //PlayerMove();
    }

    public void EnemyMove()
    {
        //Debug.DrawRay(transform.position, transform.forward);

        //if (!unit.isCurrentTurn) return;

        //if (!moving)
        //{
        //    FindNearestTarget();
        //    CalculatePath();
        //    FindSelectableGrid();
        //    if(actualTargetGrid != null)
        //    {
        //        Debug.Log($"actualTargetGrid: {actualTargetGrid.name}");
        //        actualTargetGrid.target = true;
        //    }
        //    //FindSelectableGrid();
        //    //SearchClosestGridToPlayer();
        //}
        //else
        //{
        //    Move(() => {
        //        //hasFoundTargetGrid = false;
        //        unit.DeductPointsOrChangeTurn(1);
        //    });
        //}
        if(moving)
        {
            Move(() => {
                //hasFoundTargetGrid = false;
                unit.DeductPointsOrChangeTurn(1);
            });
        }
    }

    private void SearchClosestGridToPlayer()
    {
        if (!hasFoundTargetGrid)
        {
            closestGridToPlayerIndex = 0;
            distanceOfClosestGridToPlayer = 0;

            for (int i = 0; i < SelectableGrids.Count; i++)
            {
                if (i <= 0)
                {
                    distanceOfClosestGridToPlayer = Vector3.Distance(SelectableGrids[i].transform.position, player.transform.position);
                }
                else
                {
                    if (Vector3.Distance(SelectableGrids[i].transform.position, player.transform.position) < distanceOfClosestGridToPlayer)
                    {
                        distanceOfClosestGridToPlayer = Vector3.Distance(SelectableGrids[i].transform.position, player.transform.position);
                        closestGridToPlayerIndex = i;
                    }
                }
            }

            hasFoundTargetGrid = true;
        }
        else
        {
            //Debug.Log($"Closest Grid to Player Distance: {distanceOfClosestGridToPlayer}");
            MoveToGrid(SelectableGrids[closestGridToPlayerIndex]);
        }
    }

    void CalculatePath()
    {
        Grid targetGrid = GetTargetTile(player);
        FindPath(targetGrid);
    }

    public Unit Player
    {
        get => player.GetComponent<Unit>();
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
}
