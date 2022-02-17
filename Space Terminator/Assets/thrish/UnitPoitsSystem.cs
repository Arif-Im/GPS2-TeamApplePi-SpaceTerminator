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

    // Start is called before the first frame update
    void Awake()
    {

        currentPoints = maxPoints;
        points.text = maxPoints.ToString();

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

}
