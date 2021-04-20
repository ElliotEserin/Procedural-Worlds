using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Building))]
public class BuildingGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Building tilePrefab = (Building)target;

        if (GUILayout.Button("Generate"))
        {
            tilePrefab.Initialise(ObjectStore.instance.worldManager);
        }
    }
}
