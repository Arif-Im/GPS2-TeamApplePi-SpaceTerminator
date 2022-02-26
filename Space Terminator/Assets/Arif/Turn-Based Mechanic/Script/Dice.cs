using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum DiceRoll { Idle, Rolling, EndRoll }

public class Dice : MonoBehaviour
{
    Sprite[] diceSide;
    SpriteRenderer sr;
    int finalSide;

    public int FinalSide { get => finalSide; }
    public DiceRoll state;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        diceSide = Resources.LoadAll<Sprite>("Dice Sides");
    }

    public void RollDice()
    {
        StartCoroutine(RollingTheDice());
    }

    private void OnMouseDown()
    {
        StartCoroutine(RollingTheDice());
    }

    IEnumerator RollingTheDice()
    {
        int randomSide = 0;

        for(int i = 0; i < 20; i++)
        {
            randomSide = UnityEngine.Random.Range(0, 6);

            sr.sprite = diceSide[randomSide];

            yield return new WaitForSeconds(.05f);
        }

        finalSide = randomSide + 1;
    }
}