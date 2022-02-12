using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
       
    }

    public float maxPoints;
    public float currentPoints;

    private void Start()
    {

        currentPoints = maxPoints;

    }

}
