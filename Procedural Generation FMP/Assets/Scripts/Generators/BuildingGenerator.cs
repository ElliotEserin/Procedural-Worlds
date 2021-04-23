using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGenerator : Generator
{
    public GameObject buildingPrefab;
    public float minBuildingHeight;
    public float maxBuildingHeight;

    List<Building> buildings;

    public int minimumBuildingDistance;
    public int maxNumberOfBuildings;

    public override void Initialise(WorldManager worldManager)
    {
        seed = worldSeed;
        UIManager.UpdateLoadScreenText("Generating dungeons...");
        base.Initialise(worldManager);
    }

    protected override IEnumerator Generate(WorldManager worldManager)
    {
        if (!WorldInfo.useVillages)
        {
            FinishGenerating(worldManager);
            yield return null;
        }

        else
        {
            System.Random rand = new System.Random(seed);
            var world = ObjectStore.instance.worldManager;

            int worldSize = (int)world.worldSize;

            buildings = new List<Building>();
            Village[] villages = FindObjectsOfType<Village>();

            int n = 0;

            for (int i = 0; i < maxNumberOfBuildings && n < 100;)
            {
                UIManager.UpdateLoadScreenText($"Building dungeon {i}.");

                bool canBuild = true;

                Vector2Int position = new Vector2Int(rand.Next(1, worldSize), rand.Next(1, worldSize));

                var height = world.worldData.heightMap[position.x, position.y];

                if (height < minBuildingHeight || height > maxBuildingHeight)
                {
                    canBuild = false;
                }

                if (buildings.Count > 0)
                    foreach (Building building in buildings)
                    {
                        Vector2Int otherPos = new Vector2Int((int)building.transform.position.x, (int)building.transform.position.y);
                        if (Vector2Int.Distance(position, otherPos) < minimumBuildingDistance)
                        {
                            canBuild = false;
                        }

                        if (!canBuild)
                            break;
                    }

                if (villages.Length > 0)
                    foreach (Village village in villages)
                    {
                        Vector2Int otherPos = new Vector2Int((int)village.transform.position.x, (int)village.transform.position.y);
                        if (Vector2Int.Distance(position, otherPos) < minimumBuildingDistance)
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
                    var go = Instantiate(buildingPrefab, new Vector3(position.x, position.y, 0), Quaternion.identity);

                    var v = go.GetComponent<Building>();
                    v.direction = Village.Direction.Up;
                    buildings.Add(v);
                    v.Initialise(worldManager);
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
