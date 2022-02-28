using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceHolder : MonoBehaviour
{
    public GameObject tes;
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "Player")
        {
            tes.SetActive(true);
        }
    }
}
