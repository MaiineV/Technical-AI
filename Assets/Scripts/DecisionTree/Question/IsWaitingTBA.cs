using AI;
using AI.FSM;
using UnityEngine;

namespace DecisionTree.Question
{
    [CreateAssetMenu(fileName = "IsWaitingTBA", menuName = "Decision Tree/Question Nodes/IsWaitingTBA")]
    public class IsWaitingTBA : BaseDecisionQuestionSO
    {
        public override bool MakeQuestion(Agent agent)
        {
            return agent.CurrentTba < agent.Tba;
        }
    }
}