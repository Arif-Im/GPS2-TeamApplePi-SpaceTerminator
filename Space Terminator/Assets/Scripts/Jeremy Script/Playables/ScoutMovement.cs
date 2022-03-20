using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoutMovement : PlayableMovement
{
    Scout scout;
    public bool grenadeMode = false;

    // Start is called before the first frame update
    void Start()
    {
        unit = GetComponent<Unit>();
        scout = GetComponentInChildren<Scout>();
        //gameObject.GetComponent<MeshRenderer>().enabled = false;
        TurnManager.AddUnit(this);
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMove();
    }

    public override void CheckInput()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(grenadeMode)
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.GetComponent<Grid>())
                    {
                        Grid g = hit.collider.GetComponent<Grid>();

                        if (g.selectable)
                        {
                            scout.Activate(g.gameObject, true, this);
                            //unit.DeductPointsOrChangeTurn(1);
                        }
                    }
                    grenadeMode = false;
                }
            }
        }
        else
        {
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

            if (Physics.Raycast(ray, out hit) && Input.GetMouseButtonDown(1))
            {
                if (hit.collider.tag == "Alien" && !attacking)
                {
                    Grid g = hit.collider.GetComponent<Grid>();

                    if (g.selectable)
                    {
                        enemy = g;
                    }
                }
            }
        }
    }

    public void ActivateGrenadeMode()
    {
        grenadeMode = true;
    }
}
