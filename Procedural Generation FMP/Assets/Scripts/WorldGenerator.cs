using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGenerator : MonoBehaviour
{
    public enum WorldSize
    {
        Island = 100,
        Small = 500,
        Medium = 750,
        Large = 1000,
    }

    public WorldSize worldSize;

    public bool useCustomSize;
    public int customSize;

    //Seed for the world
    public int seed;
    public bool randomSeed;

    public bool useTilemap;
    public bool displayVillages;

    //Data for the different noise maps
    public NoiseData terrainData;
    public NoiseData temperatureData;
    public NoiseData moistureData;

    public GameObject villagePrefab;
    public float minVillageHeight;
    public float maxVillageHeight;

    List<VillageGenerator> villages;

    public int minimumVillageDistance;
    public int maxNumberOfVillages;

    WorldData worldData;

    public UnityEngine.UI.InputField iField;
    public UnityEngine.UI.Dropdown dropdown;

    private void Start()
    {
        MapGenerator mapGen = FindObjectOfType<MapGenerator>();
        MapDisplay mapDisplay = FindObjectOfType<MapDisplay>();

        if (randomSeed)
            seed = RandomString(8).GetHashCode();

        StartCoroutine(Generation(mapGen, mapDisplay));
    }

    IEnumerator Generation(MapGenerator mapGen, MapDisplay mapDisplay)
    {
        //Generates the world data
        worldData = mapGen.GenerateMap(useCustomSize ? customSize : (int)worldSize, seed, terrainData, temperatureData, moistureData);

        yield return null;

        //Draws the tilemap
        if (useTilemap)
        {
            mapDisplay.DrawWorldMap(worldData);

            yield return null;
        }

        if (displayVillages && useTilemap)
        {
            GenerateVillages();

            yield return null;

            foreach (var village in villages)
            {
                mapDisplay.DrawVillage(village.villageData);
            }
        }
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

            if(worldData.heightMap[(int)worldSize - position.x, (int)worldSize - position.y] < minVillageHeight || worldData.heightMap[(int)worldSize - position.x, (int)worldSize - position.y] > maxVillageHeight)
            {
                canBuild = false;
            }   

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
                var go = Instantiate(villagePrefab, new Vector3(position.x, position.y, 0), Quaternion.identity);

                var v = go.GetComponent<VillageGenerator>();
                v.Initialise(seed);
                villages.Add(v);
                i++;
                n = 0;
            }
        }
    }

    public void ChangeWorldSize()
    {
        switch (dropdown.value)
        {
            case 0:
                worldSize = WorldSize.Large;
                break;
            case 1:
                worldSize = WorldSize.Medium;
                break;
            case 2:
                worldSize = WorldSize.Small;
                break;
            case 3:
                worldSize = WorldSize.Island;
                break;
        }
    }

    public void ChangeSeed()
    {
        seed = iField.text.GetHashCode();
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
        var stringChars = new char[8];
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
public struct WorldData
{
    public Texture2D worldMap;
    public float[,] heightMap;
    public Vector3Int[] tilePositions;
    public TileBase[] tiles;
}
