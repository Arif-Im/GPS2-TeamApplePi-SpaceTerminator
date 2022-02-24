using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class leverSystem : MonoBehaviour
{

    public bool isColliding;

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.GetComponent<PlayableMovement>())
        {

            Debug.Log("Press 'E' ");
            isColliding = true;

        }
        else
        {
            isColliding = false;
        }

    }

}
