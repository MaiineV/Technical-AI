using UnityEngine;

namespace DecisionTree.Question
{
    [CreateAssetMenu(fileName = "IsPlayerOnAttackRange", menuName = "Decision Tree/Question Nodes/IsPlayerOnAttackRange")]
    public class IsPlayerOnAttackRangeSO : BaseDecisionQuestionSO
    {
        public override bool MakeQuestion()
        {
            Debug.Log("AttackRange Question");
            return true;
        }
    }
}
