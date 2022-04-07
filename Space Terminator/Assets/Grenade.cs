using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [HideInInspector] public ScoutMovement scoutMovement;
    public GameObject VFX;

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
            Instantiate(VFX, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}
