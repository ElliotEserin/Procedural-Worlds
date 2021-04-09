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

    List<VillageGenerator> villages;

    public int minimumVillageDistance;
    public int maxNumberOfVillages;

    [Header("Buildings")]
    public GameObject buildingPrefab;
    public float minBuildingHeight;
    public float maxBuildingHeight;

    List<IndividualBuildingGenerator> buildings;

    public int minimumBuildingDistance;
    public int maxNumberOfBuildings;

    public WorldData worldData;

    public TilemapPrefab[] tilemapPrefabs;

    private void Start()
    {
        MapGenerator mapGen = FindObjectOfType<MapGenerator>();
        DetailGenerator detailGen = FindObjectOfType<DetailGenerator>();
        MapDisplay mapDisplay = FindObjectOfType<MapDisplay>();

        if (randomSeed)
            seed = RandomString(8).GetHashCode();

        Generation(mapGen, detailGen, mapDisplay);
    }

    void Generation(MapGenerator mapGen, DetailGenerator detailGen, MapDisplay mapDisplay)
    {
        //Generates the world data
        worldData = mapGen.GenerateMap(useCustomSize ? customSize : (int)worldSize, seed, terrainData, temperatureData, moistureData);
        Debug.Log("Made world");

        //Draws the tilemap
        if (useTilemap)
        {
            mapDisplay.DrawWorldMap(worldData);
            Debug.Log("Loaded tilemap");
        }
     
        //Villages
        if (useVillages && useTilemap)
        {
            GenerateVillages();

            foreach (var village in villages)
            {
                mapDisplay.DrawVillage(village.villageData);
            }
            Debug.Log("Made villages");
        }

        //Buildings
        if (useBuildings)
        {
            GenerateBuildings();
            Debug.Log("Made buildings");

            foreach (var building in buildings)
            {
                building.Initialise(seed);
            }
        }

        //Detail
        if (useDetail)
        {
            var data = detailGen.Generate(seed, (int)worldSize);
            mapDisplay.DrawDetail(data);
        }

        //Pathfinding
        FindObjectOfType<Grid>().Initialise();

        FindObjectOfType<PathGenerator>().Initialise(seed);
    }

    public void GenerateWorld()
    {
        MapGenerator mapGen = FindObjectOfType<MapGenerator>();
        //Generates the world data
        worldData = mapGen.GenerateMap(useCustomSize ? customSize : (int)worldSize, seed, terrainData, temperatureData, moistureData);

        //Draws the tilemap
        //FindObjectOfType<MapDisplay>().DrawMap(worldData);
    }

    public void GenerateVillages()
    {

        Debug.Log("Generating villages...");
        System.Random rand = new System.Random(seed);

        villages = new List<VillageGenerator>();

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
                foreach (VillageGenerator village in villages)
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
                Debug.Log("Placing village!");

                var go = Instantiate(villagePrefab, new Vector3(position.x, position.y, 0), Quaternion.identity);

                var v = go.GetComponent<VillageGenerator>();
                v.Initialise(seed);
                villages.Add(v);
                i++;
                n = 0;
            }
        }
    }

    public void GenerateBuildings()
    {

        Debug.Log("Generating buildings...");
        System.Random rand = new System.Random(seed);

        buildings = new List<IndividualBuildingGenerator>();

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
                foreach (IndividualBuildingGenerator building in buildings)
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
                foreach (VillageGenerator village in villages)
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
                Debug.Log("Placing building!");

                var go = Instantiate(buildingPrefab, new Vector3(position.x, position.y, 0), Quaternion.identity);

                var v = go.GetComponent<IndividualBuildingGenerator>();
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
