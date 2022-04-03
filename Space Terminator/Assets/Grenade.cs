using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public ScoutMovement scoutMovement;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Grid")
        {
            Collider[] enemies = Physics.OverlapSphere(transform.position, 2);
            foreach(Collider enemy in enemies)
            {
                if (enemy.GetComponent<Unit>())
                {
                    enemy.GetComponent<Unit>().TakeDamage(10);
                }
            }
            GameObject.FindGameObjectWithTag("Turn Manager").GetComponent<TurnManager>().attackState = AttacksState.Idle;
            scoutMovement.unit.DeductPointsOrChangeTurn(scoutMovement.unit.GetUnitPoints());
            Destroy(this.gameObject);
        }
    }
}
