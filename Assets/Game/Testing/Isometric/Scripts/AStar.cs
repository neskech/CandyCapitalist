using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Result;
using Path = System.Collections.Generic.List<UnityEngine.Vector2Int>;

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

public class AStar 
{
    public Result<Path, PathNotFoundError> FindPath(TileData[,] map, Vector2Int start, 
                                                    Vector2Int end, bool allowDiagonals)
    {
        Queue<Vector2Int> q = new Queue<Vector2Int>();

        return Err<Path, PathNotFoundError>(new PathNotFoundError());
    }

    float GetHeuristic(Vector2Int v1, Vector2Int v2, Heuristic heuristic) => heuristic switch
    {
        Heuristic.Euclidian => Vector2.Distance(v1, v2),
        Heuristic.Manhattan => Mathf.Abs(v1.x - v2.x) + Mathf.Abs(v1.y - v2.y),
        Heuristic.Cherbozev => Mathf.Max(v1.x - v2.x, v1.y - v2.y),
        _ => throw new UnreachableException()
    };
}
