using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageGenerator : Generator
{
    public GameObject villagePrefab;
    public float minVillageHeight;
    public float maxVillageHeight;

    List<Village> villages;

    public int minimumVillageDistance;
    public int maxNumberOfVillages;

    public override void Initialise(WorldManager worldManager)
    {
        seed = worldSeed;
        UIManager.UpdateLoadScreenText("Generating villages...");
        base.Initialise(worldManager);
    }

    protected override IEnumerator Generate(WorldManager worldManager)
    {
        if (!WorldInfo.generateBuildings)
        {
            FinishGenerating(worldManager);
            yield return null;
        }

        else
        {
            System.Random rand = new System.Random(seed);
            var world = ObjectStore.instance.worldManager;

            int worldSize = (int)world.worldSize;

            villages = new List<Village>();

            int n = 0;

            for (int i = 0; i < maxNumberOfVillages && n < 100;)
            {
                UIManager.UpdateLoadScreenText($"Constructing village {i}.");

                bool canBuild = true;

                Vector2Int position = new Vector2Int(rand.Next(0, worldSize), rand.Next(0, worldSize));

                try
                {
                    if (world.worldData.heightMap[position.x, position.y] < minVillageHeight || world.worldData.heightMap[position.x, position.y] > maxVillageHeight)
                    {
                        canBuild = false;
                    }
                }
                catch { continue; }

                if (villages.Count > 0)
                    foreach (Village village in villages)
                    {
                        Vector2Int otherPos = new Vector2Int((int)village.transform.position.x, (int)village.transform.position.y);
                        if (Vector2Int.Distance(position, otherPos) < minimumVillageDistance)
                        {
                            canBuild = false;
                        }

                        if (!canBuild)
                            break;
                    }

                if (!canBuild)
                {
                    n++;
                }
                else
                {
                    var go = Instantiate(villagePrefab, new Vector3(position.x, position.y, 0), Quaternion.identity);

                    var v = go.GetComponent<Village>();
                    v.Initialise(worldManager);
                    villages.Add(v);
                    i++;
                    n = 0;

                    worldManager.worldData.SetWorldMapIcon(position.x, position.y, mapIcon);
                }

                yield return null;
            }

            FinishGenerating(worldManager);
        }
    }
}
