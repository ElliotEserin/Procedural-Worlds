using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TilemapBuilder))]
public class PrefabBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TilemapBuilder tilePrefab = (TilemapBuilder)target;

        if (GUILayout.Button("Generate"))
        {
            tilePrefab.Generate();
        }
        if (GUILayout.Button("Override"))
        {
            tilePrefab.Override();
        }
    }
}
