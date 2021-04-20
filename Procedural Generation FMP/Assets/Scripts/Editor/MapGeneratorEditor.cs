using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator mapGen = (MapGenerator)target;
        WorldManager worldGen = FindObjectOfType<WorldManager>();

        if (DrawDefaultInspector())
        {
            if (mapGen.autoUpdate)
                mapGen.Initialise(worldGen);
        }

        if (GUILayout.Button("Generate"))
        {
            mapGen.Initialise(worldGen);
        }

    }
}
