using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoutMovement : PlayableMovement
{
    Scout scout;
    public bool grenadeMode = false;

    // Start is called before the first frame update
    new void Start()
    {
        unit = GetComponent<Unit>();
        scout = GetComponentInChildren<Scout>();
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

        if (grenadeMode)
        {
            if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit) && hit.collider.tag == "Grid")
            {
                Grid g = hit.collider.GetComponent<Grid>();

                if (g.isTouched == false)
                {
                    if (choosenGrid == null)
                    {
                        choosenGrid = g;
                    }
                    choosenGrid.isTouched = false;
                    g.isTouched = true;
                    choosenGrid = g;
                }
                else
                {
                    if (g.selectable)
                    {
                        scout.Activate(g.gameObject, true, this);
                    }
                    grenadeMode = false;
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit) && hit.collider.tag == "Grid")
            {
                Grid g = hit.collider.GetComponent<Grid>();

                if (g.isTouched == false)
                {
                    if (choosenGrid == null)
                    {
                        choosenGrid = g;
                    }
                    choosenGrid.isTouched = false;
                    g.isTouched = true;
                    choosenGrid = g;
                }
                else
                {
                    if (g.selectable)
                    {
                        MoveToGrid(g);
                    }
                }
            }

            if (activate)
            {
                if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit) && hit.collider.tag == "Alien" && !attacking)
                {
                    Grid g = hit.collider.GetComponent<Grid>();

                    if (g.isTouched == false)
                    {
                        if (choosenGrid == null)
                        {
                            choosenGrid = g;
                        }
                        choosenGrid.isTouched = false;
                        g.isTouched = true;
                        choosenGrid = g;
                    }
                    else
                    {
                        if (g.selectable)
                        {
                            if (ammoCount >= 0)
                            {
                                Debug.Log("Initiate Attack");
                                enemy = g;
                                InitiateAttack();

                            }
                            else
                            {
                                if (Vector3.Distance(g.transform.position, transform.position) <= 1.2f)
                                {
                                    InitiatePunch();
                                }
                                else
                                {
                                    Instantiate(outOfAmmoText, transform.position, Quaternion.identity, transform);
                                }
                            }
                        }
                        else
                        {
                            Debug.Log("G not selectable");
                        }
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
