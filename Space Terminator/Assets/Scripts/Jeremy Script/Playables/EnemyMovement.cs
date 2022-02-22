using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : PlayableMovement
{
    [SerializeField] GameObject player;
    bool hasFoundTargetGrid = false;
    int closestGridToPlayerIndex = 0;
    float distanceOfClosestGridToPlayer = 0;

    // Start is called before the first frame update
    void Start()
    {
        unit = GetComponent<Unit>();
        //player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (!unit.isCurrentTurn) return;
        PlayerMove();
    }

    public override void PlayerMove()
    {
        Debug.DrawRay(transform.position, transform.forward);

        if (!moving)
        {
            FindSelectableGrid();
            SearchClosestGridToPlayer();
        }
        else
        {
            Move(() => {
                hasFoundTargetGrid = false;
                if (pointSystem.CurrentPoints < 1)
                {
                    battleSystem.ChangeTurn(this.GetComponent<Unit>(), pointSystem.maxPoints);
                }
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

    public Unit Player
    {
        get => player.GetComponent<Unit>();
    }
}
