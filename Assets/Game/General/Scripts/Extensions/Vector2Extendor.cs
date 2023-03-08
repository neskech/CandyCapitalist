using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector2Extendor
{
    public static Vector2 yx(this Vector2 v)
    {
        return new Vector2(v.y, v.x);
    }

    public static Vector2Int CeilToInt(this Vector2 v)
    {
        return new Vector2Int(Mathf.CeilToInt(v.x), Mathf.CeilToInt(v.y));
    }

      public static Vector2Int FloorToInt(this Vector2 v)
    {
        return new Vector2Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y));
    }
}
