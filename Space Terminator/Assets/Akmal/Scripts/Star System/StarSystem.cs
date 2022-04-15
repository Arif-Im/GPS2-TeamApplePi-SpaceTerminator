using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StarSystem : MonoBehaviour
{
    public GameObject[] stars;
    private int objectiveCounts;

    void Start()
    {
        objectiveCounts = GameObject.FindGameObjectsWithTag("objectives").Length;
    }

    public void StarsAchieved()
    {
        int objectiveLeft = GameObject.FindGameObjectsWithTag("objectives").Length;
        int objectiveCollected = objectiveCounts - objectiveLeft;

        float percentage = float.Parse(objectiveCollected.ToString()) / float.Parse(objectiveCounts.ToString()) * 100f;

        if(percentage >= 33f && percentage < 66)
        {
            stars[0].SetActive(true);
        }
        else if (percentage >= 66 && percentage < 70)
        {
            stars[0].SetActive(true);
            stars[1].SetActive(true);
        }
        else
        {
            stars[0].SetActive(true);
            stars[1].SetActive(true);
            stars[2].SetActive(true);
        }
    }

    public void RestartButton()
    {
        SceneManager.LoadScene("LobbyMenu");
    }

    public void MenuButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
