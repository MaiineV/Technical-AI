using UnityEngine;

namespace DecisionTree
{
    [CreateAssetMenu(fileName = "PatrolResponse", menuName = "Decision Tree/Response Nodes/PatrolResponse")]
    public class PatrolResponseSO : BaseDecisionResponseSO
    {
        public override void ExecuteAction()
        {
            Debug.Log("Patrol Response");
        }
    }
}
