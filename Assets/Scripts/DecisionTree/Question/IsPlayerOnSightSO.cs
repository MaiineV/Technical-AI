using System.Linq;
using AI;
using UnityEngine;

namespace DecisionTree.Question
{
    [CreateAssetMenu(fileName = "IsPlayerOnSight", menuName = "Decision Tree/Question Nodes/IsPlayerOnSight")]
    public class IsPlayerOnSightSO : BaseDecisionQuestionSO
    {
        public override bool MakeQuestion(Agent agent)
        {
            var closestPlayer = 
                Physics.OverlapSphere(agent.transform.position, agent.SearchRadius, agent.TargetLayer);
            
            return closestPlayer.Any() && 
                   Pathfinding.Pathfinding.FieldOfView(closestPlayer[0].transform, 
                       agent.transform, agent.FieldOfView, agent.ObstacleLayer);
        }
    }
}
