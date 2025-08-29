using AI.FSM;
using Pathfinding;
using UnityEngine;

namespace AI.States
{
    public class PatrolState : State
    {
        private int _actualIndex;
        private Path _currentPath;

        private bool _waitingForPath;
        private bool _isFollowingPath;

        public PatrolState(Agent agent, StateType stateType) : base(agent, stateType)
        {
        }

        public override void OnEnter()
        {
            Agent.Animator.SetBool("IsChasing", false);
            
            if (Pathfinding.Pathfinding.LineOfSight(Agent.Waypoints[_actualIndex].position, Agent.transform.position,
                    Agent.ObstacleLayer)) return;

            Pathfinding.Pathfinding.Instance.RequestPath(Agent.transform.position,
                Agent.Waypoints[_actualIndex].position,
                PathCallBack, ErrorCallBack);
            _waitingForPath = true;
            _isFollowingPath = true;
        }

        public override void OnUpdate()
        {
            if (_isFollowingPath)
            {
                if (_waitingForPath) return;
                
                var dir = _currentPath.CheckNextNode().transform.position - Agent.transform.position;
                Agent.transform.forward = dir;
                Agent.transform.position += dir.normalized * Agent.Speed * Time.deltaTime;

                if (dir.magnitude > .5f) return;

               _currentPath.GetNextNode();

               if (_currentPath.AnyInPath()) return;
               
               _isFollowingPath = false;
               _waitingForPath = false;
            }
            else
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

        public override void OnExit()
        {
            _isFollowingPath = false;
        }

        private void PathCallBack(Path p)
        {
            _currentPath = p;
            _waitingForPath = false;
        }

        private void ErrorCallBack()
        {
            _isFollowingPath = false;
            _waitingForPath = false;
            Agent.transform.position = Agent.Waypoints[_actualIndex].position;
        }
    }
}