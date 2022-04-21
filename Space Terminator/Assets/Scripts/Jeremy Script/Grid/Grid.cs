using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Hazard
{
    none, burn, intoxicated
}

public class Grid : MonoBehaviour
{
    [SerializeField] LayerMask whatIsPlayer;

    public bool playerPresent = false;

    public bool occupied = false;  //if NPC or player is not standing on it
    public bool selectable = false; //selectable area for movement
    public bool walkable = true;
    public bool target = false; //where the player is moving
    public bool isTouched; //Selected Grid for unit placement
    public bool placeable;

    public bool isCover = false; //where the player is moving
    public bool isCoverEffectArea = false; //where the player is moving

    public List<Grid> adjacencyList = new List<Grid>(); //identify neighbours next to the occupied tile

    //breadth first search
    public bool visited = false; //tile has been walked through
    public Grid parent = null;
    public int distance = 0; //max range the player can walk

    public float f = 0;
    public float g = 0;
    public float h = 0;
    
    public Grid CoverOrigin { get;  set; }
    public Cover CoverObject { get; set; }

    public Hazard haz;

    void Start()
    {

        GetComponent<Renderer>().material.color = Color.clear;

        isTouched = false;
        placeable = false;

    }

    void Update()
    {
        if (Physics.Raycast(transform.position, Vector2.up, out RaycastHit hit))
        {
            if (hit.collider.tag == "Player" || hit.collider.tag == "Alien")
                occupied = true;
        }
        else
            occupied = false;

        CheckGridStatus();
    }

    public virtual void CheckGridStatus()
    {
        if (occupied)
        {
            //GetComponent<Renderer>().material.color = new Color(1, 0, 0, .55f);
        }
        else if (haz == Hazard.burn)
        {
            GetComponent<Renderer>().material.color = new Color(1, 0, 0, 2.5f);
        }
        else if (haz == Hazard.intoxicated)
        {
            GetComponent<Renderer>().material.color = new Color(0, 1, 0, 2.5f);
        }
        else if (isTouched == true)
        {
            //Blue
            GetComponent<Renderer>().material.color = new Color(0.4575472f, 0.6672957f, 1, .55f);
        }
        else if (isCover)
        {
            if(Physics.OverlapSphere(transform.position, 10, whatIsPlayer).Length > 0)
                GetComponent<Renderer>().material.color = new Color(0, 0, 1, .55f);
        }
        else if (isCoverEffectArea)
        {
            //cyan
            GetComponent<Renderer>().material.color = new Color(0, 1, 1, .55f);
        }
        //else if (target)
        //{
        //    //GetComponent<Renderer>().material.color = Color.green;
        //    GetComponent<Renderer>().material.color = new Color(0, 1, 0, .55f);
        //}
        else if (selectable)
        {
            //GetComponent<Renderer>().material.color = Color.yellow;
            GetComponent<Renderer>().material.color = new Color(1, 0.92f, 0.016f, .55f);
        }
        else if (placeable == true)
        {
            //Blue
            GetComponent<Renderer>().material.color = Color.blue;
        }
        else
        {
            //GetComponent<Renderer>().material.color = Color.white;
            GetComponent<Renderer>().material.color = Color.clear;
        }
    }

    //Reset  variables
    public void Reset()
    {
        adjacencyList.Clear();
        occupied = false;  //if NPC or player is not standing on it
        selectable = false; //selectable area for movement
        target = false; //where the player is moving
        visited = false; //tile has been walked through
        parent = null;
        distance = 0; //max range the player can walk
        f = g = h = 0;
    }

    //jump height in case there's vertical 
    public void FindNeighbors(float jumpHeight, Grid target)
    {
        Reset(); //Reset previous variables when finding new neighbors

        CheckGrid(Vector3.forward, jumpHeight, target); //front
        CheckGrid(-Vector3.forward, jumpHeight, target); //back
        CheckGrid(Vector3.right, jumpHeight, target); //right
        CheckGrid(-Vector3.right, jumpHeight, target); //left

        CheckGrid(Vector3.up, jumpHeight, target); //up


    }

    public void CheckGrid(Vector3 dir, float jumpHeight, Grid target)
    {
        Vector3 halfExtent = new Vector3(.25f, (1 * jumpHeight) / 2f, .25f); //check if a tile is present (1 x 1 x 1 dimension)
        Collider[] colliders = Physics.OverlapBox(transform.position + dir, halfExtent);

        foreach (Collider item in colliders)
        {
            Grid grid = item.GetComponent<Grid>();
            if (grid != null && grid.walkable) //if there's a grid and it's walkable
            {
                RaycastHit hit;
                adjacencyList.Add(grid);
                //check if there's NPC or players occupying the grid. If no, grid is walkable
                if (!Physics.Raycast(grid.transform.position, Vector2.up, out hit, 1) || grid == target)
                {
                    //selectable = false;
                    //adjacencyList.Add(grid);
                }

                if (Physics.Raycast(grid.transform.position, Vector2.up, out hit, 1) || grid == target)
                {
                    selectable = false;
                    //adjacencyList.Add(grid);
                }

            }
        }
    }

    public Vector3 GetDirectionOfCover(Vector3 coverGridPosition, Vector3 currentGridPosition)
    {
        Vector3 directionOfConfirmedCover = (currentGridPosition - coverGridPosition).normalized;
        return directionOfConfirmedCover;
    }
}
