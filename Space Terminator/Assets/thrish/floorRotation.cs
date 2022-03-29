using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class floorRotation : MonoBehaviour
{

    [Range(0.0f, 1.0f)]
    public float turning_Speed;
    
    private Quaternion rotation_90;
    private Quaternion Rotation;

    public GameObject leverScript;
    private new AudioSource audio;

    private void Start()
    {

        Rotation = Quaternion.Euler(-90, 0, 0);
        audio = GetComponent<AudioSource>();

    }
    Collider[] shits;
    public void floorRotate()
    {

        if (leverScript.GetComponent<leverSystem>().isColliding == true)
        {
            rotation_90 = Quaternion.Euler(0, 0, 90);
            Rotation = Rotation * rotation_90;
            Debug.Log("Rotate Platform");

            shits = Physics.OverlapBox(transform.position, new Vector3(6, 2, 6));
            foreach(Collider shit in shits)
            {
                if(shit.gameObject.tag == "Player" || shit.gameObject.tag == "Alien")
                {
                    shit.gameObject.transform.parent = transform;
                }
            }

            //audio.Play();
        }
        
    }

    // Update is called once per frame
    void Update()
    {

        transform.rotation = Quaternion.Slerp(transform.rotation, Rotation, turning_Speed);
        if (shits != null && shits.Length > 0 && Quaternion.Angle(transform.rotation, Rotation) < 1f)
        {
            foreach(Collider shit in shits)
            {
                shit.transform.parent = null;
            }
            shits = null;
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
