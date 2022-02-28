using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCam : MonoBehaviour
{
   

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Camera.main.transform);
        if (transform.parent.gameObject.GetComponent<Unit>().Health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
