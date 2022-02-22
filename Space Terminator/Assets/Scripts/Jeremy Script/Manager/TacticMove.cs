using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticMove : MonoBehaviour
{
    protected Unit unit;

    List<Grid> selectableGrid = new List<Grid>(); //to reset selectable tiles after moving
    GameObject[] grids;

    Stack<Grid> path = new Stack<Grid>(); //path is calculated in reverse (from end to beginning)
    Grid currentGrid;

    public bool moving = false;
    [SerializeField] protected UnitPoitsSystem pointSystem;
    [SerializeField] protected BattleSystem battleSystem;
    [SerializeField] int moveTile = 5; //can move 5 tiles per turn
    [SerializeField] float jumpHeight = 2; //can jump 2 tiles
    [SerializeField] float moveSpeed = 2;
    [SerializeField] float jumpVel = 5;


    Vector3 velocity = new Vector3();
    Vector3 heading = new Vector3(); //direction going

    float halfHeight = 0;

    //Jump variables
    bool fallingDown = false, jumpingUp = false, movingEdge = false;
    Vector3 jumpTarget;

    void Start()
    {
        Initialize();
    }

    void Update()
    {

    }

    void Initialize()
    {
        grids = GameObject.FindGameObjectsWithTag("Grid"); //store all the gameobjects with grid into the array
        halfHeight = GetComponent<Collider>().bounds.extents.y;

    }

    void GetCurrentGrid()
    {
        currentGrid = GetTargetTile(gameObject);
        currentGrid.occupied = true;
    }

    Grid GetTargetTile(GameObject target)
    {
        RaycastHit hit;
        Grid grid = null;

        if (Physics.Raycast(target.transform.position, -Vector3.up, out hit, 1))
        {
            grid = hit.collider.GetComponent<Grid>();
        }

        return grid;
    }

    void ComputeAdjacencyList()
    {
        grids = GameObject.FindGameObjectsWithTag("Grid"); //find all the grids

        foreach (GameObject grid in grids)
        {
            Grid g = grid.GetComponent<Grid>();
            g.FindNeighbors(jumpHeight);
        }
    }

    public void FindSelectableGrid()
    {
        ComputeAdjacencyList();
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
                //grid center reached
                //if(g as Cover)
                //{
                //    Debug.Log("Player in Cover");
                //}
                transform.position = target;
                path.Pop();
            }

        }
        else
        {
            RemoveSelectableGrid();
            moving = false;
            pointSystem.minusPoints();
            //battleSystem.ChangeTurn(unit);
            onReachTarget.Invoke();
            //fallingDown = false;
            //jumpingUp = false;
            //movingEdge = true;
        }
    }

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
}
