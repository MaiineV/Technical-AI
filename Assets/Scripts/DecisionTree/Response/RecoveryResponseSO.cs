using UnityEngine;

namespace DecisionTree
{
    [CreateAssetMenu(fileName = "RecoveryResponseSO", menuName = "Decision Tree/Response Nodes/RecoveryResponseSO")]
    public class RecoveryResponseSO : BaseDecisionResponseSO
    {
        public override void ExecuteAction()
        {
            Debug.Log("Recovery Response");
        }
    }
}