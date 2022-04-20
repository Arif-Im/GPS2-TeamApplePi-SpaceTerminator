using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cover : Grid
{
    [SerializeField] public int health = 1;
    [SerializeField] LayerMask whatIsGrid;
    List<Grid> coverPositions = new List<Grid>();
    Grid grid;

    // Start is called before the first frame update
    void Start()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 10, whatIsGrid))
        {
            grid = hit.collider.gameObject.GetComponent<Grid>();
            grid.walkable = false;
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

    private void OnDisable()
    {
        foreach(Grid coverPosition in coverPositions)
        {
            coverPosition.isCover = false;
        }
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
        grid.CoverObject = this;
        grid.CoverOrigin = this.grid;
        grid.isCover = true;
        coverPositions.Add(grid);
    }

    public void DamageGrid(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public List<Grid> CoverPositions { get => coverPositions; }
}
