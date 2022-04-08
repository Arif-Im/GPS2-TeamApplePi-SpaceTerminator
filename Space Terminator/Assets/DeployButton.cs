using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeployButton : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => TurnManager.instance.Deploy());
        GetComponent<Button>().onClick.AddListener(() => gameObject.SetActive(false));

    }
}
