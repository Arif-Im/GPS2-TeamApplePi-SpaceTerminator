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
                if(pointSystem.CurrentPoints < 1)
                {
                    battleSystem.ChangeTurn(this.GetComponent<Unit>(), pointSystem.maxPoints);
                }
            });
        }
    }

    void CheckInput()
    {
        //mouse click for now to test, me hates mobile testing
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
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
    }
}
