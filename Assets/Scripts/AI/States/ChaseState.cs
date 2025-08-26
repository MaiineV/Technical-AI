using AI.FSM;
using UnityEngine;

namespace AI.States
{
    public class ChaseState : State
    {
        public ChaseState(Agent agent, StateType stateType) : base(agent, stateType)
        {
        }

        public override void OnEnter()
        {
            //TODO: Animation
        }

        public override void OnUpdate()
        {
            var dir = Agent.Target.transform.position - Agent.transform.position;
            Agent.transform.forward = dir;
            Agent.transform.position += dir.normalized * Agent.Speed * Time.deltaTime;
        }
    }
}
