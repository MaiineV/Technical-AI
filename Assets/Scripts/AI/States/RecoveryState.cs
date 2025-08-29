using AI.FSM;
using UnityEngine;

namespace AI.States
{
    public class RecoveryState : State
    {
        public RecoveryState(Agent agent, StateType stateType) : base(agent, stateType)
        { }

        public override void OnEnter()
        {
            Agent.Animator.SetBool("IsWaitingTBA", true);
        }

        public override void OnUpdate()
        {
            Agent.CurrentTba += Time.deltaTime;
        }
        
        public override void OnExit()
        {
            Agent.Animator.SetBool("IsWaitingTBA", false);
        }
    }
}