using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowManager : MonoBehaviour
{

    public Material glow, nonglow;
    public bool isGlowing;
    public UnitManager unitManager;

    private void Awake()
    {

        unitManager = GetComponent<UnitManager>();

    }

    public void RayGlow()
    {

        if (isGlowing == true)
        {
            gameObject.GetComponent<MeshRenderer>().material = nonglow;
            isGlowing = false;
        }
        else
        {
            gameObject.GetComponent<MeshRenderer>().material = glow;
            isGlowing = true;
        }

    }

}
