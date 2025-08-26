using System.Collections.Generic;

namespace AI.FSM
{
    public abstract class State
    {
        protected readonly Agent Agent;
        private readonly Dictionary<StateType, State> _transitions = new();
        public StateType StateType { get; }

        public virtual void OnEnter(){}
        public virtual void OnUpdate(){}
        public virtual void OnExit(){}

        protected State(Agent agent, StateType stateType)
        {
            Agent = agent;
            StateType = stateType;
        }

        public void AddTransition(StateType type, State nextState)
        {
            _transitions.Add(type, nextState);
        }

        public bool CanTransition(StateType type)
        {
            return _transitions.ContainsKey(type);
        }
    }
}