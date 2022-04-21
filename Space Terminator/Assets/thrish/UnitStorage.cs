using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnitStorage : MonoBehaviour
{

    public StackMnager stackMnager;
    private Grid choosenGrid;
    [SerializeField] List<GameObject> Grids;
    [SerializeField] List<GameObject> placeableGrids;
    GameObject mainGrid;

    private void Update()
    {

        if (stackMnager.queue.Count == 0)
        {
            Destroy(GameObject.FindGameObjectsWithTag("dropdown")[0]);
        }

        string gridName;

        foreach (GameObject grid in Grids)
        {
            gridName = grid.name;
            mainGrid = GameObject.Find(gridName);
            if (gridName != null && mainGrid != null)
            {
                if (gridName == mainGrid.name)
                {
                    //Debug.Log("gridName" + gridName);
                    //Debug.Log("mainGridName" + mainGrid);
                    mainGrid.GetComponent<Grid>().placeable = true;
                    if (placeableGrids.Contains(mainGrid) == false)
                    {
                        placeableGrids.Add(mainGrid);
                    }
                    foreach (GameObject gameObject in placeableGrids)
                    {
                        if (stackMnager.queue.Count <= 0)
                        {

                            gameObject.GetComponent<Grid>().placeable = false;
                            gameObject.GetComponent<Grid>().occupied = false;
                        }
                    }

                }
            }
        }



        RayCheckGrid();

    }

    void SpawnUnits()
    {

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            foreach (GameObject gameObject in stackMnager.queue)
            {
                if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Main Level"))
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
                    if (hit.collider.GetComponent<Grid>().isTouched == false)
                    {
                        Grid g = hit.collider.GetComponent<Grid>();
                        if (choosenGrid == null)
                        {
                            choosenGrid = g;
                        }
                        choosenGrid.isTouched = false;
                        g.isTouched = true;
                        choosenGrid = g;
                    }
                    else if (hit.collider.GetComponent<Grid>().isTouched == true && hit.collider.GetComponent<Grid>().occupied == false)
                    {
                        Instantiate(gameObject, hit.collider.transform.position + new Vector3(0, 1.28f, 0), Quaternion.identity);
                        stackMnager.queue.Dequeue();
                        stackMnager.dropdown.options.RemoveAt(0);
                        hit.collider.GetComponent<Grid>().isTouched = false;
                        hit.collider.GetComponent<Grid>().occupied = true;
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
