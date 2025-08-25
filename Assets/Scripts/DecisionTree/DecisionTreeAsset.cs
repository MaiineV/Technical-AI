using UnityEngine;

namespace DecisionTree
{
    [CreateAssetMenu(fileName = "DecisionTree", menuName = "Decision Tree/Tree Asset")]
    public class DecisionTreeAsset : ScriptableObject
    {
        [SerializeReference] public DecisionNode rootNode;
    }
}