using UnityEngine;

namespace DecisionTree.Question
{
    [CreateAssetMenu(fileName = "IsWaitingTBA", menuName = "Decision Tree/Question Nodes/IsWaitingTBA")]
    public class IsWaitingTBA : BaseDecisionQuestionSO
    {
        public override bool MakeQuestion()
        {
            Debug.Log("Waiting TBA Question");
            return false;
        }
    }
}