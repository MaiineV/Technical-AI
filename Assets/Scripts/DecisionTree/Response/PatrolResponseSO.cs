using AI;
using AI.FSM;
using UnityEngine;

namespace DecisionTree
{
    [CreateAssetMenu(fileName = "PatrolResponse", menuName = "Decision Tree/Response Nodes/PatrolResponse")]
    public class PatrolResponseSO : BaseDecisionResponseSO
    {
        public override void ExecuteAction(Agent agent)
        {
            if (agent.CurrentState != StateType.Patrol)
            {
                agent.ChangeState(StateType.Patrol);
            }
        }
    }
}
