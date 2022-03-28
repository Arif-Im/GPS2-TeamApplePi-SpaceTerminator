using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class leverSystem : MonoBehaviour
{
    public bool isColliding;
    public Image consoleImagePrefab;
    Image consoleImage;
    Vector2 worldToScreen;
    //public GameObject Image;

    private void Awake()
    {
        CanvasController.singleton.onSpawnImage += SpawnConsoleImage;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Image.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        worldToScreen = Camera.main.WorldToScreenPoint(transform.position);
        if (consoleImage != null)
        {
            consoleImage.transform.position = worldToScreen;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.GetComponent<UnitManager>())
        {

            //Image.SetActive(true);
            Debug.Log("Press 'E' ");
            isColliding = true;
            
        }
        else
        {
            isColliding = false;
        }

    }

    public void SpawnConsoleImage()
    {
        //Debug.Log("Spawn Image");
        consoleImage = Instantiate(consoleImagePrefab, worldToScreen, Quaternion.identity);
        consoleImage.transform.parent = CanvasController.singleton.GetComponent<Canvas>().transform;
        //return consoleImage;
    }

    private void OnDisable()
    {
        CanvasController.singleton.onSpawnImage -= SpawnConsoleImage;
    }
}
