using System.Linq;
using AI;
using AI.FSM;
using UnityEngine;

namespace DecisionTree
{
    [CreateAssetMenu(fileName = "ChaseResponseSO", menuName = "Decision Tree/Response Nodes/ChaseResponseSO")]
    public class ChaseResponseSO : BaseDecisionResponseSO
    {
        public override void ExecuteAction(Agent agent)
        {
            var closestPlayer =
                Physics.OverlapSphere(agent.transform.position, agent.SearchRadius, agent.TargetLayer);

            agent.Target = closestPlayer[0].transform;

            if (agent.CurrentState != StateType.Chase)
            {
                agent.ChangeState(StateType.Chase);
            }
        }
    }
}