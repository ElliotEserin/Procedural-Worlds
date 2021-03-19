using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(IndividualBuildingGenerator))]
public class BuildingGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        IndividualBuildingGenerator tilePrefab = (IndividualBuildingGenerator)target;

        if (GUILayout.Button("Generate"))
        {
            tilePrefab.Initialise(0);
        }
    }
}
