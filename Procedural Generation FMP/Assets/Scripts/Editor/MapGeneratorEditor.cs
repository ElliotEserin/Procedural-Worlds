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
        WorldGenerator worldGen = FindObjectOfType<WorldGenerator>();

        if (DrawDefaultInspector())
        {
            if (mapGen.autoUpdate)
                mapGen.GenerateMap(worldGen.useCustomSize ? worldGen.customSize : (int)worldGen.worldSize, worldGen.seed, worldGen.terrainData, worldGen.temperatureData, worldGen.moistureData);
        }

        if (GUILayout.Button("Generate"))
        {
            mapGen.GenerateMap(worldGen.useCustomSize ? worldGen.customSize : (int)worldGen.worldSize, worldGen.seed, worldGen.terrainData, worldGen.temperatureData, worldGen.moistureData);
        }

    }
}
