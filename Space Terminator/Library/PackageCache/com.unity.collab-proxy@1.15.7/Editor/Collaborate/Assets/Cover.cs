using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cover : Grid
{
    
    // Start is called before the first frame update
    void Start()
    {
        CheckGrid(Vector3.forward); //front
        CheckGrid(-Vector3.forward); //back
        CheckGrid(Vector3.right); //right
        CheckGrid(-Vector3.right); //left
    }

    // Update is called once per frame
    void Update()
    {
        //CheckGridStatus();
    }

    //public override void CheckGridStatus()
    //{
    //    if (occupied)
    //    {
    //        GetComponent<Renderer>().material.color = Color.red;
    //    }
    //    else if (target)
    //    {
    //        GetComponent<Renderer>().material.color = Color.green;
    //    }
    //    else if (selectable)
    //    {
    //        GetComponent<Renderer>().material.color = Color.yellow;
    //    }
    //    else if(isCover)
    //    {
    //        GetComponent<Renderer>().material.color = Color.blue;
    //    }
    //    else
    //    {
    //        GetComponent<Renderer>().material.color = Color.white;
    //    }
    //}

    public void CheckGrid(Vector3 dir)
    {
        Vector3 halfExtent = new Vector3(.25f, 1, .25f); //check if a tile is present (1 x 1 x 1 dimension)
        Collider[] colliders = Physics.OverlapBox(transform.position + dir, halfExtent);

        foreach (Collider item in colliders)
        {
            Grid grid = item.GetComponent<Grid>();
            Debug.Log($"Grid Name: {grid}");
            if (grid != null && grid.walkable) //if there's a grid and it's walkable
            {
                RaycastHit hit;

                //check if there's NPC or players occupying the grid. If no, grid is walkable
                if (!Physics.Raycast(grid.transform.position, Vector2.up, out hit, 1))
                {
                    grid.isCover = true;
                    //adjacencyList.Add(grid);
                }

            }
        }
    }
}
