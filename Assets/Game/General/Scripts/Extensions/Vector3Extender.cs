using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extender
{
    public static Vector3 xzy(this Vector3 v)
    {
        return new Vector3(v.x, v.z, v.y);
    }

    public static Vector3 yzx(this Vector3 v)
    {
        return new Vector3(v.y, v.z, v.x);
    }

    public static Vector3 yxz(this Vector3 v)
    {
        return new Vector3(v.y, v.x, v.z);
    }

    public static Vector3 zxy(this Vector3 v)
    {
        return new Vector3(v.z, v.x, v.y);
    }
     public static Vector3 zyx(this Vector3 v)
    {
        return new Vector3(v.z, v.y, v.z);
    }

    public static Vector3 xy(this Vector3 v)
    {
        return new Vector3(v.x, v.y);
    }

    public static Vector3 yx(this Vector3 v)
    {
        return new Vector3(v.y, v.x);
    }

    public static Vector3 yz(this Vector3 v)
    {
        return new Vector3(v.y, v.z);
    }

    public static Vector3 zx(this Vector3 v)
    {
        return new Vector3(v.z, v.x);
    }

    public static Vector3 xz(this Vector3 v)
    {
        return new Vector3(v.x, v.z);
    }
}
