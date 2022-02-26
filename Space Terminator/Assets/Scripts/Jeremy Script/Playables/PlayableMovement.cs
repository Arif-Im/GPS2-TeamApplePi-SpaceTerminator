using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableMovement : TacticMove
{

    // Start is called before the first frame update
    void Start()
    {
        unit = GetComponent<Unit>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!unit.isCurrentTurn) return;
        PlayerMove();
    }

    public virtual void PlayerMove()
    {
        Debug.DrawRay(transform.position, transform.forward);

        if (!moving)
        {
            FindSelectableGrid();
            CheckInput();
        }
        else
        {
            Move(() => {
                unit.DeductPointsOrChangeTurn(1);
            });
        }
    }

    void CheckInput()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        //mouse click for now to test, me hates mobile testing
        if (Input.GetMouseButtonUp(0))
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
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag == "Alien" && !attacking)
            {
                Grid g = hit.collider.GetComponent<Grid>();

                if (g.selectable)
                {
                    attacking = true;
                    StartCoroutine(Shoot(g));
                }
            }
        }
    }
}
