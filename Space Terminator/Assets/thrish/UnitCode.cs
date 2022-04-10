using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitCode : MonoBehaviour
{

    [SerializeField] private int unitCode;
    [SerializeField] private Sprite pic;
    [SerializeField] private GameObject prefab;
    public int UNITCODE { get => unitCode; }
    public Sprite PIC { get => pic; }
    public GameObject PREFAB { get => prefab; }

}
