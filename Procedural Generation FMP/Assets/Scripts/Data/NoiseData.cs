using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data/NoiseData")]
public class NoiseData : UpdatableData
{
    public float noiseScale;

    public int octaves;
    [Range(0, 1)]
    public float persistance; //Change in amplitude between octaves
    public float lacunarity; //Change in ffrequency between octaves

    public Vector2 offset;  //Custom offset

    //Ensures the persistance and lacunarity are valid
    protected override void OnValidate()
    {
        if (lacunarity < 1)
            lacunarity = 1;

        if (octaves < 0)
            octaves = 0;

        base.OnValidate();
    }
}
