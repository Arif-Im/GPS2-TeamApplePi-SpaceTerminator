using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class leverSystem : MonoBehaviour
{

    public bool isColliding;
    //public GameObject Image;

    // Start is called before the first frame update
    void Start()
    {
        //Image.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.GetComponent<UnitManager>())
        {

            //Image.SetActive(true);
            Debug.Log("Press 'E' ");
            isColliding = true;
            
        }
        else
        {
            isColliding = false;
        }

    }

}
