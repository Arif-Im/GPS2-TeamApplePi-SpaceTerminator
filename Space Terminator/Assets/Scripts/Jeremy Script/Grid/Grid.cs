using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public bool occupied = false;  //if NPC or player is not standing on it
    public bool selectable = false; //selectable area for movement
    public bool walkable = true;
    public bool target = false; //where the player is moving

    public List<Grid> adjacencyList = new List<Grid>(); //identify neighbours next to the occupied tile

    //breadth first search
    public bool visited = false; //tile has been walked through
    public Grid parent = null;
    public int distance = 0; //max range the player can walk

    void Start()
    {

    }

    void Update()
    {
        if (occupied)
        {
            GetComponent<Renderer>().material.color = Color.red;
        }
        else if (target)
        {
            GetComponent<Renderer>().material.color = Color.green;
        }
        else if (selectable)
        {
            GetComponent<Renderer>().material.color = Color.yellow;
        }
        else
        {
            GetComponent<Renderer>().material.color = Color.white;
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
    }

    //jump height in case there's vertical 
    public void FindNeighbors(float jumpHeight)
    {
        Reset(); //Reset previous variables when finding new neighbors

        CheckGrid(Vector3.forward, jumpHeight); //front
        CheckGrid(-Vector3.forward, jumpHeight); //back
        CheckGrid(Vector3.right, jumpHeight); //right
        CheckGrid(-Vector3.right, jumpHeight); //left

    }

    public void CheckGrid(Vector3 dir, float jumpHeight)
    {
        Vector3 halfExtent = new Vector3(.25f, (1 * jumpHeight) / 2f, .25f); //check if a tile is present (1 x 1 x 1 dimension)
        Collider[] colliders = Physics.OverlapBox(transform.position + dir, halfExtent);

        foreach (Collider item in colliders)
        {
            Grid grid = item.GetComponent<Grid>();
            if (grid != null && grid.walkable) //if there's a grid and it's walkable
            {
                RaycastHit hit;

                //check if there's NPC or players occupying the grid. If no, grid is walkable
                if (!Physics.Raycast(grid.transform.position, Vector2.up, out hit, 1))
                {
                    adjacencyList.Add(grid);
                }

            }
        }
    }
}
