using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector2IntExtensor
{
    public static Vector2Int yx(this Vector2Int v)
    {
        return new Vector2Int(v.y, v.x);
    }
}
