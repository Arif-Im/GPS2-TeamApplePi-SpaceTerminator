using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    public Image healthBar;

    public void UpdateHealth(float fraction)
    {
        healthBar.fillAmount = fraction;
    }
}
