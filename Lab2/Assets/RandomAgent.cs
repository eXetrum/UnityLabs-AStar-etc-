using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class RandomAgent : MonoBehaviour
{

    public GameObject waypointSet; // Set from inspector
                                   // Waypoints
    protected Graph navGraph;
    protected List<GameObject> waypoints;
    protected int targetNode;
    protected int prevNode = -1;

    public float speed;
    protected static float NEARBY = 0.2f;
    static System.Random rnd = new System.Random();
    double totalCost = 0;
    // Use this for initialization
    void Start()
    {
        waypoints = new List<GameObject>();
        navGraph = new AdjacencyListGraph();
        findWaypoints();
        buildGraph();
    }

    // Update is called once per frame
    void Update()
    {
        float dist = Vector3.Distance(targetPosition(), transform.position);
        if (dist < NEARBY)
        {
            prevNode = targetNode;
            List<int> nodes = navGraph.neighbours(targetNode);
            targetNode = nodes[rnd.Next(0, nodes.Count)];
            if(prevNode != targetNode)
            totalCost += navGraph.getEdgeCost(prevNode, targetNode);

            Debug.Log("Targeted waypoint " + targetNode + ", totalCost=" + totalCost);
            
        }
    }

    void FixedUpdate()
    {
        Vector3 targetPosn = targetPosition();
        transform.position = Vector3.MoveTowards(transform.position, targetPosn, speed);
    }

    Vector3 targetPosition()
    {
        Vector3 pos = waypoints[targetNode].transform.position;
        pos.y = 0.3f;
        return pos;
    }

    void findWaypoints()
    {
        if (waypointSet != null)
        {
            foreach (Transform t in waypointSet.transform)
            {
                waypoints.Add(t.gameObject);
            }
            Debug.Log("Found " + waypoints.Count + " waypoints.");
        }
        else
        {
            Debug.Log("No waypoints found.");
        }
    }
    private double DistanceBetweenWP(int a, int b)
    {
        if (a < 0 || a >= waypoints.Count || b < 0 || b >= waypoints.Count) return double.MaxValue;
        return Vector3.Distance(waypoints[a].transform.position, waypoints[b].transform.position);
    }
    void buildGraph()
    {
        for (int i = 0; i < waypoints.Count; i++)
        {
            navGraph.addNode(i);
        }
        // TO DO: add edges
        // Waypoint #1
        navGraph.addEdge(0, 1, DistanceBetweenWP(0, 1));
        navGraph.addEdge(0, 2, DistanceBetweenWP(0, 2));
        navGraph.addEdge(0, 3, DistanceBetweenWP(0, 3));
        navGraph.addEdge(0, 7, DistanceBetweenWP(0, 7));
        // Waypoint #2
        navGraph.addEdge(1, 0, DistanceBetweenWP(1, 0));
        navGraph.addEdge(1, 2, DistanceBetweenWP(1, 2));
        navGraph.addEdge(1, 3, DistanceBetweenWP(1, 3));
        // Waypoint #3
        navGraph.addEdge(2, 0, DistanceBetweenWP(2, 0));
        navGraph.addEdge(2, 1, DistanceBetweenWP(2, 1));
        // Waypoint #4
        navGraph.addEdge(3, 0, DistanceBetweenWP(3, 0));
        navGraph.addEdge(3, 1, DistanceBetweenWP(3, 1));
        navGraph.addEdge(3, 4, DistanceBetweenWP(3, 4));
        navGraph.addEdge(3, 8, DistanceBetweenWP(3, 8));
        // Waypoint #5
        navGraph.addEdge(4, 3, DistanceBetweenWP(4, 3));
        navGraph.addEdge(4, 1, DistanceBetweenWP(4, 1));
        navGraph.addEdge(4, 7, DistanceBetweenWP(4, 7));
        // Waypoint #6
        navGraph.addEdge(5, 6, DistanceBetweenWP(5, 6));
        navGraph.addEdge(5, 7, DistanceBetweenWP(5, 7));
        // Waypoint #7
        navGraph.addEdge(6, 5, DistanceBetweenWP(6, 5));
        navGraph.addEdge(6, 7, DistanceBetweenWP(6, 7));
        navGraph.addEdge(6, 8, DistanceBetweenWP(6, 8));
        // Waypoint #8
        navGraph.addEdge(7, 0, DistanceBetweenWP(7, 0));
        navGraph.addEdge(7, 5, DistanceBetweenWP(7, 5));
        navGraph.addEdge(7, 6, DistanceBetweenWP(7, 6));
        navGraph.addEdge(7, 4, DistanceBetweenWP(7, 4));
        navGraph.addEdge(7, 8, DistanceBetweenWP(7, 8));
        // Waypoint #9
        navGraph.addEdge(8, 3, DistanceBetweenWP(8, 3));
        navGraph.addEdge(8, 7, DistanceBetweenWP(8, 7));
        navGraph.addEdge(8, 6, DistanceBetweenWP(8, 6));
    }

}

public interface Graph
{
    bool addNode(int a); // true if node added
    bool addEdge(int a, int b, double cost); // true if edge added
    double getEdgeCost(int a, int b); // Return edge cost
    List<int> nodes();
    List<int> neighbours(int a);
}

public class AdjacencyListGraph : Graph
{
    Dictionary<int, Dictionary<int, double>> vertices = new Dictionary<int, Dictionary<int, double>>();
    // vertex -> adjacency list -> each adjacency vertex has cost

    public bool addEdge(int a, int b, double cost)
    {
        // Vertex a or b not found in vertices dictionary or a and b same vertex then we cant add edge
        if (!vertices.ContainsKey(a) || !vertices.ContainsKey(b) || a == b) return false;
        // If vertex 'a' have edge with vertex 'b' then vertex 'b' should have same edge with 'a' (non directional graph)
        if (vertices[a].ContainsKey(b)) return false;
        vertices[a].Add(b, cost);
        if (vertices[b].ContainsKey(a)) return false;
        vertices[b].Add(a, cost);
        return true;
    }

    public bool addNode(int a)
    {
        if (vertices.ContainsKey(a)) return false;
        vertices.Add(a, new Dictionary<int, double>());
        return true;
    }
    // Return edge cost 
    public double getEdgeCost(int a, int b)
    {
        // Vertices not found
        if (!vertices.ContainsKey(a) || !vertices.ContainsKey(b)) return -1;
        return vertices[a][b];
    }

    public List<int> neighbours(int a) { return new List<int>(vertices[a].Keys); }
    public List<int> nodes() { return new List<int>(vertices.Keys); }
}