using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Pathfinder{

    public static List<Pair<string, string>> prim(PrimGraph g, string root) {
        // Dictionaries
        Dictionary<string, string> res =    new Dictionary<string, string>();
        Dictionary<string, string> PARENT = new Dictionary<string, string>();
        Dictionary<string, int> KEY =       new Dictionary<string, int>();

        // Set the KEY values of all vertices to infinite except for the starting vertex, this KEY value will be set to 0.
        for (int i = 0; i < g.vertices.Count; i++) {
            PARENT[g.vertices[i]] = null;
            KEY[g.vertices[i]] = System.Int32.MaxValue;
        }
        KEY[root] = 0;

        // Create a copy of the list with all the graph's vertices.
        List<string> Q = g.vertices;

        // While the queue has elements left, we take the vertex with the lowest KEY value.
        while (Q.Count != 0) {
            // We take the vertex with the lowest KEY, and remove it from the queue.
            string u = Q[0];
            foreach (string vertex in Q) {
                u = KEY[vertex] < KEY[u] ? vertex : u;       
            }
            Q.Remove(u);

            // Add "u" to the MST.
            if (PARENT[u] != null) {
                res[u] = PARENT[u];
            }

            // We update the KEY value for all vertices connected to "u".
            // KEY should contain the lowest cost possible to connect that vertex to the MST.
            List<Pair<string, Edge>> adjacent = g.adjacent(u);
            foreach (Pair<string, Edge> vertex in adjacent) {
                if (Q.IndexOf(vertex.first) != Q.Count) {
                    if (vertex.second.weight < KEY[vertex.first]) {
                        PARENT[vertex.first] = u;
                        KEY[vertex.first] = vertex.second.weight;
                    }
                }
            }
        }

        // Create output
        List<Pair<string, string>> result = new List<Pair<string, string>>();

        foreach (KeyValuePair<string, string> e in res) {
            Debug.Log(e.Key + " -- " + e.Value);
            result.Add(new Pair<string, string>(e.Key, e.Value));
        }

        return result;
    }
}

// Graph
public class PrimGraph {
    public List<string> vertices;
    public List<Edge> edges;

    public PrimGraph() {
        this.vertices = new List<string>();
        this.edges = new List<Edge>();
    }

    public PrimGraph(List<string> v) {
        this.vertices = v;
        this.edges = new List<Edge>();
    }

    // Connects two vertices with a weighted line.
    public void connect(string v1, string v2, int w) {
        this.edges.Add(new Edge(v1, v2, w));
    }

    // Returns a list of all adjacent vertices
    public List<Pair<string, Edge>> adjacent(string u) {
        List<Pair<string, Edge>> res = new List<Pair<string, Edge>>();
        for (int i = 0; i < edges.Count; i++) {
            if (edges[i].vertex1.Equals(u)) {
                res.Add(new Pair<string, Edge>(edges[i].vertex2, edges[i]));
            }
            else if (edges[i].vertex2.Equals(u)) {
                res.Add(new Pair<string, Edge>(edges[i].vertex1, edges[i]));
            }
        }
        return res;
    }    
};

// Edge
public struct Edge {
    public string vertex1;
    public string vertex2;
    public int weight;

    public Edge(string v1, string v2, int w) {
        this.vertex1 = v1;
        this.vertex2 = v2;
        this.weight = w;
    }
};

// Pair
public class Pair<T, U> {
    public Pair() {
    }

    public Pair(T first, U second) {
        this.first = first;
        this.second = second;
    }

    public T first { get; set; }
    public U second { get; set; }
};