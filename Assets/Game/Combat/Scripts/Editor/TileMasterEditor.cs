using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TileMaster))]
public class TileMasterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TileMaster layout = target as TileMaster;

        if (GUILayout.Button("Regenerate"))
        {
            layout.Setup();
        }
    }
}
