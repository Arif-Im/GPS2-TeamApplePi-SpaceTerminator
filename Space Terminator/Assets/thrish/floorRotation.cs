using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

        Rotation = Quaternion.Euler(0, 0, 0);
        audio = GetComponent<AudioSource>();

    }

    public void floorRotate()
    {

        if (leverScript.GetComponent<leverSystem>().isColliding == true)
        {
            rotation_90 = Quaternion.Euler(0, 90, 0);
            Rotation = Rotation * rotation_90;
            audio.Play();
        }
        
    }

    // Update is called once per frame
    void Update()
    {

        transform.rotation = Quaternion.Slerp(transform.rotation, Rotation, turning_Speed);
        

    }

    
}
