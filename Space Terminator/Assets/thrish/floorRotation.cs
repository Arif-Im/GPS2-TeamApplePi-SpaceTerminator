using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class floorRotation : MonoBehaviour
{

    [Range(0.0f, 1.0f)]
    public float turning_Speed;
    
    private Quaternion Rotation;

    public GameObject leverScript;
    private new AudioSource audio;
    Collider[] troops;

    public bool rotate;

    private void Start()
    {

        audio = GetComponent<AudioSource>();

    }
    public void floorRotate()
    {
        rotate = !rotate;

        if(rotate)
        {
            Rotation = Quaternion.Euler(-90, 0, 90);
            Debug.Log($"Rotation: {Rotation}");
        }
        else
        {
            Rotation = Quaternion.Euler(-90, 0, 180);
            Debug.Log($"Rotation: {Rotation}");
        }

        if (leverScript.GetComponent<leverSystem>().isColliding == true)
        {

            troops = Physics.OverlapBox(transform.position, new Vector3(6, 2, 6));
            foreach(Collider troop in troops)
            {
                if(troop.gameObject.tag == "Player" || troop.gameObject.tag == "Alien")
                {
                    troop.gameObject.transform.parent = transform;
                }
            }

            //audio.Play();
        }
        
    }

    // Update is called once per frame
    void Update()
    {

        transform.rotation = Quaternion.Slerp(transform.rotation, Rotation, turning_Speed);
        if (troops != null && troops.Length > 0 && Quaternion.Angle(transform.rotation, Rotation) < 1f)
        {
            transform.rotation = Rotation;
            foreach(Collider troop in troops)
            {
                if (troop.gameObject.tag == "Player" || troop.gameObject.tag == "Alien")
                {
                    troop.transform.parent = null;
                }
            }
            troops = null;
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.tag == "Player" && other.gameObject.tag == "Alien")
    //    {
    //        other.transform.parent = transform;
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.gameObject.tag == "Player" && other.gameObject.tag == "Alien")
    //    {
    //        other.transform.parent = null;
    //    }
    //}
}
