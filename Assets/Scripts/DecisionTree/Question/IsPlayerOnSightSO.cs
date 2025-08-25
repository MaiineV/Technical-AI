using UnityEngine;

namespace DecisionTree.Question
{
    [CreateAssetMenu(fileName = "IsPlayerOnSight", menuName = "Decision Tree/Question Nodes/IsPlayerOnSight")]
    public class IsPlayerOnSightSO : BaseDecisionQuestionSO
    {
        public override bool MakeQuestion()
        {
            Debug.Log("Is On Sight Question");
            return true;
        }
    }
}
