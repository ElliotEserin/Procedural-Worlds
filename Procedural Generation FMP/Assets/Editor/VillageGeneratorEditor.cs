using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VillageGenerator))]
public class VillageGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        VillageGenerator village = (VillageGenerator)target;

        if (GUILayout.Button("Generate"))
        {
            village.Initialise(1);
        }

    }
}
