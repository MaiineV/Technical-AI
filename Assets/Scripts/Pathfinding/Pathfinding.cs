using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Util;

namespace Pathfinding
{
    public struct RequestData
    {
        public Vector3 InitPosition;
        public Vector3 TargetPosition;
        public Action<Path> Callback;
        public Action ErrorCallback;
    }
    
    public class Pathfinding : MonoBehaviour
    {
        [SerializeField] private LayerMask nodeMask;
        [SerializeField] private LayerMask obstacleMask;
        
        public static Pathfinding Instance;
        private Path _actualPath;
        private Node _origenNode;
        private Node _targetNode;
        private Node _actualNode;
        public float searchingRange;

        [SerializeField] private int watchdogValue = 1500;

        private HashSet<Node> _closeNodes = new();
        private PriorityQueue<Node> _openNodes = new();

        private Action _clearNodes = delegate { };

        private Coroutine _getPathCoroutine;
        private bool _doingPath = false;
        private Coroutine _aStartCoroutine;
        private bool _doingAStart = false;

        private readonly Queue<RequestData> _requestQueue = new();

        private void Awake()
        {
            Instance = this;
            StartCoroutine(LazyPathFinding());
        }

        private IEnumerator LazyPathFinding()
        {
            while (true)
            {
                if (!_requestQueue.Any())
                {
                    yield return null;
                    continue;
                }

                var actualRequest = _requestQueue.Dequeue();

                _doingPath = true;
                _getPathCoroutine = StartCoroutine(GetPath(actualRequest.InitPosition, 
                    actualRequest.TargetPosition));

                yield return new WaitUntil(() => _doingPath == false);

                if (_actualPath != null && _actualPath.AnyInPath()) actualRequest.Callback(_actualPath);
                else actualRequest.ErrorCallback();

                yield return null;
            }
        }

        public void RequestPath(Vector3 origen, Vector3 target, Action<Path> successCallBack, Action failCallBack)
        {
            _requestQueue.Enqueue(new RequestData
            {
                InitPosition = origen,
                TargetPosition = target,
                Callback = successCallBack,
                ErrorCallback = failCallBack
            });
        }

        private IEnumerator GetPath(Vector3 origen, Vector3 target)
        {
            _actualPath = new Path();
            _closeNodes = new HashSet<Node>();
            _openNodes = new PriorityQueue<Node>();

            _origenNode = GetClosestNode(origen);

            if (!_origenNode)
            {
                _doingPath = false;
                yield break;
            }

            _targetNode = GetClosestNode(target);

            if (!_targetNode)
            {
                _doingPath = false;
                yield break;
            }

            _actualNode = _origenNode;

            _doingAStart = true;
            _aStartCoroutine = StartCoroutine(AStar());

            yield return new WaitUntil(() => _doingAStart == false);

            _clearNodes();
            _clearNodes = delegate { };
            _doingPath = false;
        }

        private IEnumerator AStar()
        {
            if (!_actualNode)
            {
                _doingAStart = false;
                yield break;
            }
            
            _closeNodes.Add(_actualNode);

            var watchdog = watchdogValue;
            var loopsUntilLazy = 300;

            while (!OnSight(_actualNode.transform.position, _targetNode.transform.position, obstacleMask) && watchdog > 0)
            {
                watchdog--;
                loopsUntilLazy--;

                if (!_actualNode)
                {
                    _doingAStart = false;
                    yield break;
                }

                foreach (var node in _actualNode.neighbors.Where(node => !_closeNodes.Contains(node) &&
                                                                         (!node.previousNode ||
                                                                          !(node.previousNode.Weight <
                                                                            _actualNode.Weight))))
                {
                    if (!node)
                    {
                        _doingAStart = false;
                        yield break;
                    }

                    node.previousNode = _actualNode;

                    node.SetWeight(_actualNode.Weight + 1 +
                                   Vector3.Distance(node.transform.position, _targetNode.transform.position));

                    _clearNodes += node.ResetNode;

                    _openNodes.Enqueue(node);
                }

                if (!_openNodes.IsEmpty)
                    _actualNode = _openNodes.Dequeue();
                else
                {
                    _doingAStart = false;
                    yield break;
                }

                _closeNodes.Add(_actualNode);

                if (loopsUntilLazy > 0) continue;

                yield return null;
                loopsUntilLazy = 300;
            }

            if (watchdog <= 0)
            {
                _doingAStart = false;
                yield break;
            }

            _targetNode.previousNode = _actualNode;

            ThetaStar();
            _doingAStart = false;
        }

        private void ThetaStar()
        {
            var stack = new Stack<Node>();
            stack.Push(_targetNode);
            _actualNode = _targetNode;
            var previousNode = _actualNode.previousNode;

            if (!previousNode)
            {
                _actualPath = null;
                return;
            }

            var watchdog = watchdogValue;
            while (!OnSight(_actualNode.transform.position, _origenNode.transform.position, obstacleMask)
                   && watchdog > 0)
            {
                if (Mathf.Abs(_actualNode.transform.position.y - previousNode.transform.position.y) > .1f)
                {
                    _actualNode.previousNode = previousNode;
                    _actualNode = previousNode;
                    stack.Push(_actualNode);
                    continue;
                }

                watchdog--;

                if (previousNode.previousNode &&
                    OnSight(_actualNode.transform.position, previousNode.previousNode.transform.position, obstacleMask))
                {
                    previousNode = previousNode.previousNode;
                }
                else
                {
                    _actualNode.previousNode = previousNode;
                    _actualNode = previousNode;
                    stack.Push(_actualNode);
                }
            }

            while (stack.Count > 0)
            {
                var nextNode = stack.Pop();
                _actualPath.AddNode(nextNode);
            }
        }

        private Node GetClosestNode(Vector3 t, bool isForAssistant = false)
        {
            var actualSearchingRange = searchingRange;
            var closestNodes = Physics.OverlapSphere(t, actualSearchingRange, nodeMask)
                .Where(x => OnSight(t, x.transform.position, obstacleMask));

            var watchdog = 100;
            while (!closestNodes.Any())
            {
                watchdog--;
                if (watchdog <= 0)
                {
                    return null;
                }

                actualSearchingRange += searchingRange;
                closestNodes = Physics.OverlapSphere(t, actualSearchingRange, nodeMask)
                    .Where(x => OnSight(t, x.transform.position, obstacleMask));
            }

            return closestNodes.OrderBy(x => Vector3.Distance(t, x.transform.position)).First().GetComponent<Node>();
        }

        public static bool OnSight(Vector3 from, Vector3 to, LayerMask mask)
        {
            var dir = to - from;

            return !Physics.Raycast(from, dir, dir.magnitude, mask);
        }
    }
}