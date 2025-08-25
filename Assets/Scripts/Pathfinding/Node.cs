using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Util;

namespace Pathfinding
{
    public class Node : MonoBehaviour, IWeighted
    {
        [SerializeField] public List<Node> neighbors = new();

        private float _weight = 0;
        public Node previousNode = null;

        public void AddNeighbor(Node neighbor)
        {
            if (neighbor && !neighbors.Contains(neighbor))
            {
            
                neighbors.Add(neighbor);
            }
        }

        public void RemoveNeighbor(Node neighbor)
        {
            var index = neighbors.IndexOf(neighbor);
            neighbors.RemoveAt(index);
        }

        public bool CheckNeighbor(Node neighbor)
        {
            return neighbors.Contains(neighbor);
        }

        public void ClearNeighbours()
        {
            neighbors = new List<Node>();
        }

        public bool HasNeighbours()
        {
            return neighbors.Count > 0;
        }

        public Node GetNeighbor(int index)
        {
            return neighbors[index];
        }

        public int NeighboursCount()
        {
            return neighbors.Count;
        }

        public void SetWeight(float weight)
        {
            _weight = weight;
        }

        public void ResetNode()
        {
            _weight = 0;
            previousNode = null;
        }

        public void DestroyNode()
        {
            foreach (var t in neighbors)
            {
                t.RemoveNeighbor(this);
            }
        }
    
    
        public bool isOnPath = false;
    
        private void OnDrawGizmos()
        {
            Gizmos.color = isOnPath? Color.yellow : Color.green;
            Gizmos.DrawWireSphere(transform.position, 0.2f);
        
            Gizmos.color = Color.red;
            if (neighbors.Any())
            {
                foreach (var neighbor in neighbors)
                {
                    Gizmos.DrawLine(transform.position, neighbor.transform.position);
                }
            }
        }

        public float Weight => _weight;
    }
}