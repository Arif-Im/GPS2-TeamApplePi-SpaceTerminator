using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    public delegate void SpawnImage();
    public SpawnImage onSpawnImage;


    public static CanvasController singleton;

    List<Image> consoleImages = new List<Image>();

    private void Awake()
    {
        singleton = this;
    }

    void Start()
    {
        onSpawnImage.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
