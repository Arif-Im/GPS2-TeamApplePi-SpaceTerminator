using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableMovement : TacticMove
{
    protected Grid enemy;
    protected bool activate = false;


    // Start is called before the first frame update
    void Start()
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
                unit.DeductPointsOrChangeTurn(1);
            });
        }
    }

    public virtual void CheckInput()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
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

        if(activate)
        {
            if (Physics.Raycast(ray, out hit) && Input.GetMouseButtonDown(0))
            {
                if (hit.collider.tag == "Alien" && !attacking)
                {
                    Grid g = hit.collider.GetComponent<Grid>();

                    if (g.selectable)
                    {
                        enemy = g;
                        InitiateAttack();
                    }
                }
            }
        }
    }

    public void SetEnemy()
    {
        activate = true;
    }

    public void InitiateAttack()
    {
        if (attacking) return;
        StartCoroutine(Shoot(enemy, () =>
        {
            enemy = null;
            activate = false;
            unit.DeductPointsOrChangeTurn(unit.GetUnitPoints());
        }));
    }

    public override void BeginTurn()
    {
        base.BeginTurn();
        ButtonManager.instance.SetButtonsToCurrentPlayer(this);
    }
}
