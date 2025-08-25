using System;
using UnityEngine;

namespace DecisionTree
{
    public class BaseDecisionNodeSO : ScriptableObject
    {
        private Type _type;
        public Type NodeType => _type ??= GetType();
    }
}
