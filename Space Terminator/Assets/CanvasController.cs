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
        //consoleImages.Add(onSpawnImage.Invoke());
        //foreach (Image consoleImage in consoleImages)
        //{
        //    consoleImage.transform.parent = this.transform;
        //}
    }

    // Update is called once per frame
    void Update()
    {
    }
}
