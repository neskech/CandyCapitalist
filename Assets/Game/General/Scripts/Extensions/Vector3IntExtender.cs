using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3IntExtender
{
    public static Vector3Int xzy(this Vector3Int v)
    {
        return new Vector3Int(v.x, v.z, v.y);
    }

    public static Vector3Int yzx(this Vector3Int v)
    {
        return new Vector3Int(v.y, v.z, v.x);
    }

    public static Vector3Int yxz(this Vector3Int v)
    {
        return new Vector3Int(v.y, v.x, v.z);
    }

    public static Vector3Int zxy(this Vector3Int v)
    {
        return new Vector3Int(v.z, v.x, v.y);
    }
     public static Vector3Int zyx(this Vector3Int v)
    {
        return new Vector3Int(v.z, v.y, v.z);
    }

    public static Vector3Int xy(this Vector3Int v)
    {
        return new Vector3Int(v.x, v.y);
    }

    public static Vector3Int yx(this Vector3Int v)
    {
        return new Vector3Int(v.y, v.x);
    }

    public static Vector3Int yz(this Vector3Int v)
    {
        return new Vector3Int(v.y, v.z);
    }

    public static Vector3Int zx(this Vector3Int v)
    {
        return new Vector3Int(v.z, v.x);
    }

    public static Vector3Int xz(this Vector3Int v)
    {
        return new Vector3Int(v.x, v.z);
    }

    ////
    public static Vector2Int xy2D(this Vector3Int v)
    {
        return new Vector2Int(v.x, v.y);
    }

    public static Vector2Int  yx2D(this Vector3Int v)
    {
        return new Vector2Int(v.y, v.x);
    }

    public static Vector2Int  yz2D(this Vector3Int v)
    {
        return new Vector2Int(v.y, v.z);
    }

    public static Vector2Int  zx2D(this Vector3Int v)
    {
        return new Vector2Int(v.z, v.x);
    }

    public static Vector2Int  xz2D(this Vector3Int v)
    {
        return new Vector2Int(v.x, v.z);
    }

}
