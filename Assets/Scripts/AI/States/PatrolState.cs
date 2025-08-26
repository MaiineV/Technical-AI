using AI.FSM;
using UnityEngine;

namespace AI.States
{
    public class PatrolState : State
    {
        private int _actualIndex;
        
        public PatrolState(Agent agent, StateType stateType) : base(agent, stateType)
        {
        }

        public override void OnEnter()
        {
            //TODO: Animation
        }

        public override void OnUpdate()
        {
            var dir = Agent.Waypoints[_actualIndex].position - Agent.transform.position;
            Agent.transform.forward = dir;
            Agent.transform.position += dir.normalized * Agent.Speed * Time.deltaTime;

            if (dir.magnitude > .5f) return;
            
            _actualIndex++;

            if (_actualIndex >= Agent.Waypoints.Length)
            {
                _actualIndex = 0;
            }
        }
    }
}
