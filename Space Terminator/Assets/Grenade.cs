using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Alien")
        {
            other.GetComponent<Unit>().TakeDamage(5);
            Destroy(this.gameObject);
        }
    }
}
