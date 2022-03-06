using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticMove : MonoBehaviour
{

    protected Unit unit;

    List<Grid> selectableGrid = new List<Grid>(); //to reset selectable tiles after moving
    protected GameObject[] grids;

    Stack<Grid> path = new Stack<Grid>(); //path is calculated in reverse (from end to beginning)
    Grid currentGrid;

    public bool moving = false;
    [SerializeField] int moveTile = 5; //can move 5 tiles per turn
    [SerializeField] float jumpHeight = 2; //can jump 2 tiles
    [SerializeField] float moveSpeed = 2;
    [SerializeField] float jumpVel = 5;

    [SerializeField] float rotateSpeed = 2;
    [SerializeField] GameObject bulletPrefab;

    Vector3 velocity = new Vector3();
    Vector3 heading = new Vector3(); //direction going

    float halfHeight = 0;

    //Jump variables
    bool fallingDown = false, jumpingUp = false, movingEdge = false;
    Vector3 jumpTarget;

    public Grid actualTargetGrid;
    public bool attacking;

    //temporary to be removed
    public float attackerDamage;
    public AttacksState attackState;
    public bool turn = false;

    public int MoveTile { get => moveTile; }

    private void Awake()
    {
        unit = GetComponent<Unit>();
    }

    public void Start()
    {
        Initialize();
    }

    //public void CheckStandingGrid()
    //{
    //    if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 10, whatIsGrid))
    //    {
    //        standingGrid = hit.collider.gameObject.GetComponent<Grid>();
    //    }
    //}

    void Initialize()
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

        if (Physics.Raycast(target.transform.position, -Vector3.up, out hit, 1))
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

    void ComputeAdjacencyList(float jumpHeight, Grid target)
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
    public IEnumerator Shoot(Grid targetEnemy)
    {
        //if (hasShot == true) yield break;
        //GetComponent<TacticCover>().CheckStandingGrid();
        //targetEnemy.GetComponent<TacticCover>().CheckIfCanSetCover();

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

        yield return null;

        for(int x = 0; x < 3; x++)
        {
            if (bulletPrefab != null)
                SpawnBullet(this.gameObject, targetEnemy.GetComponent<Unit>().isTakingCover, GetTargetTile(this.gameObject).isCoverEffectArea);
                //Instantiate(bulletPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z +0.5f), transform.rotation);
            Debug.Log($"{gameObject.name}, Pew");
            yield return new WaitForSeconds(0.1f);
        }

        //gameObject.GetComponent<Unit>().state = AttackState.FinishAttacked;
        GameObject.FindGameObjectWithTag("Turn Manager").GetComponent<TurnManager>().attackState = AttacksState.FinishAttacked;

        if (GameObject.FindGameObjectWithTag("Turn Manager").GetComponent<TurnManager>().attackState == AttacksState.FinishAttacked/*gameObject.GetComponent<Unit>().state == AttackState.FinishAttacked*/)
        {
            yield return new WaitForSeconds(0.5f);
            //gameObject.GetComponent<Unit>().state = AttackState.Idle;
            GameObject.FindGameObjectWithTag("Turn Manager").GetComponent<TurnManager>().attackState = AttacksState.Idle;
        }

        unit.DeductPointsOrChangeTurn(1);
        attacking = false;
        //hasShot = false;
    }

    public void SpawnBullet(GameObject shooter, bool opponentInCover, bool inCoverEffect)
    {
        GameObject bullet = Instantiate(bulletPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.5f), transform.rotation);
        bullet.GetComponent<Bullet>().OpponentInCover = opponentInCover;
        bullet.GetComponent<Bullet>().InCoverEffect = inCoverEffect;
        bullet.GetComponent<Bullet>().Shooter = shooter;
    }

    public void Move(Action onReachTarget)
    {
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
            onReachTarget.Invoke();
            //fallingDown = false;
            //jumpingUp = false;
            //movingEdge = true;
        }
    }

    #region Cover
    //public void CheckIfCanSetCover()
    //{
    //    CheckStandingGrid();
    //    if (standingGrid.CoverOrigin == null) return;
    //    //grid center reached
    //    if (standingGrid.isCover)
    //    {
    //        directionOfCoverEffect = standingGrid.GetDirectionOfCover(standingGrid.CoverOrigin.transform.position, standingGrid.transform.position);
    //        SetCoverEffect();
    //        unit.isTakingCover = true;
    //    }
    //    else
    //    {
    //        foreach (GameObject grid in grids)
    //        {
    //            grid.GetComponent<Grid>().isCoverEffectArea = false;
    //        }
    //    }
    //}

    //void SetCoverEffect()
    //{
    //    if (Mathf.Abs(directionOfCoverEffect.x) > 0)
    //    {
    //        switch(directionOfCoverEffect.x)
    //        {
    //            case 1:
    //                SetDiagonalCoverGrids(Vector3.back, Vector3.left, 100);
    //                break;
    //            case -1:
    //                SetDiagonalCoverGrids(Vector3.back, -Vector3.left, 100);
    //                break;
    //        }
    //    }

    //    if (Mathf.Abs(directionOfCoverEffect.z) > 0)
    //    {
    //        switch (directionOfCoverEffect.z)
    //        {
    //            case 1:
    //                SetDiagonalCoverGrids(Vector3.back, Vector3.left, 100);
    //                break;
    //            case -1:
    //                SetDiagonalCoverGrids(-Vector3.back, Vector3.left, 100);
    //                break;
    //        }
    //    }
    //}

    //private void SetDiagonalCoverGrids(Vector3 x, Vector3 z, float amount)
    //{
    //    if(Mathf.Abs(directionOfCoverEffect.x) > 0)
    //    {
    //        Bothways(x, z, amount);
    //        Bothways(-x, z, amount);
    //    }

    //    if (Mathf.Abs(directionOfCoverEffect.z) > 0)
    //    {
    //        Bothways(x, z, amount);
    //        Bothways(x, -z, amount);
    //    }
    //}

    //private void Bothways(Vector3 x, Vector3 z, float amount)
    //{
    //    for (int i = 0; i < amount; i++)
    //    {
    //        if (i <= 0)
    //        {
    //            FindDiagonalGrid(x, z, 5, standingGrid.transform.position);
    //        }
    //        else
    //        {
    //            if (adjacentGrid == null) continue;
    //            FindDiagonalGrid(x, z, 5, diagonalGrid.transform.position);
    //        }
    //    }
    //}

    //Grid adjacentGrid;
    //Grid diagonalGrid;
    //void FindDiagonalGrid(Vector3 dir1, Vector3 dir2, float distance, Vector3 origin)
    //{
    //    SetAreaOfCoverEffect(origin);

    //    if (Physics.Raycast(origin, dir1, out RaycastHit hit, distance, whatIsGrid))
    //    {
    //        adjacentGrid = hit.collider.gameObject.GetComponent<Grid>();
    //    }

    //    if (Physics.Raycast(adjacentGrid.transform.position, dir2, out RaycastHit hit2, distance, whatIsGrid) && adjacentGrid != null)
    //    {
    //        diagonalGrid = hit2.collider.gameObject.GetComponent<Grid>();
    //        diagonalGrid.isCoverEffectArea = true;

    //        SetAreaOfCoverEffect(diagonalGrid.transform.position);
    //    }
    //}

    //private void SetAreaOfCoverEffect(Vector3 origin)
    //{
    //    Vector3 absoluteDir = new Vector3(Mathf.Abs(directionOfCoverEffect.x), Mathf.Abs(directionOfCoverEffect.y), Mathf.Abs(directionOfCoverEffect.z));

    //    Collider[] coverGrids = Physics.OverlapBox(origin + -directionOfCoverEffect * 6, new Vector3(absoluteDir.x, absoluteDir.y, absoluteDir.z) * 6);
    //    foreach (Collider coverGrid in coverGrids)
    //    {
    //        coverGrid.GetComponent<Grid>().isCoverEffectArea = true;
    //    }
    //}

    #endregion

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

    public void BeginTurn()
    {
        gameObject.GetComponent<UnitPoitsSystem>().CurrentPoints = gameObject.GetComponent<UnitPoitsSystem>().maxPoints;
        turn = true;
    }

    public void EndTurn()
    {
        turn = false;
    }
}
