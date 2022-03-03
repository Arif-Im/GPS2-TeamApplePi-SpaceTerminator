using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelector : MonoBehaviour
{

    public static UnitSelector unitSelector;
    public GameObject[] units;
    public static int SIZE = 3;

    void Awake()
    {

        if (unitSelector != null)
            GameObject.Destroy(unitSelector);
        else
            unitSelector = this;

        DontDestroyOnLoad(this);

    }

    private void Start()
    {

        units = new GameObject[SIZE];

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void selectingUnit()
    {

        

    }

}
