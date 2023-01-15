using UnityEngine;
using System.Text;
using System.Collections.Generic;
using Priority_Queue;

public delegate float Heuristic(int a, int b);

public class FinalAgent : PathAgent
{
    public override Pathfinder createPathfinder()
    {
        return new AStarPathfinder(wpDistance);
    }
}

public class AStarPathfinder : Pathfinder
{
	protected Heuristic guessCost;
	public AStarPathfinder(Heuristic h) {
		guessCost = h;
        //navGraph.
	}
    struct nodeInfo
    {
        public int sender;
        public int receiver;
        public double F;
        public nodeInfo(int sender, int receiver, double F)
        {
            this.sender = sender;
            this.receiver = receiver;
            this.F = F;
        }
    }
    public override List<int> findPath(int start, int goal)
    {
        List<int> path = new List<int>();
        SimplePriorityQueue<nodeInfo> pq = new SimplePriorityQueue<nodeInfo>();

        Dictionary<int, int> parent = new Dictionary<int, int>();
        Dictionary<int, double> distance = new Dictionary<int, double>();
        bool pathFound = false;
        List<int> nodes = navGraph.nodes();
        for (int i = 0; i < nodes.Count; ++i) parent.Add(nodes[i], -1);
        for (int i = 0; i < nodes.Count; ++i) distance.Add(nodes[i], -1);

        nodeInfo cur_node = new nodeInfo(-1, start, 0);
        pq.Enqueue(cur_node, 0);
        if (parent[cur_node.receiver] == -1)
        {
            parent[cur_node.receiver] = cur_node.sender;
            distance[cur_node.receiver] = cur_node.F;
        }

        while (pq.Count > 0 && cur_node.receiver != goal)
        {
            cur_node = pq.Dequeue();
            List<int> adj = navGraph.neighbours(cur_node.receiver);
            if (parent[cur_node.receiver] == -1)
            {
                parent[cur_node.receiver] = cur_node.sender;
                distance[cur_node.receiver] = cur_node.F;

                for (int i = 0; i < adj.Count; ++i)
                {
                    int vi = adj[i];
                    if (parent[vi] != -1) continue;


                    double F = distance[cur_node.receiver]
                        + navGraph.getEdgeCost(cur_node.receiver, vi);

                    double H = guessCost(cur_node.receiver, vi);

                    pq.Enqueue(new nodeInfo(cur_node.receiver, vi, F),
                        (float)(F + H)); // MinHeap cost its summ of full path + heuristic cost

                }
            }

            if (cur_node.receiver == goal)
            {
                pathFound = true;
                break;
            }
        }

        if (!pathFound) return path;

        int current = goal;

        while (current != start && current != -1)
        {
            path.Add(current);
            current = parent[current];
        }
        // Path found append start node
        if (current != -1)
            path.Add(start);
        else // Path not found
            return new List<int>();
        Debug.Log("Total COST=" + distance[goal]);
        path.Reverse();

        return path;
    }
}
