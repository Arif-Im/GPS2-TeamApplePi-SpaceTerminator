using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnitStorage : MonoBehaviour
{

    public StackMnager stackMnager;
    public GameObject[] units;

    private int clickCount = 0;

    private void Start()
    {

        GameObject units = new GameObject();

    }

    private void Update()
    {

        SpawnUnits();

    }

    void SpawnUnits()
    {

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {

            int code;
            foreach (GameObject gameObject in units)
            {
                code = gameObject.GetComponent<UnitCode>().UNITCODE;
                if (stackMnager.stack.Contains(code) == true && SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Prototype Level"))
                {
                    RayHit(gameObject);
                }
            }
        }
    }

    void RayHit(GameObject gameObject)
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Input.GetMouseButtonDown(0))
        {
            clickCount++;
            Debug.Log(clickCount);
            if (Physics.Raycast(ray, out hit))
            {
                
                if (hit.collider.tag == "Grid")
                {
                    if (clickCount == 1) {
                        Grid g = hit.collider.GetComponent<Grid>();
                        Debug.Log("Target: " + g.name);
                        g.target = true;
                    }
                    else if (clickCount == 2)
                    {
                        Instantiate(gameObject, hit.point, Quaternion.identity);
                        clickCount = 0;
                    }
                }
            }
        }

    }

}
