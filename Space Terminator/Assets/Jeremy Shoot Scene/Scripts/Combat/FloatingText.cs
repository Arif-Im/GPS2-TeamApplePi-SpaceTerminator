using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    [SerializeField] float destroyTime = 3f;
    [SerializeField] Vector3 offset = new Vector3(0, 1, 0);
    [SerializeField] Vector3 random = new Vector3(0.5f, 0, 0);

    void Start()
    {
        transform.parent = null;
        Destroy(gameObject, destroyTime);
        transform.rotation = Quaternion.Euler(50, 0, 0);
        transform.localPosition += offset;
        transform.localPosition += new Vector3(Random.Range(-random.x, random.x),
            Random.Range(-random.y, random.y),
            Random.Range(-random.z, random.z));


    }
}
