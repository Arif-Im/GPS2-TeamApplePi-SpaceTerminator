using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerActions : MonoBehaviour
{

    private Vector3 currentPos;
    private Vector3 previousPos;
    private float numOfAtck;

    // Start is called before the first frame update
    void Awake()
    {

        previousPos = gameObject.transform.position;

    }

    // Update is called once per frame
    void Update()
    {

        currentPos = gameObject.transform.position;

        //stuffs
        attack();

        previousPos = currentPos;

    }

    public float attack()
    {

        //testing
        if (Input.GetKeyDown(KeyCode.E))
        {

            numOfAtck++;

        }

        return numOfAtck;

    }

}
