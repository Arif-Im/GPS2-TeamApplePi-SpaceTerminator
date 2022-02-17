using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UnitManager : MonoBehaviour
{

    public Material glow;
    public static Action onDeselectUnits;

    // Start is called before the first frame update
    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            onDeselectUnits.Invoke();
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                var selectedUnit = hit.transform;
                if (selectedUnit.GetComponent<UnitPoitsSystem>())
                {
                    var selectedMesh = selectedUnit.GetComponent<MeshRenderer>();
                    var selectedPointSystem = selectedUnit.GetComponent<UnitPoitsSystem>();
                    if (hit.transform != null)
                    {
                        print(hit.transform.gameObject);
                        //hit.transform.gameObject.GetComponent<GlowManager>().isGlowing = true;
                        selectedMesh.material = glow;
                        selectedPointSystem.isSelected = true;
                    }
                }
            }
        }
    }

}
