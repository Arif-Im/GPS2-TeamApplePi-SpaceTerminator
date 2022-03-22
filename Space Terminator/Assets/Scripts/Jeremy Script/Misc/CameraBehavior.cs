using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    Vector3 touchStart;
    [SerializeField] Camera cam;
    [SerializeField] float groundZ;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            touchStart = GetWorldPos(groundZ);
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 direction = touchStart - GetWorldPos(groundZ);

            direction.y = 0;
            Camera.main.transform.position += direction;
        }
    }

    Vector3 GetWorldPos(float z)
    {
        Ray mousePos = cam.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(cam.transform.forward, new Vector3(0, 0, z)); ;
        float dist;
        ground.Raycast(mousePos, out dist);
        return mousePos.GetPoint(dist);
    }
}
