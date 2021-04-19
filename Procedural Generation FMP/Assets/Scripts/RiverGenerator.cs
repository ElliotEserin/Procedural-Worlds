using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverGenerator : Generator
{
    public River riverObject;
    System.Random rand;

    public float minRiverHeight;
    public float maxRiverHeight;

    public int maxNumberOfRivers = 3;

    public override void Initialise(int seed)
    {
        this.seed = seed;
        rand = new System.Random(seed);

        Generate();
    }

    protected override void Generate()
    {
        WorldGenerator wg = FindObjectOfType<WorldGenerator>();

        for (int i = 0, n = 0; i < maxNumberOfRivers && n < 500; n++)
        {
            int x = rand.Next(0, (int)wg.worldSize);
            int y = rand.Next(0, (int)wg.worldSize);

            float height = wg.worldData.heightMap[x, y];

            if (height < maxRiverHeight && height > minRiverHeight)
            {
                Vector3Int startPos = new Vector3Int(x, y, 0);

                var river = Instantiate(riverObject, startPos, Quaternion.identity);

                river.Generate();
                Debug.Log("Generated river");
                i++;
                n = 0;
            }
        }
    }
} 
