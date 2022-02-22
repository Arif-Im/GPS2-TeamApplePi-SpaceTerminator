using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitPoitsSystem : MonoBehaviour
{

    [SerializeField] public float maxPoints;
    [SerializeField] public float minusAmount = 1;
    float currentPoints;
    public Text points;
    public bool isSelected;
    public Material unGlow;

    public float CurrentPoints
    {
        get => currentPoints;
        set => currentPoints = value;
    }

    // Start is called before the first frame update
    void Awake()
    {
        currentPoints = maxPoints;
        if (points != null)
            points.text = maxPoints.ToString();

        UnitManager.onDeselectUnits += DeSelectUnit;

    }

    

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.E) && isSelected == true)
        {
            minusPoints();
            points.text = currentPoints.ToString();
        }

    }

    public void minusPoints()
    {

        currentPoints -= minusAmount;

    }

    void DeSelectUnit()
    {

        isSelected = false;
        this.gameObject.GetComponent<MeshRenderer>().material = unGlow;

    }

    private void OnDestroy()
    {

        UnitManager.onDeselectUnits -= DeSelectUnit;

    }

    private void OnDisable()
    {

        UnitManager.onDeselectUnits -= DeSelectUnit;

    }

}
