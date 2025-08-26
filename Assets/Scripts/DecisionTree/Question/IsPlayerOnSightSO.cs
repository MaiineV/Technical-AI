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
                   Pathfinding.Pathfinding.OnSight(closestPlayer[0].transform.position, 
                       agent.transform.position, agent.ObstacleLayer);
        }
    }
}
