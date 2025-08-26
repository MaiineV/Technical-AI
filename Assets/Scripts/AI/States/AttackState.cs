using AI.FSM;
using UnityEngine;

namespace AI.States
{
    public class AttackState : State
    {
        public AttackState(Agent agent, StateType stateType) : base(agent, stateType)
        {
        }

        public override void OnEnter()
        {
            //TODO: Anim
            Agent.CurrentTba = 0;
        }

        public override void OnUpdate()
        {
            Debug.Log("Attack");
        }
    }
}
