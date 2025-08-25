#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using UnityEditor;
using UnityEngine;

namespace Tools.NodeGenerator
{
    [ExecuteInEditMode]
    public class NodesGenerator : MonoBehaviour
    {
        [Header("Mask Variables")]
        [SerializeField] private LayerMask floorMask;
        [SerializeField] private LayerMask obstacleMask;
        [SerializeField] private LayerMask wallMask;
        [SerializeField] private LayerMask nodeMask;
        
        [Header("Generator Variables")]
        [SerializeField] private Vector3 area;

        [SerializeField] private Transform parent;
        
        [Header("Node Variables")]
        [SerializeField] private float nodeSize;
        private float NodeDistance => nodeSize * 2.5f;
        [SerializeField] private Node prefab;

        [SerializeField] private List<GameObject> nodeList = new();
        private HashSet<Vector3> _spawnedNodes = new();

        private float MaxX => transform.position.x + area.x / 2;
        private float MinY => transform.position.y - area.y / 2;

        private float MinZ => transform.position.z - area.z / 2;
        private float MaxZ => MinZ + area.z;

        private int _multiplyDir = -1;
        

        public void Generate()
        {
            _spawnedNodes = new HashSet<Vector3>();
            var actualStartRay = transform.position;
            actualStartRay.x -= area.x / 2;
            actualStartRay.y += area.y / 2;
            actualStartRay.z += area.z / 2;

            var count = 0;
            while (actualStartRay.y >= MinY)
            {
                actualStartRay.x = transform.position.x - (area.x / 2);
                actualStartRay.z = transform.position.z + area.z / 2;
            
                while (actualStartRay.x <= MaxX)
                {
                    var ray = new Ray(actualStartRay, Vector3.down);

                    if (Physics.Raycast(ray, out var hit, Mathf.Infinity, floorMask))
                    {
                        if (!Physics.Raycast(ray, (hit.point - actualStartRay).magnitude, obstacleMask))
                        {
                            if (!_spawnedNodes.Contains(hit.point + Vector3.up))
                            {                       
                                var actualNode = PrefabUtility.InstantiatePrefab(prefab, parent) as Node;
                                actualNode.gameObject.name += count;
                                actualNode.transform.position = hit.point + Vector3.up;
                                _spawnedNodes.Add(hit.point + Vector3.up);

                                nodeList.Add(actualNode.gameObject);
                            }
                        }
                    }

                    var distance = NodeDistance;
                    distance *= _multiplyDir;
                    actualStartRay.z += distance;

                    if (actualStartRay.z > MaxZ || actualStartRay.z < MinZ)
                    {
                        actualStartRay.z -= distance;
                        _multiplyDir *= -1;
                        actualStartRay.x += NodeDistance;
                    }

                    count++;
                }
            
                _multiplyDir = -1;
                actualStartRay.y -= 1;
            }
       

            _multiplyDir = -1;
        }

        public void DeleteNodes()
        {
            if (!nodeList.Any()) return;

            foreach (var node in nodeList)
            {
                DestroyImmediate(node);
            }

            nodeList = new List<GameObject>();
        }

        public void GenerateNeighbours()
        {
            foreach (var node in nodeList.Select(actualNode => actualNode.GetComponent<Node>()))
            {
                Undo.RecordObject(node, "SetNeighbours");
                node.ClearNeighbours();
            }

            for (var i = 0; i < 2; i++)
            {
                foreach (var actualNode in nodeList.Select(actualNode => actualNode.GetComponent<Node>()))
                {
                    Undo.RecordObject(actualNode, "SetNeighbours");
                    var neighbours =
                        Physics.OverlapSphere(actualNode.transform.position, NodeDistance, nodeMask);

                    foreach (var neighbour in neighbours)
                    {
                        var checkingNeighbour = neighbour.gameObject.GetComponent<Node>();
                    
                        if (checkingNeighbour == actualNode) continue;

                        if (actualNode.CheckNeighbor(checkingNeighbour)) continue;

                        var dir = neighbour.transform.position - actualNode.transform.position;
                        var rayWallChecker = new Ray(actualNode.transform.position, dir);

                        if (Physics.Raycast(rayWallChecker, NodeDistance, wallMask)) continue;

                        checkingNeighbour.AddNeighbor(actualNode);
                        actualNode.AddNeighbor(checkingNeighbour);
                    }
                    PrefabUtility.RecordPrefabInstancePropertyModifications(actualNode);
                }

                ClearNodes();
            }
        }

        private  void ClearNodes()
        {
            var nodesToDelete = new Queue<GameObject>();

            foreach (var node in from node in nodeList let colliders = Physics.OverlapSphere(node.transform.position, nodeSize, nodeMask).Select(x=>x.gameObject) let closestNodes = colliders.Where(x=>x != node && !nodesToDelete.Contains(x)).Select(x => x) let mNode = node.GetComponent<Node>() where Physics.CheckSphere(node.transform.position, nodeSize, obstacleMask) || !mNode.HasNeighbours() || closestNodes.Any() select node)
            {
                nodesToDelete.Enqueue(node);
            }

            while (nodesToDelete.Count > 0)
            {
                var node = nodesToDelete.Dequeue();
                nodeList.Remove(node);
                node.GetComponent<Node>().DestroyNode();

                DestroyImmediate(node);
            }

            nodeList = nodeList.Where(x => x != null).ToList();
        }

        public void RemoveNulls()
        {
            nodeList = nodeList.Where(x => x != null).ToList();
        }

        public void RemoveMissing()
        {
            var mNodes = nodeList.Select(x => x.GetComponent<Node>());
            foreach (var node in mNodes)
            {
                node.neighbors = node.neighbors.Where(x => true).ToList();
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position, area);
        }
    }
}

#endif