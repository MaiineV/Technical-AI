using System;
using AI;
using UnityEngine;
using UnityEngine.Serialization;

namespace DecisionTree
{
    [Serializable]
    public class DecisionNode
    {
        [SerializeField] public BaseDecisionQuestionSO question;
        [SerializeReference] public DecisionNode trueNode;
        [SerializeReference] public DecisionNode falseNode;
        
        [SerializeField] public BaseDecisionResponseSO action;
        
        [SerializeField] public Vector2 editorPosition;
    }
    
    [RequireComponent(typeof(Agent))]
    public class DecisionTree : MonoBehaviour
    {
        [SerializeField] private DecisionTreeAsset treeAsset;
        private DecisionNode _currentNode;
        
        private Agent _agent;

        private void Awake()
        {
            _agent = GetComponent<Agent>();
        }

        private void Start()
        {
            if (treeAsset != null)
            {
                _currentNode = treeAsset.rootNode;
            }
        }

        private void Update()
        {
            if (_currentNode == null) return;
            
            ExecuteNode(_currentNode);
        }
        
        private void ExecuteNode(DecisionNode node)
        {
            if (node == null) return;
            
            if (node.question)
            {
                var result = node.question.MakeQuestion(_agent);
                var nextNode = result ? node.trueNode : node.falseNode;
                
                if (nextNode != null)
                {
                    ExecuteNode(nextNode);
                }
            }
            else if (node.action)
            {
                node.action.ExecuteAction(_agent);
            }
        }
    }
}
