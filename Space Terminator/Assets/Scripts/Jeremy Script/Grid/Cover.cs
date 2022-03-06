using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cover : Grid
{
    [SerializeField] LayerMask whatIsGrid;
    List<Grid> coverPositions = new List<Grid>();
    Grid grid;

    // Start is called before the first frame update
    void Start()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 10, whatIsGrid))
        {
            //Debug.Log(hit.collider.gameObject.GetComponent<Grid>());
            grid = hit.collider.gameObject.GetComponent<Grid>();
        }


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
            if (grid != null && grid.walkable) //if there's a grid and it's walkable
            {
                RaycastHit hit;

                if (!Physics.Raycast(grid.transform.position, Vector2.up, out hit, 1))
                {
                    SetCover(grid);
                    coverPositions.Add(grid);
                }

            }
        }
    }

    public void SetCover(Grid grid)
    {
        grid.CoverOrigin = this.grid;
        grid.isCover = true;
    }

    public List<Grid> CoverPositions { get => coverPositions; }
}
