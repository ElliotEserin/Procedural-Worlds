using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BuildingGenerator))]
public class BuildingGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BuildingGenerator tilePrefab = (BuildingGenerator)target;

        if (GUILayout.Button("Generate"))
        {
            tilePrefab.Initialise(0);
        }
    }
}
