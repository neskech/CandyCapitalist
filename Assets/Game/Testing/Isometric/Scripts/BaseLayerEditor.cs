using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BaseLayout))]
public class BaseLayerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        BaseLayout layout = target as BaseLayout;

        if (GUILayout.Button("Generate Map"))
        {
            layout.CreateTileMap();
        }
        else if (GUILayout.Button("Reset Map"))
        {
            layout.ResetTileMap();
        }
    }
}
