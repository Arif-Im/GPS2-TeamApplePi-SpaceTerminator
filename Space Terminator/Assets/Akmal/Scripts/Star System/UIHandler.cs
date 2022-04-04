using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    public GameObject levelDialog;
    public Text LevelStatus;
    public Text scoreText;

    public static UIHandler instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
}
