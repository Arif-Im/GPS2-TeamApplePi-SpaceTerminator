using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{

    public GlowManager glowManger;

    // Start is called before the first frame update
    void Awake()
    {
        glowManger = GetComponent<GlowManager>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                if (hit.transform != null)
                {
                    print(hit.transform.gameObject);
                    glowManger.RayGlow();
                }
            }
        }
    }
}
