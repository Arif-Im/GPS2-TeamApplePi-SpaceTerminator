using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayableMovement : TacticMove
{
    protected Grid enemy;
    protected bool activate = false;

    [Header("UI")]
    [SerializeField] GameObject cameraHolder;
    [SerializeField] protected GameObject outOfAmmoText;
    public bool isWalking;
    private bool playOnce = false;

    protected Grid choosenGrid;

    // Start is called before the first frame update
    new void Start()
    {
        unit = GetComponent<Unit>();
        TurnManager.AddUnit(this);
    }

    // Update is called once per frame
    void Update()
    {
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

            if (turn)
            {
                TurnManager.EndTurn();
                turn = false;
            }

            //if (turn)
            //{
            //    GetComponent<TacticMove>().arrow.SetActive(false);
            //    unit.DeductPointsOrChangeTurn(unit.GetUnitPoints());
            //    return;
            //}
        }

        //if (turn && unit.Health <= 0)
        //{
        //    GetComponent<TacticMove>().arrow.SetActive(false);
        //    unit.DeductPointsOrChangeTurn(unit.GetUnitPoints());
        //    return;
        //}

        if (!turn)
            return;
        else
        {
            if(unit.isDucking)
                unit.isDucking = false;
        }

        if (!moving)
        {
            if (relocateCam)
            {
                StartCoroutine(MoveCamera(transform.position, .5f));
            }

            FindSelectableGrid();

            if (GameObject.FindGameObjectWithTag("Turn Manager").GetComponent<TurnManager>().attackState == AttacksState.Idle)
                CheckInput();
        }
        else
        {
            Debug.Log($"Moving Unit: {unit.gameObject.name}");
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

        if (activate && Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit) && hit.collider.tag == "Alien" && !attacking)
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

    public void SetEnemy()
    {
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
        playOnce = false;
        ButtonManager.instance.SetButtonsToCurrentPlayer(this);
    }
}
