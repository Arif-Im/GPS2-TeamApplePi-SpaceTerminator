using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum AttacksState { Idle, UnderAttack, FinishAttacked, FinishRoll }

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;

    static Dictionary<string, List<TacticMove>> units = new Dictionary<string, List<TacticMove>>();
    static ConditionalQueue<string> turnKey = new ConditionalQueue<string>();
    public static ConditionalQueue<TacticMove> turnTeam = new ConditionalQueue<TacticMove>();
    public AttacksState attackState;

    public bool deploymentState = true;
    bool speedUp = false;

    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Deploy();
        }

        if (speedUp)
            SetTimeScale(5);
        else
            SetTimeScale(1);

        if (deploymentState) return;

        if (turnTeam.Count == 0)
        {
            InitTeamTurnQueue();
        }
    }

    public void Deploy()
    {
        deploymentState = false;
    }

    public void SpeedUp()
    {
        speedUp = !speedUp;
    }

    static void InitTeamTurnQueue()
    {
        List<TacticMove> teamList = units[turnKey.Peek()];

        foreach (TacticMove unit in teamList)
        {
            turnTeam.Enqueue(unit);
        }

        StartTurn();
    }

    static void StartTurn()
    {
        if (turnTeam.Count > 0)
        {
            turnTeam.Peek().BeginTurn();
        }
    }

    public static void EndTurn()
    {
        TacticMove unit = turnTeam.Dequeue();
        ButtonManager.instance.ResetButtons();
        unit.EndTurn();

        if (turnTeam.Count > 0)
        {
            StartTurn();
        }
        else
        {
            string team = turnKey.Dequeue();
            turnKey.Enqueue(team);
            InitTeamTurnQueue();
        }

    }

    public static void AddUnit(TacticMove unit)
    {
        List<TacticMove> list;
        Debug.Log($"ADD: {unit.name}");

        if (!units.ContainsKey(unit.tag))
        {
            list = new List<TacticMove>();
            units[unit.tag] = list;

            if (!turnKey.Contains(unit.tag))
            {
                turnKey.Enqueue(unit.tag);
            }
        }
        else
        {
            list = units[unit.tag];
        }

        list.Add(unit);
    }
    public static void RemoveUnit(TacticMove unit)
    {
        //turnKey.Dequeue(unit.tag);
        if (units[unit.tag].Contains(unit))
        {
            units[unit.tag].Remove(unit);
        }
        //turnTeam.Dequeue(unit);
    }

    private void SetTimeScale(float scale)
    {

        Time.timeScale = scale;
        Time.fixedDeltaTime = scale * 0.02f;

    }
}
