using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GroundControl
{
    class Graph<T>
    {
        public List<T> Vertices=new List<T>();
        List<Edge<T>> Edges = new List<Edge<T>>();

        public void Connect(T from, T to, string tag, int weight=0)
        {
            if (Vertices.Contains(from) && Vertices.Contains(to))
                Edges.Add(new Edge<T>(from, to, tag, weight));
        }
        public void Disconnect(T from, T to)
        {
            if (IsAdjacent(from,to))
            {
                Edges.Remove(_GetEdge(from, to));
            }
        }
        public List<T> IncidentTo(T node)
        {
            List<T> temp = new List<T>();
            foreach (Edge<T> edge in Edges)
            {
                if (edge.From.Equals(node)) temp.Add(edge.To);
                if (edge.To.Equals(node)) temp.Add(edge.From);
            }
            return temp;
        }
        public bool IsAdjacent(T from, T to)
        {
            return IncidentTo(from).Contains(to);
        }
        public string GetTag(T from, T to)
        {
            if (IsAdjacent(from, to))
                return _GetEdge(from, to).Tag;
            else return null;
        }
        public int GetWeight(T from, T to)
        {
            if (IsAdjacent(from, to))
                return _GetEdge(from, to).Weight;
            else return int.MaxValue;
        }

        public Stack<T> Dijkstra(T source, T target)
        {
            Dictionary<T, int> dist = new Dictionary<T, int>();
            Dictionary<T, T> previous = new Dictionary<T, T>();
            Heap<T> queue = new Heap<T>();
            foreach (T v in Vertices)                                // Initializations
            {
                dist[v] = int.MaxValue;                                  // Unknown distance function from 
                // source to v
                previous[v] = default(T);      // Previous node in optimal path
                queue.Push(v,int.MaxValue);
            }                                             // from source

            dist[source] = 0;                                        // Distance from source to source

            // All nodes in the graph are
            // unoptimized - thus are in Q
            T u = default(T); // Start node in first case
            while (queue.Count > 0)// The main loop
            {

                int minDist = int.MaxValue;
                foreach (KeyValuePair<T,int> distance in dist)
                    if (minDist > distance.Value && queue.Contains(distance.Key))
                    {
                        u = distance.Key;
                        minDist = distance.Value;
                    }

                queue.Remove(u);
                if (u.Equals(target))
                    break;

                if (dist[u] == int.MaxValue)
                    break;                                             // inaccessible from source

                foreach (T v in IncidentTo(u))// where v has not yet been
                {                                                    // removed from Q.
                    int alt = dist[u] + GetWeight(u, v);
                    if (alt < dist[v])                                 // Relax (u,v,a)
                    {
                        dist[v] = alt;
                        previous[v] = u;
                        queue.Remove(v);                           // Reorder v in the Queue
                        queue.Push(v, dist[v]);
                    }
                }
            }
            Stack<T> S = new Stack<T>();
            u = target;
            while (previous[u]!=null)                                   // Construct the shortest path with a stack S
            {
                S.Push(u);                          // Push the vertex into the stack
                u = previous[u];                                            // Traverse from target to source
            }
            return S;
        }



        private Edge<T> _GetEdge(T from, T to)
        {
            foreach (Edge<T> edge in Edges)
            {
                if (edge.From.Equals(from) && edge.To.Equals(to) ||
                    edge.From.Equals(to) && edge.To.Equals(from))
                    return edge;
            }
            return null;
        }
    }

    /// <summary>
    /// Implements an edge in a graph with two end points, a tag (name) and a weight value
    /// </summary>
    /// <typeparam name="T">This is a generic class</typeparam>
    class Edge<T>
    {
        public T From { get; set; }
        public T To { get; set; }
        public string Tag { get; set; }
        public int Weight { get; set; }

        public Edge(T from, T to, string tag, int weight=0)
        {
            this.From = from;
            this.To = to;
            this.Tag = tag;
            this.Weight = weight;
        }   
    }
}
