using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    [SerializeField] float destroyTime = 3f;
    void Start()
    {
        Destroy(gameObject, destroyTime);
    }

    void Update()
    {
        
    }
}
