using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cover : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        CheckCoverPositions(Vector3.forward); //front
        CheckCoverPositions(-Vector3.forward); //back
        CheckCoverPositions(Vector3.right); //right
        CheckCoverPositions(-Vector3.right); //left
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CheckCoverPositions(Vector3 dir)
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
