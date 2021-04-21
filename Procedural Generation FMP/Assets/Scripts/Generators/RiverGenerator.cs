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

    public override void Initialise(WorldManager worldManager)
    {
        seed = worldSeed;
        rand = new System.Random(seed);
        UIManager.UpdateLoadScreenText("Generating rivers...");
        base.Initialise(worldManager);
    }

    protected override IEnumerator Generate(WorldManager worldManager)
    {
        for (int i = 0, n = 0; i < maxNumberOfRivers && n < 500; n++)
        {
            UIManager.UpdateLoadScreenText($"Forming river {i}.");

            int x = rand.Next(0, (int)worldManager.worldSize);
            int y = rand.Next(0, (int)worldManager.worldSize);

            float height = worldManager.worldData.heightMap[x, y];

            if (height < maxRiverHeight && height > minRiverHeight)
            {
                Vector3Int startPos = new Vector3Int(x, y, 0);

                var river = Instantiate(riverObject, startPos, Quaternion.identity);

                river.Generate();
                i++;
                n = 0;
            }

            yield return null;
        }

        FinishGenerating(worldManager);
    }
} 
