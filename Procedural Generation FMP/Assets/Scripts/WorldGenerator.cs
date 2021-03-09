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

    //Data for the different noise maps
    public NoiseData terrainData;
    public NoiseData temperatureData;
    public NoiseData moistureData;

    WorldData worldData;

    public UnityEngine.UI.InputField iField;
    public UnityEngine.UI.Dropdown dropdown;

    private void Start()
    {
        MapGenerator mapGen = FindObjectOfType<MapGenerator>();

        if (randomSeed)
            seed = RandomString(8).GetHashCode();

        //Generates the world data
        worldData = mapGen.GenerateMap(useCustomSize ? customSize:(int)worldSize, seed, terrainData, temperatureData, moistureData); 

        //Draws the tilemap
        //FindObjectOfType<MapDisplay>().DrawMap(worldData);
    }

    public void GenerateWorld()
    {
        MapGenerator mapGen = FindObjectOfType<MapGenerator>();

        //Generates the world data
        worldData = mapGen.GenerateMap(useCustomSize ? customSize : (int)worldSize, seed, terrainData, temperatureData, moistureData);

        //Draws the tilemap
        //FindObjectOfType<MapDisplay>().DrawMap(worldData);
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
