using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Mesher 
{
    public static Mesh CreateQuadMesh(Vector2 up, Vector2 right)
    {

        //top left is at the origin
        Vector3 tl = new Vector3(0, 0, BaseLayout.BASE_LAYER_HEIGHT);

        //top right
        Vector2 tr_ = right;
        Vector3 tr = new Vector3(tr_.x, tr_.y, BaseLayout.BASE_LAYER_HEIGHT);

        //bottom left
        Vector2 bl_ = -up;
        Vector3 bl = new Vector3(bl_.x, bl_.y, BaseLayout.BASE_LAYER_HEIGHT);

        //bottom right
        Vector2 br_ = -up + right;
        Vector3 br = new Vector3(br_.x, br_.y, BaseLayout.BASE_LAYER_HEIGHT);

        Vector3[] vertices = {tl, tr, bl, br};
        int[] triangles = {
            3, 0, 2, //triangle 1 (bottom left), clockwise winding
            3, 1, 0 //triangle 2 (top right), clockwise winding
        };

        Mesh m = new Mesh();
        m.vertices = vertices;
        m.triangles = triangles;
        m.normals = new Vector3[]{Vector3.back, Vector3.back, Vector3.back, Vector3.back};
        return m;
    }

    public static Mesh CreateQuadMesh(float width, float height, Vector2 up, Vector2 right)
    {
        //up and right are the basis for the rotated space of the isometric tiles
        Debug.Assert(up.magnitude == 1 && right.magnitude == 1, "vectors must be norm");

        //top left is at the origin
        Vector3 tl = new Vector3(0, 0, BaseLayout.BASE_LAYER_HEIGHT);

        //top right
        Vector2 tr_ = right * width;
        Vector3 tr = new Vector3(tr_.x, tr_.y, BaseLayout.BASE_LAYER_HEIGHT);

        //bottom left
        Vector2 bl_ = -up * height;
        Vector3 bl = new Vector3(bl_.x, bl_.y, BaseLayout.BASE_LAYER_HEIGHT);

        //bottom right
        Vector2 br_ = -up * height + right * width;
        Vector3 br = new Vector3(br_.x, br_.y, BaseLayout.BASE_LAYER_HEIGHT);

        Vector3[] vertices = {tl, tr, bl, br};
        int[] triangles = {
            3, 0, 2, //triangle 1 (bottom left), clockwise winding
            3, 1, 0 //triangle 2 (top right), clockwise winding
        };

        Mesh m = new Mesh();
        m.vertices = vertices;
        m.triangles = triangles;
        m.normals = new Vector3[]{Vector3.back, Vector3.back, Vector3.back, Vector3.back};
        return m;
    }

    public static Mesh CreateQuadMesh(Vector2 topLeft, Vector2 topRight, Vector2 bottomLeft, Vector2 bottomRight)
    {
        Vector3[] vertices = {topLeft, topRight, bottomLeft, bottomRight};
        int[] triangles = {
            3, 0, 2, //triangle 1 (bottom left), clockwise winding
            3, 1, 0 //triangle 2 (top right), clockwise winding
        };

        Mesh m = new Mesh();
        m.vertices = vertices;
        m.triangles = triangles;
        m.normals = new Vector3[]{Vector3.back, Vector3.back, Vector3.back, Vector3.back};
        return m;
    }
}
