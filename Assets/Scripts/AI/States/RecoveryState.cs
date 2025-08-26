using AI.FSM;
using UnityEngine;

namespace AI.States
{
    public class RecoveryState : State
    {
        private int _actualIndex;
        
        public RecoveryState(Agent agent, StateType stateType) : base(agent, stateType)
        { }

        public override void OnEnter()
        {
            //TODO: Animation
        }

        public override void OnUpdate()
        {
            Agent.CurrentTba += Time.deltaTime;
        }
    }
}