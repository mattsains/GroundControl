using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GroundControl
{
    /// <summary>
    /// A node in a graph
    /// </summary>
    class Graph<T>
    {
        public List<Tuple<Graph<T>, string>> edges = new List<Tuple<Graph<T>, string>>();
        public T node;

        public Graph(T node)
        {
            this.node = node;
        }
        public void Connect(Graph<T> node, string tag)
        {
            _FlowConnect(node,tag);
            node._FlowConnect(this,tag);
        }
        public void _FlowConnect(Graph<T> node, string tag)
        {
            if (!hasChild(node))
                edges.Add(new Tuple<Graph<T>,string>(node,tag));
        }
        public bool hasChild(Graph<T> node)
        {
            foreach(Tuple<Graph<T>,string> edge in edges)
                if (edge.Item1==node) return true;
            return false;
        }
    }
}
