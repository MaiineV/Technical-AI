using System;
using System.Collections.Generic;
using AI.States;
using UnityEngine;

namespace AI.FSM
{
    public enum StateType
    {
        Patrol,
        Chase,
        Attack,
        Recovery
    }

    [Serializable]
    public struct StateData
    {
        public StateType actualState;
        public StateType[] transitions;
    }

    public class FSMComponent
    {
        private readonly Dictionary<StateType, State> _states = new();
        private State _currentState;
        public StateType CurrentState => _currentState.StateType;

        public FSMComponent(StateData[] statesData, Agent agent, StateType initialState)
        {
            foreach (var stateData in statesData)
            {
                switch (stateData.actualState)
                {
                    case StateType.Patrol:
                        _states.Add(StateType.Patrol, new PatrolState(agent, stateData.actualState));
                        break;
                    case StateType.Chase:
                        _states.Add(StateType.Chase, new ChaseState(agent, stateData.actualState));
                        break;
                    case StateType.Attack:
                        _states.Add(StateType.Attack, new AttackState(agent, stateData.actualState));
                        break;
                    case StateType.Recovery:
                        _states.Add(StateType.Recovery, new RecoveryState(agent, stateData.actualState));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            foreach (var stateData in statesData)
            {
                foreach (var transition in stateData.transitions)
                {
                    _states[stateData.actualState].AddTransition(transition, _states[transition]);
                }
            }

            ChangeState(initialState);
        }

        public void OnUpdate()
        {
            _currentState.OnUpdate();
        }

        public void ChangeState(StateType newState)
        {
            if (!_states.ContainsKey(newState) ||
                (_currentState != null && !_currentState.CanTransition(newState))) return;

            _currentState?.OnExit();
            _currentState = _states[newState];
            _currentState.OnEnter();
        }
    }
}