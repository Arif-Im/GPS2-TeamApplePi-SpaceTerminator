using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Healthbar : MonoBehaviour
{

    public Image healthBar;
    [SerializeField] private TextMeshProUGUI healthText;

    public void UpdateHealth(float fraction)
    {
        healthBar.fillAmount = fraction;
    }

    public void UpdateTextHealth(float healthNum)
    {
        healthText.text = healthNum.ToString();
    }

}
