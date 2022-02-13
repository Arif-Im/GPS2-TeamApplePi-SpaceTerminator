using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointsManager : MonoBehaviour
{
    private static PointsManager instance;

    public static PointsManager Instance
    {

        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<PointsManager>();
            }

            return instance;
        }

    }

    void Awake()
    {

        DontDestroyOnLoad(gameObject);
        points.text = maxPoints.ToString();
       
    }

    public float maxPoints;
    private float currentPoints;
    public GameObject[] units;
    playerActions playerActions;
    public Text points;
    //public GameObject unit;

    private void Start()
    {

        currentPoints = maxPoints;
        foreach (GameObject unit in units)
        {
            playerActions = unit.GetComponent<playerActions>();
        }

    }

    private void Update()
    {

        if (playerActions.attack() > 0)
        {
            points.text = MinusPoints().ToString();
        }

    }

    float MinusPoints()
    {

        currentPoints -= 1;
        return currentPoints;

    }

}
