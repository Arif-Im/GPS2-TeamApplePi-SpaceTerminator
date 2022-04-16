using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StarSystem : MonoBehaviour
{
    public static StarSystem instance;

    public GameObject[] stars;
    private int objectiveCounts;

    [SerializeField] int troopCount;
    bool collectedDocuments = false;
    bool reachPointB = false;

    Toggle[] objectiveToggles;

    public GameObject winPanel;
    public GameObject losePanel;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        objectiveCounts = GameObject.FindGameObjectsWithTag("objectives").Length;
        objectiveToggles = transform.Find("Objective Panel").transform.Find("Objectives").GetComponentsInChildren<Toggle>();
    }

    private void Update()
    {
        Objectives();

        if (troopCount <= 0 && !TurnManager.instance.deploymentState)
        {
            losePanel.SetActive(true);
        }
    }

    public void StarsAchieved()
    {
        int objectiveLeft = GameObject.FindGameObjectsWithTag("objectives").Length;

        foreach (Toggle objectiveToggle in objectiveToggles)
        {
            if (objectiveToggle.isOn)
                objectiveLeft--;
        }

        int objectiveCollected = objectiveCounts - objectiveLeft;

        float percentage = float.Parse(objectiveCollected.ToString()) / float.Parse(objectiveCounts.ToString()) * 100f;

        if (percentage >= 33f && percentage < 66)
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

    void Objectives()
    {
        if (troopCount == 4)
            objectiveToggles[0].isOn = true;
        else
            objectiveToggles[0].isOn = false;

        if (collectedDocuments)
            objectiveToggles[1].isOn = true;
        else
            objectiveToggles[1].isOn = false;

        if (reachPointB)
            objectiveToggles[2].isOn = true;
        else
            objectiveToggles[2].isOn = false;
    }

    public void ReachPointB()
    {
        reachPointB = true;
        winPanel.SetActive(true);
        StarsAchieved();
    }

    public void CollectedDocuments()
    {
        collectedDocuments = true;
    }

    public void AddTroop()
    {
        troopCount++;
    }

    public void RemoveTroop()
    {
        troopCount--;
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
