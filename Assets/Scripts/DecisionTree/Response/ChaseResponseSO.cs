using UnityEngine;

namespace DecisionTree
{
    [CreateAssetMenu(fileName = "ChaseResponseSO", menuName = "Decision Tree/Response Nodes/ChaseResponseSO")]
    public class ChaseResponseSO : BaseDecisionResponseSO
    {
        public override void ExecuteAction()
        {
            Debug.Log("Chase Response");
        }
    }
}