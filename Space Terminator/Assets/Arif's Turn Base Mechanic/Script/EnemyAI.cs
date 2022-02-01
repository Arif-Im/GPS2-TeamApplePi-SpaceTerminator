using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    State currentState;
    Unit unit;
    NavMeshAgent agent;
    PlayerController player;
    BattleSystem battleSystem;

    private void Awake()
    {
        unit = GetComponent<Unit>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
        battleSystem = FindObjectOfType<BattleSystem>();
        currentState = new Idle(agent, battleSystem, unit, player);
    }

    // Update is called once per frame
    void Update()
    {
        currentState = currentState.Process();
    }
}
