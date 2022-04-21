using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectAmmo : MonoBehaviour
{
    private void Update()
    {
        transform.Rotate(0, 0, 50 * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Got ammo");
            other.gameObject.GetComponent<TacticMove>().ammoCount += 2;
            Destroy(gameObject);
        }
    }
}
