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

    public void ComputeConditionsToSetCover()
    {
        if (GetTargetTile(this.gameObject).CoverOrigin == null) return;
        
        if (GetTargetTile(this.gameObject).isCover)
        {
            directionOfCoverEffect = GetTargetTile(this.gameObject).GetDirectionOfCover(GetTargetTile(this.gameObject).CoverOrigin.transform.position, GetTargetTile(this.gameObject).transform.position);
            SetCoverEffect();
            unit.isTakingCover = true;
        }
        else
        {
            ResetCoverEffectArea();
        }
    }

    public void ResetCoverEffectArea()
    {
        foreach (GameObject grid in grids)
        {
            grid.GetComponent<Grid>().isCoverEffectArea = false;
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

    private void SetDiagonalCoverGrids(Vector3 z, Vector3 x, float amount)
    {
        if (Mathf.Abs(directionOfCoverEffect.x) > 0)
        {
            Bothways(z, x, amount);
            Bothways(-z, x, amount);
        }

        if (Mathf.Abs(directionOfCoverEffect.z) > 0)
        {
            Bothways(z, x, amount);
            Bothways(z, -x, amount);
        }
    }

    private void Bothways(Vector3 z, Vector3 x, float amount)
    {
        Vector3 currentPos = GetTargetTile(this.gameObject).gameObject.transform.position;

        for (int i = 0; i < amount; i++)
        {
            if (i <= 0)
            {
                SetAreaOfCoverEffect(currentPos);
            }
            else
            {
                SetAreaOfCoverEffect(currentPos + (x*i) + (z*i));

            }
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
