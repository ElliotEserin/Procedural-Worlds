using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VillageGeneratorMK2))]
public class VillageGeneratorMK2Editor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        VillageGeneratorMK2 village = (VillageGeneratorMK2)target;

        if (GUILayout.Button("Generate"))
        {
            village.Initialise(village.debugSeed);
        }
    }
}
