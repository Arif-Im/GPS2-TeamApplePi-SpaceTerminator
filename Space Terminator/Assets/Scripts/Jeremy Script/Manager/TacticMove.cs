using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatusID
{
    none, burn, intoxicated
}


public class TacticMove : MonoBehaviour
{

    [HideInInspector] public Unit unit;
    [HideInInspector] public List<Grid> selectableGrid = new List<Grid>(); //to reset selectable tiles after moving
    protected GameObject[] grids;

    Stack<Grid> path = new Stack<Grid>(); //path is calculated in reverse (from end to beginning)
    Grid currentGrid;


    [Header("Grid")]
    [SerializeField] int moveTile = 5; //can move 5 tiles per turn
    [SerializeField] int originalMoveTile = 5; //can move 5 tiles per turn
    [SerializeField] float jumpHeight = 2; //can jump 2 tiles
    [SerializeField] LayerMask whatIsGrid;
    public bool moving = false;
    public Grid actualTargetGrid;
    public bool attacking;

    [Space]

    [Header("Movements")]
    [SerializeField] float moveSpeed = 2;
    [SerializeField] float jumpVel = 5;
    [SerializeField] float rotateSpeed = 2;
    Vector3 velocity = new Vector3();
    Vector3 heading = new Vector3(); //direction going

    [Space]

    [Header("Attack VFX")]
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] GameObject punchPrefab;

    [Space]

    [Header("Attack Stats")]
    //temporary to be removed
    public float attackerDamage;
    public AttacksState attackState;
    public GameObject arrow;
    public StatusID currentStatus;
    public int ammoCount = 3;
    public bool isPunching;

    [Space]

    [Header("Turn")]
    public bool turn = false;

    [Space]

    [Header("Jump")]
    //Jump variables
    bool fallingDown = false, jumpingUp = false, movingEdge = false;
    Vector3 jumpTarget;
    float halfHeight = 0;

    int statusCount;

    [Space]

    [Header("Camera")]
    public bool relocateCam = true;


    public int MoveTile { get => moveTile; }

    public bool isAttack;

    private void Awake()
    {
        unit = GetComponent<Unit>();
    }

    protected void Start()
    {
        Initialize();
    }

    void Update()
    {

    }

    public void Initialize()
    {
        grids = GameObject.FindGameObjectsWithTag("Grid"); //store all the gameobjects with grid into the array
        halfHeight = GetComponent<Collider>().bounds.extents.y;
        TurnManager.AddUnit(this);
    }

    void GetCurrentGrid()
    {
        //Debug.Log("Getting Current Grid");
        currentGrid = GetTargetTile(gameObject);
        currentGrid.occupied = true;
    }

    public Grid GetTargetTile(GameObject target)
    {
        RaycastHit hit;
        Grid grid = null;

        if (Physics.Raycast(target.transform.position, -Vector3.up, out hit, 1, whatIsGrid))
        {
            grid = hit.collider.GetComponent<Grid>();
        }
        else
        {
            grid = target.GetComponent<Grid>();
        }

        //if (grid == null) { Debug.Log("GetTargetTile: grid = null"); }

        return grid;
    }

    public virtual void ComputeAdjacencyList(float jumpHeight, Grid target)
    {
        //Debug.Log("Computing");
        grids = GameObject.FindGameObjectsWithTag("Grid"); //find all the grids

        foreach (GameObject grid in grids)
        {
            Grid g = grid.GetComponent<Grid>();
            g.FindNeighbors(jumpHeight, target);
        }
    }

    public void FindSelectableGrid()
    {
        ComputeAdjacencyList(jumpHeight, null);
        GetCurrentGrid();

        //initiate Breadth First Search, starts with first grid, then grow outward to check selectable grids
        Queue<Grid> process = new Queue<Grid>();

        process.Enqueue(currentGrid);
        currentGrid.visited = true; //don't come back to current grid

        while (process.Count > 0)
        {
            Grid g = process.Dequeue();

            selectableGrid.Add(g);
            g.selectable = true;

            //Can't stand on unit

            RaycastHit hit;
            if (Physics.Raycast(g.transform.position, Vector2.up, out hit, 1))
            {
                g.selectable = false;
                //adjacencyList.Add(grid);
            }


            if (g.distance < moveTile) //if the distance is less than the max amount of moveable tile, search moveable beighbors
            {
                foreach (Grid grid in g.adjacencyList)
                {
                    //only process unvisited grids
                    if (!grid.visited)
                    {
                        grid.parent = g;
                        grid.visited = true;
                        grid.distance = 1 + g.distance;
                        process.Enqueue(grid);
                    }
                }
            }
        }
    }

    public List<Grid> SelectableGrids
    {
        get => selectableGrid;
    }

    public Stack<Grid> Path
    {
        get => path;
    }

    public bool Moving
    {
        get => moving;
    }

    public void MoveToGrid(Grid grid)
    {
        if(grid == null)
        {
            Debug.Log("Grid Bug");
            unit.DeductPointsOrChangeTurn(unit.GetUnitPoints());
        }

        path.Clear();
        grid.target = true;
        moving = true;

        Grid next = grid;
        //loop until there is no more grid
        while (next != null)
        {
            path.Push(next);
            next = next.parent;
        }
    }

    //bool hasShot = false;
    public IEnumerator Shoot(Grid targetEnemy, Action actionPoint)
    {
        if (targetEnemy == null) yield break;
        targetEnemy.GetComponent<TacticCover>().ComputeConditionsToSetCover();

        gameObject.GetComponent<Unit>().state = AttackState.UnderAttack;
        GameObject.FindGameObjectWithTag("Dice").GetComponent<Dice>().state = DiceRoll.Rolling;
        GameObject.FindGameObjectWithTag("Turn Manager").GetComponent<TurnManager>().attackState = AttacksState.UnderAttack;
        GameObject.FindGameObjectWithTag("Dice").GetComponent<Dice>().RollDice();
        GameObject.FindGameObjectWithTag("Crit Dice").GetComponent<Dice>().RollDice();
        attacking = true;

        Quaternion shootRotation = Quaternion.LookRotation(targetEnemy.transform.position - transform.position);
        float time = 0;
        RemoveSelectableGrid();

        yield return new WaitForSeconds(2f);

        while(time < 1)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, shootRotation, time);
            time += Time.deltaTime * rotateSpeed;
        }

        isAttack = true;
        yield return null;
        isAttack = false;

        yield return new WaitForSeconds(.25f);
        

        for(int x = 0; x < 3; x++)
        {
            if (bulletPrefab != null)
                SpawnBullet(this.gameObject, GetTargetTile(this.gameObject).isCoverEffectArea, bulletPrefab);
            yield return new WaitForSeconds(0.15f);
        }

        GameObject.FindGameObjectWithTag("Turn Manager").GetComponent<TurnManager>().attackState = AttacksState.FinishAttacked;
        isAttack = false;
        if (GameObject.FindGameObjectWithTag("Turn Manager").GetComponent<TurnManager>().attackState == AttacksState.FinishAttacked/*gameObject.GetComponent<Unit>().state == AttackState.FinishAttacked*/)
        {
            yield return new WaitForSeconds(0.5f);
            GameObject.FindGameObjectWithTag("Turn Manager").GetComponent<TurnManager>().attackState = AttacksState.Idle;
        }

        if(grids != null)
        {
            foreach (GameObject grid in grids)
            {
                grid.GetComponent<Grid>().isCoverEffectArea = false;
            }
        }

        targetEnemy.GetComponent<TacticMove>().unit.interrupted = false;
        if (actionPoint != null)
        {
            actionPoint.Invoke();
        }
        attacking = false;
        //targetEnemy = null;
    }

    public IEnumerator Punch(Grid targetEnemy, Action actionPoint)
    {
        RemoveSelectableGrid();
        GameObject.FindGameObjectWithTag("Turn Manager").GetComponent<TurnManager>().attackState = AttacksState.UnderAttack;
        GameObject.FindGameObjectWithTag("Dice").GetComponent<Dice>().state = DiceRoll.Rolling;
        GameObject.FindGameObjectWithTag("Dice").GetComponent<Dice>().RollDice();
        GameObject.FindGameObjectWithTag("Crit Dice").GetComponent<Dice>().RollDice();

        attacking = true;
        Quaternion shootRotation = Quaternion.LookRotation(targetEnemy.transform.position - transform.position);
        float time = 0;
        yield return new WaitForSeconds(2f);

        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, shootRotation, time);
            time += Time.deltaTime * rotateSpeed;
        }

        yield return null;


        isPunching = true;
        SpawnBullet(this.gameObject, GetTargetTile(this.gameObject).isCoverEffectArea, punchPrefab);
        yield return new WaitForSeconds(0.1f);
        attacking = false;
        GameObject.FindGameObjectWithTag("Turn Manager").GetComponent<TurnManager>().attackState = AttacksState.FinishAttacked;

        if (GameObject.FindGameObjectWithTag("Turn Manager").GetComponent<TurnManager>().attackState == AttacksState.FinishAttacked)
        {
            yield return new WaitForSeconds(0.8f);
            GameObject.FindGameObjectWithTag("Turn Manager").GetComponent<TurnManager>().attackState = AttacksState.Idle;
          
        }


        if (grids != null)
        {
            foreach (GameObject grid in grids)
            {
                grid.GetComponent<Grid>().isCoverEffectArea = false;
            }
        }

        targetEnemy.GetComponent<TacticMove>().unit.interrupted = false;
        if (actionPoint != null)
        {
            actionPoint.Invoke();
        }
    }

    public void SpawnBullet(GameObject shooter, bool inCoverEffect, GameObject whatPref)
    {
        GameObject bullet = Instantiate(whatPref, new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.5f), transform.rotation);
        bullet.GetComponent<Bullet>().InCoverEffect = inCoverEffect;
        bullet.GetComponent<Bullet>().Shooter = shooter;
        bullet.GetComponent<Bullet>().UnitRollToHit = unit.rollToHit;
    }

    public void Move(Action onReachTarget)
    {
        if (unit.interrupted) return;
        unit.isOverwatch = false;

        //move as long as there's a path
        if (path.Count > 0)
        {
            Grid g = path.Peek();
            Vector3 target = g.transform.position;

            //calculate units position on top of target grid
            target.y += halfHeight + g.GetComponent<Collider>().bounds.extents.y * 2.6f;

            if (Vector3.Distance(transform.position, target) >= 0.05f) //keep going until you reach the grid
            {
                bool jump = transform.position.y != target.y;

                if (jump)
                {
                    Jump(target);
                } else
                {
                    CalculateHeading(target);
                    SetHorizontalVel();
                }
              

                transform.forward = heading; //set facing direction
                                     //play walk animation here
                transform.position += velocity * Time.deltaTime;
            }
            else
            {
                //CheckStandingGrid();
                transform.position = target;
                path.Pop();
            }

        }
        else
        {
            RemoveSelectableGrid();
            moving = false;
                 
            onReachTarget?.Invoke();
            //fallingDown = false;
            //jumpingUp = false;
            //movingEdge = true;
        }
    }

    #region Heading
    void CalculateHeading(Vector3 target)
    {
        heading = target - transform.position;
        heading.Normalize();
    }

    void SetHorizontalVel()
    {
        velocity = heading * moveSpeed;
    }

    void RemoveSelectableGrid()
    {
        if (currentGrid != null)
        {
            currentGrid.occupied = false;
            currentGrid = null;
        }

        foreach (Grid grid in selectableGrid)
        {
            grid.Reset();
        }

        selectableGrid.Clear();

        //CheckStandingGrid();
    }

    void Jump(Vector3 target)
    {
        if (fallingDown)
        {
            FallDownward(target);
        } else if (jumpingUp)
        {
            JumpUpward(target);
        } else if (movingEdge)
        {
            MoveToEdge();
        } else
        {
            PrepareJump(target);
        }
    }

    void PrepareJump(Vector3 target)
    {
        float targetY = target.y;
        target.y = transform.position.y;
        CalculateHeading(target);

        if (transform.position.y > target.y)
        {
            fallingDown = false;
            jumpingUp = false;
            movingEdge = true;

            jumpTarget = transform.position + (target - transform.position) / 2.0f;
        } else
        {
            fallingDown = false;
            jumpingUp = true;
            movingEdge = false;

            velocity = heading * moveSpeed / 3.0f;

            float difference = targetY - transform.position.y;

            velocity.y = jumpVel * (0.5f + difference / 2.0f);
        }
    }

    void FallDownward(Vector3 target)
    {
        velocity += Physics.gravity * Time.deltaTime; 

        if (transform.position.y <= target.y)
        {
            fallingDown = false;
            jumpingUp = false;
            movingEdge = false;


            Vector3 pos = transform.position;
            pos.y = target.y;
            transform.position = pos;

            velocity = new Vector2(); //landed
          
        }
    }

    void JumpUpward(Vector3 target)
    {
        velocity += Physics.gravity * Time.deltaTime;

        if (transform.position.y > target.y)
        {
            fallingDown = true;
            jumpingUp = false;
        }
    }

    void MoveToEdge()
    {
        if (Vector3.Distance(transform.position, jumpTarget) >= 0.005f)
        {
            SetHorizontalVel();
        } else
        {
            movingEdge = false;
            fallingDown = true;

            velocity /= 3.0f;
            velocity.y = 1.5f; //hop
        }
    }
    #endregion

    #region Pathfinding
    protected Grid FindLowestF(List<Grid> list)
    {
        Grid lowest = list[0];
        foreach(Grid g in list)
        {
            if(g.f < lowest.f)
            {
                lowest = g;
            }
        }
        list.Remove(lowest);
        return lowest;
    }

    protected Grid FindEndGrid (Grid t, bool isAbsolutePos)
    {
        Stack<Grid> tempPath = new Stack<Grid>();

        Grid next = t.parent;
        while (next != null)
        {
            tempPath.Push(next);
            next = next.parent;
        }

        if (tempPath.Count <= moveTile)
        {
            if (isAbsolutePos)
                return t;
            else
                return t.parent;
        }

        Grid endGrid = null;
        for (int x = 0; x <= moveTile; x++)
        {
             endGrid = tempPath.Pop();
        }
        return endGrid;
    }

    protected void FindPath(Grid target, bool isAbsolutePosition)
    {
        ComputeAdjacencyList(jumpHeight, target);
        GetCurrentGrid();

        List<Grid> openList = new List<Grid>();
        List<Grid> closedList = new List<Grid>();

        openList.Add(currentGrid);
        currentGrid.h = Vector3.Distance(currentGrid.transform.position, target.transform.position);
        currentGrid.f = currentGrid.h;

        while (openList.Count > 0)
        {
            Grid t = FindLowestF(openList);
            closedList.Add(t);

            if (t == target)
            {
                actualTargetGrid = FindEndGrid(t, isAbsolutePosition);
                MoveToGrid(actualTargetGrid);
                return;
            }

            foreach (Grid grid in t.adjacencyList)
            {
                if (closedList.Contains(grid))
                {

                }
                else if (openList.Contains(grid))
                {
                    float temp = t.g + Vector3.Distance(grid.transform.position, t.transform.position);
                    if (temp < grid.g)
                    {
                        grid.parent = grid;
                        grid.g = temp;
                        grid.f = grid.g - grid.h;
                    }
                }
                else
                {
                    grid.parent = t;
                    grid.g = t.g + Vector3.Distance(grid.transform.position, t.transform.position);
                    grid.h = Vector3.Distance(grid.transform.position, target.transform.position);
                    grid.f = grid.g + grid.h;

                    openList.Add(grid);
                }
            }
        }
    }
    #endregion

    public virtual void BeginTurn()
    {
        //Debug.Log("Begin Turn");
        gameObject.GetComponent<UnitPoitsSystem>().CurrentPoints = gameObject.GetComponent<UnitPoitsSystem>().maxPoints;

        moveTile = originalMoveTile;
        if (currentStatus == StatusID.intoxicated)
        {
            moveTile /= 2;
            statusCount--;
        }

        relocateCam = true;
        if (arrow != null)
            arrow.SetActive(true);
        turn = true;
    }

    public void EndTurn()
    {
        if (statusCount == 0)
        {
            moveTile = originalMoveTile;
            currentStatus = StatusID.none;
            turn = false;
            return;
        }

        if (currentStatus == StatusID.burn)
        {
            GetComponent<Unit>().BurnDamage();
            statusCount--;
        }

        turn = false;
        arrow.SetActive(false);
    }

    public IEnumerator MoveCamera(Vector3 pos, float timeToMove)
    {
        Vector3 cameraPos = GameObject.FindGameObjectWithTag("MainCamera").transform.position;
        Vector3 newPos = cameraPos;
        Vector3 offset = new Vector3(0, 0, -7f);

        var t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime / timeToMove;

            newPos.x = Mathf.Lerp(cameraPos.x, transform.localPosition.x, t);
            newPos.z = Mathf.Lerp(cameraPos.z, transform.localPosition.z + offset.z, t);


            GameObject.FindGameObjectWithTag("MainCamera").transform.position = newPos;
            yield return null;
        }

        relocateCam = false;
    }
}
