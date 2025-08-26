using AI;

namespace DecisionTree
{
    public abstract class BaseDecisionResponseSO : BaseDecisionNodeSO
    {
        public abstract void ExecuteAction(Agent agent);
    }
}