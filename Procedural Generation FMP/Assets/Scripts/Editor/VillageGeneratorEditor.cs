using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Village))]
public class VillageGeneratorMK2Editor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Village village = (Village)target;

        if (GUILayout.Button("Generate"))
        {
            village.Initialise(ObjectStore.instance.worldManager);
        }
    }
}
