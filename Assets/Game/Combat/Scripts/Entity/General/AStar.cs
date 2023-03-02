using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Result;
using Option;
using static Option.Option;
using Path = System.Collections.Generic.List<UnityEngine.Vector2Int>;
using Err = PathNotFoundError;

public class PathNotFoundError : Error
{
    //TODO add metadata to error to be more useful, such as path length and start + end
    public override void Present()
    {
        Debug.Log("Path could not be found for A* iteration");
    }
}

public enum Heuristic 
{
    Euclidian,
    Manhattan,
    Cherbozev
}

public static class AStar 
{
    #pragma warning disable
    struct Node 
    {
        public TileData data;
        public float cost;
        public Option<Node> parent;

        public Node(TileData data, float cost, Option<Node> parent)
        {
            this.data = data;
            this.cost = cost;
            this.parent = parent;
        }

        public bool Equals(Node other)
        {
            return this.data.Index == other.data.Index;
        }

        public static bool operator !=(Node c, Node o) => !c.Equals(o);
        public static bool operator ==(Node c, Node o) => c.Equals(o);
    }
    #pragma warning restore

    const Heuristic HEURISTIC = Heuristic.Euclidian;
    const int MAX_Z_DELTA = 2; 
    const bool ALLOW_DIAGONALS = false;

    static Node[,] _modifiedMap;

    public static Result<Path, PathNotFoundError> FindPath(TileData[,] map, Vector2Int start, Vector2Int end)
    {
        ModifyMapData(map);

        Node _start = GetNodeAt(start);
        Node _end = GetNodeAt(end);

        PriorityQueue<Node, float> q = new PriorityQueue<Node, float>();
        HashSet<Node> explored = new HashSet<Node>();
        HashSet<Node> frontier = new HashSet<Node>();

        q.Enqueue(_start, 0);

        while (q.Count != 0)
        {
            Node curr = q.Dequeue();

            explored.Add(curr);
            frontier.Remove(curr);

            //check solution
            if (curr == _end)
                return Ok<Path, Err>(PathFromParentPointers(curr));

            //check adjacents
            Option<Node>[] adjacents = GetAdjacent(curr);
            foreach (Option<Node> adj in adjacents)
            {
                if (adj.IsNone() || explored.Contains(adj.Unwrap())) continue;
                
                Node n = adj.Unwrap();

                int zDelta = Mathf.Abs(curr.data.Index.z - adj.Unwrap().data.Index.z);
                if (zDelta >= MAX_Z_DELTA) continue;


                if (frontier.Contains(n))
                {
                    float newCost = curr.cost + 1 + GetHeuristic(n.data.Index.xy2D(), end, HEURISTIC);
                    if (newCost < n.cost)
                    {
                        n.cost = newCost;
                        n.parent = Some<Node>(curr);
                    }
                }
                else
                {
                    frontier.Add(n);
                    n.cost = curr.cost + 1 + GetHeuristic(n.data.Index.xy2D(), end, HEURISTIC);
                    n.parent = Some<Node>(curr);
                }
            }
            
        }




        //TODO max Z Delta determines the HEIGHT the agent can step
        //TODO so if my current tile has z index 0 and the next in the path has z index 10 
        //TODO then the delta is 10. If my max allowable delta is 6 then we can't make that jump
        return Err<Path, Err>(new Err());
    }

    static float GetHeuristic(Vector2Int v1, Vector2Int v2, Heuristic heuristic) => heuristic switch
    {
        Heuristic.Euclidian => Vector2.Distance(v1, v2),
        Heuristic.Manhattan => Mathf.Abs(v1.x - v2.x) + Mathf.Abs(v1.y - v2.y),
        Heuristic.Cherbozev => Mathf.Max(v1.x - v2.x, v1.y - v2.y),
        _ => throw new UnreachableException()
    };

    static void ModifyMapData(TileData[,] data)
    {
        if (_modifiedMap != null) return;

        //goal is to be able to use 2D indices of tileMap to index into
        //The data array
    }

    static Node GetNodeAt(Vector2Int coords)
    {
        return _modifiedMap[coords.y, coords.x];
    }

    static List<Vector2Int> PathFromParentPointers(Node node)
    {
        List<Vector2Int> path = new List<Vector2Int>();

        while (node.parent.IsSome())
        {
            Vector2Int pos = node.data.Index.xy2D();
            path.Add(pos);
            node = node.parent.Unwrap();
        }

        path.Reverse();

        return path;
    }

    static Option<Node>[] GetAdjacent(Node node)
    {
        Option<Node>[] nodes = new Option<Node>[4];
        Vector2Int currPos = node.data.Index.xy2D();
        int[] adj = new int[]{-1, 1, 0, 0};

        for (int a = 0; a < 4; a++)
        {
            Vector2Int newPos = currPos + new Vector2Int(adj[a], adj[3 - a]);
            //TODO check what axis the dimensions of the array are
            if (newPos.x >= 0 && newPos.x < _modifiedMap.GetLength(0)
                && newPos.y >= 0 && newPos.y < _modifiedMap.GetLength(1))
                nodes[a] = Some<Node>(GetNodeAt(newPos));
            else
                nodes[a] = None<Node>();
                
        }

        return nodes;
    }

}
