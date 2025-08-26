using AI;
using AI.FSM;
using UnityEngine;

namespace DecisionTree
{
    [CreateAssetMenu(fileName = "AttackResponseSO", menuName = "Decision Tree/Response Nodes/AttackResponseSO")]
    public class AttackResponseSO : BaseDecisionResponseSO
    {
        public override void ExecuteAction(Agent agent)
        {
            if (agent.CurrentState != StateType.Attack)
            {
                agent.ChangeState(StateType.Attack);
            }
        }
    }
}