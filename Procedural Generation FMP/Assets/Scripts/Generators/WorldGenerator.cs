using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGenerator : MonoBehaviour
{
    public enum WorldSize
    {
        Island = 500,
        Small = 1000,
        Medium = 1500,
        Large = 2500,
    }

    public WorldSize worldSize;

    public bool useCustomSize;
    public int customSize;

    //Seed for the world
    public int seed;
    public bool randomSeed;

    public bool useTilemap;
    public bool useVillages;
    public bool useBuildings;
    public bool useRivers;

    public bool useDetail;

    public GameObject playerPrefab;
    public CameraController cameraController;

    //Data for the different noise maps
    [Header("Noise")]
    public NoiseData terrainData;
    public NoiseData temperatureData;
    public NoiseData moistureData;

    [Header("Villages")]
    public GameObject villagePrefab;
    public float minVillageHeight;
    public float maxVillageHeight;

    List<VillageGeneratorMK2> villages;

    public int minimumVillageDistance;
    public int maxNumberOfVillages;

    [Header("Buildings")]
    public GameObject buildingPrefab;
    public float minBuildingHeight;
    public float maxBuildingHeight;

    List<BuildingGenerator> buildings;

    public int minimumBuildingDistance;
    public int maxNumberOfBuildings;

    public WorldData worldData;

    public TilemapPrefab[] tilemapPrefabs;

    private void Start()
    {
        MapGenerator mapGen = FindObjectOfType<MapGenerator>();
        DetailGenerator detailGen = FindObjectOfType<DetailGenerator>();

        if (randomSeed)
            seed = RandomString(8).GetHashCode();

        Generation(mapGen, detailGen);
    }

    void Generation(MapGenerator mapGen, DetailGenerator detailGen)
    {
        //Generates the world data
        worldData = mapGen.GenerateMap(useCustomSize ? customSize : (int)worldSize, seed, terrainData, temperatureData, moistureData);

        //Draws the tilemap
        if (useTilemap)
        {
            ObjectStore.instance.mapDisplay.DrawWorldMap(worldData);
        }

        if (useRivers)
        {
            FindObjectOfType<RiverGenerator>().Initialise(seed);
        }
     
        //Villages
        if (useVillages && useTilemap)
        {
            GenerateVillages();
        }

        //Buildings
        if (useBuildings)
        {
            GenerateBuildings();
        }

        //Detail
        if (useDetail)
        {
            detailGen.Generate(seed, (int)worldSize);
        }

        //Pathfinding
        FindObjectOfType<Grid>().Initialise();

        FindObjectOfType<PathGenerator>().Initialise(seed);

        //Player
        var middle = (int)worldSize / 2;
        var target = Instantiate(playerPrefab, new Vector3(middle, middle, 0), Quaternion.identity);

        cameraController.target = target.transform;

        var weather = FindObjectOfType<DayNightCycle>();

        weather.transform.parent = target.transform;
        weather.transform.localPosition = Vector3.zero;
        weather.spotLights.Add(target.GetComponentInChildren<UnityEngine.Experimental.Rendering.Universal.Light2D>());
    }

    public void GenerateVillages()
    {
        System.Random rand = new System.Random(seed);

        villages = new List<VillageGeneratorMK2>();

        int n = 0;

        for(int i = 0; i < maxNumberOfVillages && n < 100;)
        {
            bool canBuild = true;

            Vector2Int position = new Vector2Int(rand.Next(0, (int)worldSize), rand.Next(0, (int)worldSize));

            try
            {
                if (worldData.heightMap[position.x, position.y] < minVillageHeight || worldData.heightMap[position.x, position.y] > maxVillageHeight)
                {
                    canBuild = false;
                }
            }
            catch { continue; }

            if(villages.Count > 0)
                foreach (VillageGeneratorMK2 village in villages)
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

                var v = go.GetComponent<VillageGeneratorMK2>();
                v.Initialise(seed);
                villages.Add(v);
                i++;
                n = 0;
            }
        }
    }

    public void GenerateBuildings()
    {
        System.Random rand = new System.Random(seed);

        buildings = new List<BuildingGenerator>();

        int n = 0;

        for (int i = 0; i < maxNumberOfBuildings && n < 100;)
        {
            bool canBuild = true;

            Vector2Int position = new Vector2Int(rand.Next(1, (int)worldSize), rand.Next(1, (int)worldSize));

            var height = worldData.heightMap[position.x, position.y];

            if (height < minBuildingHeight || height > maxBuildingHeight)
            {
                canBuild = false;
            }

            if (buildings.Count > 0)
                foreach (BuildingGenerator building in buildings)
                {
                    Vector2Int otherPos = new Vector2Int((int)building.transform.position.x, (int)building.transform.position.y);
                    if (Vector2Int.Distance(position, otherPos) < minimumBuildingDistance)
                    {
                        canBuild = false;
                    }

                    if (!canBuild)
                        break;
                }

            if (villages.Count > 0)
                foreach (VillageGeneratorMK2 village in villages)
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

                var v = go.GetComponent<BuildingGenerator>();
                v.direction = VillageGeneratorMK2.Direction.Up;
                buildings.Add(v);
                i++;
                n = 0;
            }
        }
    }

    public void ChangeWorldSize()
    {
        //switch (dropdown.value)
        //{
        //    case 0:
        //        worldSize = WorldSize.Large;
        //        break;
        //    case 1:
        //        worldSize = WorldSize.Medium;
        //        break;
        //    case 2:
        //        worldSize = WorldSize.Small;
        //        break;
        //    case 3:
        //        worldSize = WorldSize.Island;
        //        break;
        //}
    }

    public void ChangeSeed()
    {
        //seed = iField.text.GetHashCode();
    }

    //Editing the generation in the editor
    void OnValuesUpdated()
    {
        if (!Application.isPlaying)
        {
            FindObjectOfType<MapGenerator>().GenerateMap((int)worldSize, seed, terrainData, temperatureData, moistureData);
        }
    }

    public string RandomString(int length)
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var stringChars = new char[length];
        var random = new System.Random();

        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
        }

        return new string(stringChars);
    }

    //Subscribing to the OnValuesUpdated event
    private void OnValidate()
        {
            if (terrainData != null)
            {
                terrainData.OnValuesUpdated -= OnValuesUpdated;
                terrainData.OnValuesUpdated += OnValuesUpdated;
            }
            if (temperatureData != null)
            {
                temperatureData.OnValuesUpdated -= OnValuesUpdated;
                temperatureData.OnValuesUpdated += OnValuesUpdated;
            }
            if (moistureData != null)
            {
                moistureData.OnValuesUpdated -= OnValuesUpdated;
                moistureData.OnValuesUpdated += OnValuesUpdated;
            }
        }
}

//Stores the world data
public class WorldData : TilemapData
{
    public Texture2D worldMap;
    public float[,] heightMap;

    public TileType[,] tileTypeMap;
}
