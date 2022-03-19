using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnitStorage : MonoBehaviour
{

    public StackMnager stackMnager;
    public GameObject[] units;
    private Grid choosenGrid;
    [SerializeField] List<GameObject> Grids;

    private void Start()
    {

        GameObject units = new GameObject();
        
    }

    private void Update()
    {

        string gridName;
        GameObject mainGrid;
        foreach (GameObject grid in Grids)
        {
            gridName = grid.name;
            mainGrid = GameObject.Find(gridName);
            if (gridName != null && mainGrid != null) 
            {
                if (gridName == mainGrid.name)
                {
                    Debug.Log("gridName" + gridName);
                    Debug.Log("mainGridName" + mainGrid);
                    mainGrid.GetComponent<Grid>().placeable = true;
                }
                //else if (stackMnager.stack == null)
                //{
                //    mainGrid.GetComponent<Grid>().placeable = false;
                //}
            }
        }

        

        RayCheckGrid();

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
                    break;
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
            if (Physics.Raycast(ray, out hit))
            {
                
                if (hit.collider.tag == "Grid")
                {
                    if (hit.collider.GetComponent<Grid>().isTouched == false) {
                        Grid g = hit.collider.GetComponent<Grid>();
                        if (choosenGrid == null)
                        {
                            choosenGrid = g;
                        }
                        choosenGrid.isTouched = false;
                        Debug.Log("Target: " + g.name);
                        g.isTouched = true;
                        choosenGrid = g;
                    }
                    else if (hit.collider.GetComponent<Grid>().isTouched == true)
                    {
                        Instantiate(gameObject, hit.point, Quaternion.identity);
                        stackMnager.stack.Pop();
                        hit.collider.GetComponent<Grid>().isTouched = false;
                    }
                }
            }
        }

    }

    private void RayCheckGrid()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "Grid")
                {
                    foreach (GameObject grid in Grids)
                    {
                        if (hit.collider.name == grid.name)
                        {
                            SpawnUnits();
                        }
                    }
                }
            }
        }
    }

}
