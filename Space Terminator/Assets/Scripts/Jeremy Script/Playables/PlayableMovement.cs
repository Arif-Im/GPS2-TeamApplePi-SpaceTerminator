using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableMovement : TacticMove
{
    public bool overUI = false;
    protected Grid enemy;
    protected bool activate = false;

    [SerializeField] GameObject cameraHolder;
    [SerializeField] GameObject outOfAmmoText;
    public bool isWalking;

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
            GetComponent<TacticMove>().arrow.SetActive(false);
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
            if (relocateCam)
            {
                StartCoroutine(MoveCamera(transform.position, .5f));
            }
            

            FindSelectableGrid();
            if (GameObject.FindGameObjectWithTag("Turn Manager").GetComponent<TurnManager>().attackState == AttacksState.Idle)
            {
                //Debug.Log("Move");
                CheckInput();

            }
        }
        else
        {
            isWalking = true;
            Move(() => {
                unit.DeductPointsOrChangeTurn(1);
                isWalking = false;
            });
        }
    }

    public virtual void CheckInput()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        //if(!overUI)
        //{
        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        if (Physics.Raycast(ray, out hit))
        //        {
        //            if (hit.collider.tag == "Grid")
        //            {
        //                Grid g = hit.collider.GetComponent<Grid>();

        //                if (g.selectable)
        //                {
        //                    MoveToGrid(g);
        //                }
        //            }
        //        }
        //    }
        //}
        if (Input.GetMouseButtonDown(0))
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

        if (activate)
        {
            if (Physics.Raycast(ray, out hit) && Input.GetMouseButtonDown(0))
            {
                if (hit.collider.tag == "Alien" && !attacking)
                {
                    Grid g = hit.collider.GetComponent<Grid>();

                    if (g.selectable)
                    {
                        if (ammoCount >= 0)
                        {
                            Debug.Log("Initiate Attack");
                            enemy = g;
                            InitiateAttack();
                            //attacking = true;

                        }
                        else
                        {
                            //Debug.Log("No Ammo");
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
                //else
                //{
                //    Debug.LogError("Still Not Found...");
                //}
            }
        }
    }

    public void SetEnemy()
    {
        //Debug.Log("Activate = true");
        activate = true;
    }

    public void InitiateAttack()
    {
        if (attacking)
        {
            //Debug.Log("Still Attacking...");
            return;
        }
        StartCoroutine(Shoot(enemy, () =>
        {
            enemy = null;
            activate = false;
            unit.DeductPointsOrChangeTurn(unit.GetUnitPoints());
        }));
    }

    public void InitiatePunch()
    {
        if (attacking) return;
        StartCoroutine(Punch(enemy, () =>
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
