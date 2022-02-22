using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseUnits : MonoBehaviour
{
    private static ChooseUnits instance;

    public static ChooseUnits Instance
    {

        get
        {
            if (instance == null)
            {
                instance = new ChooseUnits();//GameObject.FindObjectOfType<ChooseUnits>();
            }

            return instance;
        }

    }

    void Awake()
    {

        DontDestroyOnLoad(gameObject);

    }

    private void Start()
    {
        


    }

}
