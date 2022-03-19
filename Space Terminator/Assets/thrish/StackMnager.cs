using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StackMnager : MonoBehaviour
{

    private static StackMnager instance;
    public Stack<int> stack = new Stack<int>();

    public List<GameObject> slots = new List<GameObject>();

    void Awake()
    {

        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log(stack.Count);

    }

    // Update is called once per frame
    void Update()
    {

        PushObject();

    }

    private void PushObject()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("LobbyMenu")) {
            foreach (GameObject gameObject in slots)
            {
                if (gameObject.GetComponent<ChoosenUnits>().isUsed == true)
                {
                    int code = gameObject.GetComponent<ChoosenUnits>().unitCode;
                    if (stack.Contains(code) == false)
                    {
                        stack.Push(code);
                    }
                    else
                    {
                        Debug.Log("Found same code");
                    }

                    foreach (int CODE in stack)
                    {
                        Debug.Log("Element in stack: " + CODE + "\n");
                        Debug.Log("Number of elements in stack: " + stack.Count + "\n");
                    }
                }
            }
        }
    }

}
