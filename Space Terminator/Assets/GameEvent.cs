using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent : MonoBehaviour
{
    public static GameEvent Singleton;
    private List<TacticMove> listOfCharacter = new List<TacticMove>();

    private void Awake()
    {
        Singleton = this;
    }

    public void AddTacticMove(TacticMove tacticMove)
    {
        listOfCharacter.Add(tacticMove);
    }

    //public bool OverwatchEffect()
    //{
    //    if()
    //}
}
