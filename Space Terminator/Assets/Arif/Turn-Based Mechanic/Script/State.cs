using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class State
{
    public enum STATE { IDLE, PATROL, ATTACK }
    public enum EVENT { ENTER, UPDATE, EXIT }

    protected NavMeshAgent agent;
    protected BattleSystem battleSystem;
    protected Unit unit;
    protected PlayerController player;
    protected State nextState;
    protected EVENT eventName;
    protected STATE stateName;

    public State(NavMeshAgent _agent, BattleSystem _battleSystem, Unit _unit, PlayerController _player)
    {
        agent = _agent;
        battleSystem = _battleSystem;
        unit = _unit;
        player = _player;
        eventName = EVENT.ENTER;
    }

    public virtual void Enter() { eventName = EVENT.UPDATE; }
    public virtual void Update() { eventName = EVENT.UPDATE; }
    public virtual void Exit() { eventName = EVENT.EXIT; }

    public State Process()
    {
        if (eventName == EVENT.ENTER) Enter();
        if (eventName == EVENT.UPDATE) Update();
        if(eventName == EVENT.EXIT)
        {
            Exit();
            return nextState;
        }
        return this;
    }

    public bool IsCurrentTurn()
    {
        return unit.isCurrentTurn;
    }

    public bool CloseToPlayer()
    {
        if(Vector3.Distance(agent.transform.position, player.transform.position) < 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

public class Idle : State
{
    public Idle(NavMeshAgent _agent, BattleSystem _battleSystem, Unit _unit, PlayerController _player) : base(_agent, _battleSystem, _unit, _player)
    {
        stateName = STATE.IDLE;
    }
    
    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        if (IsCurrentTurn())
        {
            nextState = new Attack(agent, battleSystem, unit, player);
            eventName = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}

public class Patrol : State
{
    public Patrol(NavMeshAgent _agent, BattleSystem _battleSystem, Unit _unit, PlayerController _player) : base(_agent, _battleSystem, _unit, _player)
    {
        stateName = STATE.PATROL;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Exit()
    {
        base.Exit();
    }
}

public class Attack : State
{
    public Attack(NavMeshAgent _agent, BattleSystem _battleSystem, Unit _unit, PlayerController _player) : base(_agent, _battleSystem, _unit, _player)
    {
        stateName = STATE.ATTACK;
    }

    public override void Enter()
    {
        agent.SetDestination(player.transform.position);
        base.Enter();
    }

    public override void Update()
    {
        if (!IsCurrentTurn())
        {
            nextState = new Idle(agent, battleSystem, unit, player);
            eventName = EVENT.EXIT;
        }

        if(CloseToPlayer())
        {
            nextState = new Idle(agent, battleSystem, unit, player);
            eventName = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        //battleSystem.ChangeTurn(unit);
        base.Exit();
    }
}
