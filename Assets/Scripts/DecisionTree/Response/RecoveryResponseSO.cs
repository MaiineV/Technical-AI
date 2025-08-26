using AI;
using AI.FSM;
using UnityEngine;

namespace DecisionTree
{
    [CreateAssetMenu(fileName = "RecoveryResponseSO", menuName = "Decision Tree/Response Nodes/RecoveryResponseSO")]
    public class RecoveryResponseSO : BaseDecisionResponseSO
    {
        public override void ExecuteAction(Agent agent)
        {
            if (agent.CurrentState != StateType.Recovery)
            {
                agent.ChangeState(StateType.Recovery);
            }
        }
    }
}