using UnityEngine;

namespace DecisionTree
{
    [CreateAssetMenu(fileName = "AttackResponseSO", menuName = "Decision Tree/Response Nodes/AttackResponseSO")]
    public class AttackResponseSO : BaseDecisionResponseSO
    {
        public override void ExecuteAction()
        {
            Debug.Log("Attack");
        }
    }
}