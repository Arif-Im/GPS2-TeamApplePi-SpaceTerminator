using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSmoke : MonoBehaviour
{
    int turnBeforeEnd = 3;

    private void Update()
    {
        if (turnBeforeEnd <= 0)
            Destroy(this.gameObject);
    }

    public void DecrementTurn()
    {
        turnBeforeEnd--;
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Grid")
        {
            Collider[] enemies = Physics.OverlapSphere(transform.position, 2);
            foreach (Collider enemy in enemies)
            {
                if (enemy.GetComponent<Grid>())
                {
                    Grid grid = enemy.gameObject.GetComponent<Grid>();
                    grid.isCoverEffectArea = true;
                }
            }
        }
    }
}
