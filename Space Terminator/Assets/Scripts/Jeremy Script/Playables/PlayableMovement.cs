using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableMovement : TacticMove
{
    Grid enemy;


    // Start is called before the first frame update
    new void Start()
    {
        unit = GetComponent<Unit>();
        //gameObject.GetComponent<MeshRenderer>().enabled = false;
        TurnManager.AddUnit(this);
    }

    // Update is called once per frame
    void Update()
    {
        //if (!unit.isCurrentTurn) return;

        PlayerMove();
    }

    public virtual void PlayerMove()
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
            TurnManager.EndTurn();
            return;
        }

        if (!turn)
            return;
        else
        {
            if(unit.isDucking)
            {
                unit.isDucking = false;
            }
        }

        if (!moving)
        {
            FindSelectableGrid();
            if (GameObject.FindGameObjectWithTag("Turn Manager").GetComponent<TurnManager>().attackState == AttacksState.Idle)
            {
                //Debug.Log("Move");
                CheckInput();

            }
        }
        else
        {
            Move(() => {
                //unit.isDucking = false;
                unit.DeductPointsOrChangeTurn(1);
            });
        }
    }

    void CheckInput()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        //mouse click for now to test, me hates mobile testing
        if (Input.GetMouseButtonDown(1))
        {
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "Grid")
                {
                    Grid g = hit.collider.GetComponent<Grid>();

                    if (g.selectable)
                    {
                        MoveToGrid(g);
                    }
                }
            }
        }

        if(unit.overwatchCooldown <= 0 || unit.duckingCooldown <= 0)
        {
            if (Physics.Raycast(ray, out hit) && Input.GetMouseButtonDown(1))
            {
                if (hit.collider.tag == "Alien" && !attacking)
                {
                    Grid g = hit.collider.GetComponent<Grid>();

                    if (g.selectable)
                    {
                        Debug.Log("Shoot");
                        //attacking = true;
                        //StartCoroutine(Shoot(g));
                        enemy = g;
                        InitiateAttack();

                    }
                }
            }
            if (GetTargetTile(gameObject).isCover)
            {
                if (Input.GetKeyDown(KeyCode.D))
                {
                    unit.Activate(() => unit.isDucking = true);
                }
            }

            if (Input.GetKeyDown(KeyCode.O))
            {
                unit.Activate(() =>
                {
                    unit.isOverwatch = true;
                    unit.overwatchCooldown = 2;
                });
                //unit.isOverwatch = true;
                //unit.DeductPointsOrChangeTurn(1);
            }
        }
    }

    public void InitiateAttack()
    {
        if (unit.isDucking || unit.duckingCooldown > 0) return;
        attacking = true;
        StartCoroutine(Shoot(enemy, () => unit.DeductPointsOrChangeTurn(unit.GetUnitPoints())));
    }
}
