using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBehavior : MonoBehaviour
{
    Vector3 originalPos;
    [SerializeField] float speed;

    void Update()
    {
        float y = Mathf.PingPong(Time.time * speed, 1) + transform.parent.position.y * 2;
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }
}
