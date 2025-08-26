using System.Linq;
using AI;
using UnityEngine;

namespace DecisionTree.Question
{
    [CreateAssetMenu(fileName = "IsPlayerOnAttackRange", menuName = "Decision Tree/Question Nodes/IsPlayerOnAttackRange")]
    public class IsPlayerOnAttackRangeSO : BaseDecisionQuestionSO
    {
        public override bool MakeQuestion(Agent agent)
        {
            return agent.Target && Vector3.Distance(agent.transform.position, agent.Target.transform.position) < agent.AttackRange;
        }
    }
}
