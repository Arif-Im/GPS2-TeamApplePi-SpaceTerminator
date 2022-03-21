using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public ScoutMovement scoutMovement;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Grid" || other.GetComponent<Unit>() && other.GetComponent<Unit>() != scoutMovement.GetComponent<Unit>())
        {
            Collider[] enemies = Physics.OverlapSphere(transform.position, 2);
            foreach(Collider enemy in enemies)
            {
                if (enemy.GetComponent<Unit>())
                {
                    enemy.GetComponent<Unit>().TakeDamage(10);
                    GameObject.FindGameObjectWithTag("Turn Manager").GetComponent<TurnManager>().attackState = AttacksState.Idle;
                    scoutMovement.unit.DeductPointsOrChangeTurn(1);
                    Destroy(this.gameObject);
                }
            }
            Destroy(this.gameObject);
        }
    }
}
