using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StackMnager : MonoBehaviour
{

    private static StackMnager instance;
<<<<<<< Updated upstream
    public Queue<int> stack = new Queue<int>();
=======
    public Queue<GameObject> queue = new Queue<GameObject>();

    public Dropdown scrollbar;
    public Dropdown dropdown;
    //private Image pic;
>>>>>>> Stashed changes

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
        //Debug.Log(stack.Count);

        scrollbar.ClearOptions();

    }

    // Update is called once per frame
    void Update()
    {

        
        PushObject();
        createScrollBar();

    }

    private void PushObject()
    {

        Sprite pic;
        

        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("LobbyMenu")) {
            foreach (GameObject gameObject in slots)
            {
                if (gameObject.GetComponent<ChoosenUnits>().isUsed == true)
                {
<<<<<<< Updated upstream
                    int code = gameObject.GetComponent<ChoosenUnits>().unitCode;
                    if (stack.Contains(code) == false)
                    {
                        stack.Enqueue(code);
                    }
                    else
                    {
                        //Debug.Log("Found same code");
                    }
=======
                    //for (int i = 0; i < 0; i++)
                    //{
                        pic = gameObject.GetComponent<ChoosenUnits>().pic;
                        int code = gameObject.GetComponent<ChoosenUnits>().unitCode;
                        GameObject unit = gameObject.GetComponent<ChoosenUnits>().unit;
                        if (queue.Contains(unit) == false)
                        {
                            queue.Enqueue(unit);
>>>>>>> Stashed changes

                            var pics = new Dropdown.OptionData(pic);
                            scrollbar.options.Add(pics);
                        }
                        else
                        {
                            //Debug.Log("Found same code");
                        }
                    //}
                    //foreach (int CODE in stack)
                    //{
                        //Debug.Log("Element in stack: " + CODE + "\n");
                        //Debug.Log("Number of elements in stack: " + stack.Count + "\n");
                    //}
                }
            }
        }
    }

    private void createScrollBar()
    {

        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Prototype Level") && GameObject.FindGameObjectsWithTag("dropdown").Length == 0)
        {
            dropdown = Instantiate(scrollbar, new Vector2(866 * 2, 380 * 2), Quaternion.identity) as Dropdown;
            dropdown.transform.parent = GameObject.FindGameObjectsWithTag("canvas")[0].transform;
            //List<Dropdown.OptionData> newData = new List<Dropdown.OptionData>();
            //scrollbar
        }

    }

}
