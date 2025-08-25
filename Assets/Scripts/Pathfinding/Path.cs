using System.Collections.Generic;
using System.Linq;

namespace Pathfinding
{
    public class Path
    {
        private readonly Queue<Node> _pathQueue = new Queue<Node>();

        public void AddNode(Node node)
        {
            _pathQueue.Enqueue(node);
        }

        public Node GetNextNode()
        {
            return _pathQueue.Any() ? _pathQueue.Dequeue() : null;
        }

        public Node CheckNextNode()
        {
            return _pathQueue.Any() ?  _pathQueue.Peek() : null;
        }

        public bool AnyInPath()
        {
            return _pathQueue.Any();
        }

        public int Count()
        {
            return _pathQueue.Count;
        }
    }
}