using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GroundControl
{
    class Graph<T>
    {
        public List<T> Vertices = new List<T>();
        List<Edge<T>> Edges = new List<Edge<T>>();

        public void Connect(T from, T to, string tag, int weight = 0)
        {
            if (Vertices.Contains(from) && Vertices.Contains(to))
                Edges.Add(new Edge<T>(from, to, tag, weight));
        }
        public void Disconnect(T from, T to)
        {
            if (IsAdjacent(from, to))
            {
                Edges.Remove(_GetEdge(from, to));
            }
        }
        public List<T> IncidentTo(T node, Edge<T> exclude = default(Edge<T>))
        {
            List<T> temp = new List<T>();
            foreach (Edge<T> edge in Edges)
            {
                if (!edge.Equals(exclude))
                {
                    if (edge.From.Equals(node) && !edge.To.Equals(exclude)) temp.Add(edge.To);
                    if (edge.To.Equals(node) && !edge.From.Equals(exclude)) temp.Add(edge.From);
                }
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

        public Stack<T> Dijkstra(T source, T target, T exclude)
        {

            Dictionary<T, int> dist = new Dictionary<T, int>(); //this will store the vertex "potential" distance
            Dictionary<T, T> previous = new Dictionary<T, T>(); //this will store a vertex closer to the source than each vertex
            Heap<T> queue = new Heap<T>();
            
            Edge<T> eexclude = default(Edge<T>);
            if (IncidentTo(source).Count != 1 /*&& !target.Equals(exclude)*/)//if there's no other way, do not exclude it. Commented code: not sure why this is here, maybe an edge case I haven't considered.
                eexclude = _GetEdge(source, exclude);
            foreach (T v in Vertices)
            {
                dist[v] = int.MaxValue;       //set the potentials to "infinity" - they are uncalculated as yet                           
                previous[v] = default(T);     // Previous node in optimal path
                queue.Push(v, int.MaxValue);   //add everything to the queue to be processed/followed
            }

            dist[source] = 0; //distance from the source to itself if zero                        

            // intialize u - the current neighbour being followed
            T u = default(T);
            while (queue.Count > 0)// The main loop
            {

                int minDist = int.MaxValue;
                foreach (KeyValuePair<T, int> distance in dist)
                    if (minDist > distance.Value && queue.Contains(distance.Key))
                    {
                        u = distance.Key;
                        minDist = distance.Value; //find the closest vertex in the process queue
                    }

                queue.Remove(u); //remove this vertex from the queue
                if (u.Equals(target)) //if we're at the target, we've found the shortes path
                    break;

                if (dist[u] == int.MaxValue)// no incident vertices are in the queue.
                    break;                  // this means that there is no path

                foreach (T v in IncidentTo(u, eexclude)) // calculate the neighbours' distances in preparation of the next iteration.
                {
                    int alt = dist[u] + GetWeight(u, v);
                    if (alt < dist[v])
                    {
                        dist[v] = alt;
                        previous[v] = u;
                        queue.Remove(v);        // update the neighbour's priorities
                        queue.Push(v, dist[v]); // now the closest neighbour will be favoured in finding a path
                    }
                }
            }
            Stack<T> S = new Stack<T>();
            u = target;

            while (previous[u] != null)  //follow previous-links from the target to the source
            {
                S.Push(u);             // great use of a Stack!
                u = previous[u];
            }
            S.Push(source);
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
    class Edge<T> : IComparable<Edge<T>>
    {
        public T From { get; set; }
        public T To { get; set; }
        public string Tag { get; set; }
        public int Weight { get; set; }

        public Edge(T from, T to, string tag, int weight = 0)
        {
            this.From = from;
            this.To = to;
            this.Tag = tag;
            this.Weight = weight;
        }
        public override string ToString()
        {
            return string.Format("From {0} to {1} weight: {2}", From.ToString(), To.ToString(), Weight);
        }
        public int CompareTo(Edge<T> that)
        {
            return this.Weight - that.Weight;
        }
    }
}
