using System;
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
    
    public class DecisionTree : MonoBehaviour
    {
        [SerializeField] private DecisionTreeAsset treeAsset;
        private DecisionNode _currentNode;

        private void Start()
        {
            Debug.Log("Starting Decision Tree");
            if (treeAsset != null)
            {
                _currentNode = treeAsset.rootNode;
            }
            
            ExecuteNode(_currentNode);
        }

        private void Update()
        {
            if (_currentNode == null) return;
            
            
        }
        
        private void ExecuteNode(DecisionNode node)
        {
            if (node == null) return;
            
            if (node.question)
            {
                var result = node.question.MakeQuestion();
                var nextNode = result ? node.trueNode : node.falseNode;
                
                if (nextNode != null)
                {
                    ExecuteNode(nextNode);
                }
            }
            else if (node.action)
            {
                node.action.ExecuteAction();
            }
        }
    }
}
