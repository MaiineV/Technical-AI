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
            Agent.Animator.SetTrigger("Attack");
            Agent.CurrentTba = 0;
        }

        public override void OnUpdate()
        {
            Debug.Log("Attack");
        }
    }
}
