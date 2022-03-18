using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnitStorage : MonoBehaviour
{

    public StackMnager stackMnager;
    public GameObject[] units;
    private Grid choosenGrid;
    [SerializeField] List<Collider> Grids;

    private int clickCount = 0;

    private void Start()
    {

        GameObject units = new GameObject();

    }

    private void Update()
    {


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
            clickCount++;
            Debug.Log(clickCount);
            if (Physics.Raycast(ray, out hit))
            {
                
                if (hit.collider.tag == "Grid")
                {
                    if (hit.collider.GetComponent<Grid>().target == false) {
                        Grid g = hit.collider.GetComponent<Grid>();
                        if (choosenGrid == null)
                        {
                            choosenGrid = g;
                        }
                        choosenGrid.target = false;
                        Debug.Log("Target: " + g.name);
                        g.target = true;
                        choosenGrid = g;
                    }
                    else if (hit.collider.GetComponent<Grid>().target == true)
                    {
                        Instantiate(gameObject, hit.point, Quaternion.identity);
                        stackMnager.stack.Pop();
                        hit.collider.GetComponent<Grid>().target = false;
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
                    foreach (Collider grid in Grids)
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
