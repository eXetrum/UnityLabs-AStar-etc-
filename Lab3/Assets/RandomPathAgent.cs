using UnityEngine;
using System.Text;
using System.Collections.Generic;

public class RandomPathAgent : PathAgent
{
    public override Pathfinder createPathfinder()
    {
        return new RandomPathfinder();
    }
}

public class RandomPathfinder : Pathfinder
{
    public override List<int> findPath(int start, int goal)
    {
        // Implement this
        List<int> path = new List<int>();
        path.Add(start);
        int current = start;
        System.Random rnd = new System.Random(System.DateTime.Now.Millisecond);

        while(current != goal)
        {

            List<int> neighbours = navGraph.neighbours(current);
            // Choose random neighboar
            current = neighbours[rnd.Next(0, neighbours.Count)];
            path.Add(current);
        }


        return path;
    }
}
