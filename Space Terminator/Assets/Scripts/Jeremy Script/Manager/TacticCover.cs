using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticCover : TacticMove
{
    [SerializeField] LayerMask whatIsGrid;
    Vector3 directionOfCoverEffect;

    new void Start()
    {

    }

    public void CheckIfCanSetCover()
    {
        if (GetTargetTile(this.gameObject).CoverOrigin == null) return;
        //grid center reached
        if (GetTargetTile(this.gameObject).isCover)
        {
            directionOfCoverEffect = GetTargetTile(this.gameObject).GetDirectionOfCover(GetTargetTile(this.gameObject).CoverOrigin.transform.position, GetTargetTile(this.gameObject).transform.position);
            SetCoverEffect();
            unit.isTakingCover = true;
        }
        else
        {
            foreach (GameObject grid in grids)
            {
                grid.GetComponent<Grid>().isCoverEffectArea = false;
            }
        }
    }

    void SetCoverEffect()
    {
        if (Mathf.Abs(directionOfCoverEffect.x) > 0)
        {
            switch (directionOfCoverEffect.x)
            {
                case 1:
                    SetDiagonalCoverGrids(Vector3.back, Vector3.left, 100);
                    break;
                case -1:
                    SetDiagonalCoverGrids(Vector3.back, -Vector3.left, 100);
                    break;
            }
        }

        if (Mathf.Abs(directionOfCoverEffect.z) > 0)
        {
            switch (directionOfCoverEffect.z)
            {
                case 1:
                    SetDiagonalCoverGrids(Vector3.back, Vector3.left, 100);
                    break;
                case -1:
                    SetDiagonalCoverGrids(-Vector3.back, Vector3.left, 100);
                    break;
            }
        }
    }

    private void SetDiagonalCoverGrids(Vector3 x, Vector3 z, float amount)
    {
        if (Mathf.Abs(directionOfCoverEffect.x) > 0)
        {
            Bothways(x, z, amount);
            Bothways(-x, z, amount);
        }

        if (Mathf.Abs(directionOfCoverEffect.z) > 0)
        {
            Bothways(x, z, amount);
            Bothways(x, -z, amount);
        }
    }

    private void Bothways(Vector3 x, Vector3 z, float amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (i <= 0)
            {
                FindDiagonalGrid(x, z, 5, GetTargetTile(this.gameObject).transform.position);
            }
            else
            {
                if (adjacentGrid == null) continue;
                FindDiagonalGrid(x, z, 5, diagonalGrid.transform.position);
            }
        }
    }

    Grid adjacentGrid;
    Grid diagonalGrid;
    void FindDiagonalGrid(Vector3 dir1, Vector3 dir2, float distance, Vector3 origin)
    {
        SetAreaOfCoverEffect(origin);

        if (Physics.Raycast(origin, dir1, out RaycastHit hit, distance, whatIsGrid))
        {
            adjacentGrid = hit.collider.gameObject.GetComponent<Grid>();
        }

        if (Physics.Raycast(adjacentGrid.transform.position, dir2, out RaycastHit hit2, distance, whatIsGrid) && adjacentGrid != null)
        {
            diagonalGrid = hit2.collider.gameObject.GetComponent<Grid>();
            diagonalGrid.isCoverEffectArea = true;

            SetAreaOfCoverEffect(diagonalGrid.transform.position);
        }
    }

    private void SetAreaOfCoverEffect(Vector3 origin)
    {
        Vector3 absoluteDir = new Vector3(Mathf.Abs(directionOfCoverEffect.x), Mathf.Abs(directionOfCoverEffect.y), Mathf.Abs(directionOfCoverEffect.z));

        Collider[] coverGrids = Physics.OverlapBox(origin + -directionOfCoverEffect * 6, new Vector3(absoluteDir.x, absoluteDir.y, absoluteDir.z) * 6);
        foreach (Collider coverGrid in coverGrids)
        {
            coverGrid.GetComponent<Grid>().isCoverEffectArea = true;
        }
    }
}
