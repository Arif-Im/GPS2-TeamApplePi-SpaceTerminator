using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    [SerializeField] List<Unit> unitList;

    private void Start()
    {
        foreach(Unit unit in unitList)
        {
            unit.isCurrentTurn = false;
        }

        unitList[0].isCurrentTurn = true;
    }

    public void ChangeTurn(Unit unit, int maxPoints)
    {
        int currentUnit = 0;

        for (int i = 0; i < unitList.Count; i++)
        {
            if (unit == unitList[i])
            {
                currentUnit = i;
            }
            unitList[i].isCurrentTurn = false;
        }

        if (currentUnit >= unitList.Count - 1)
        {
            unitList[0].GetComponent<UnitPoitsSystem>().CurrentPoints = maxPoints;
            unitList[0].isCurrentTurn = true;
        }
        else
        {
            unitList[currentUnit + 1].GetComponent<UnitPoitsSystem>().CurrentPoints = maxPoints;
            unitList[currentUnit + 1].isCurrentTurn = true;
        }
    }
}
