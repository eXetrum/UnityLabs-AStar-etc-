using UnityEngine;
using System.Text;
using System.Collections.Generic;

public class DepthFirstPathAgent : PathAgent
{
    public override Pathfinder createPathfinder()
    {
        return new DepthFirstPathfinder();
    }
}

public class DepthFirstPathfinder : Pathfinder
{
    public override List<int> findPath(int start, int goal)
    {
        int current = start;

        Stack<int> route = new Stack<int>();
        List<int> visited = new List<int>();
        Dictionary<int, int> parent = new Dictionary<int, int>();
        bool pathFound = false;
        List<int> nodes = navGraph.nodes();
        for (int i = 0; i < nodes.Count; ++i) parent.Add(nodes[i], -1);

        route.Push(start);

        while(route.Count > 0 && !pathFound)
        {
            current = route.Pop();
            if (current == goal)
            {
                pathFound = true;
                break;
            }
            if (visited.Contains(current))
            {
                continue;
            }
            visited.Add(current);

            List<int> adj = navGraph.neighbours(current);
            for(int i = 0; i < adj.Count; ++i)
            {
                int vi = adj[i];
                if(!visited.Contains(vi))
                {
                    route.Push(vi);
                    parent[vi] = current;                    
                }
            }
        }

        List<int> path = new List<int>();
        if (!pathFound) return path;

        current = goal;

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

        path.Reverse();
        return path;
    }
}
